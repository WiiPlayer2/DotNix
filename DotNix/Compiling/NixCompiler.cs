using DotNix.Parsing;
using LanguageExt;

namespace DotNix.Compiling;

public class NixCompiler
{
    public static NixValue2 Compile(NixScope scope, NixExpr expr) => expr.Match(
        unaryOp: x => Compile(scope, x),
        binaryOp: x => Compile(scope, x),
        literal: Compile,
        list: x => Compile(scope, x),
        attrs: x => Compile(scope, x),
        letBinding: x => CompileLet(scope, x),
        identifier: x => Compile(scope, x),
        function: x => Compile(scope, x),
        apply: x => Compile(scope, x),
        with: x => Compile(scope, x)
    );

    private static NixValue2 Compile(NixExpr.Literal_ literalExpr) => literalExpr.Value;

    private static NixThunk Compile(NixScope scope, NixExpr.BinaryOp_ binaryOp)
    {
        var aValue = Compile(scope, binaryOp.Left);
        var bValue = Compile(scope, binaryOp.Right);
        return binaryOp.Operator.Operator switch
        {
            BinaryOperator.Plus => Op(Operators.Plus),
            BinaryOperator.Minus => Op(Operators.Minus),
            BinaryOperator.Mul => Op(Operators.Mul),
            BinaryOperator.Div => Op(Operators.Div),
            BinaryOperator.And => Op(Operators.And),
            BinaryOperator.Or => Op(Operators.Or),
            BinaryOperator.Impl => Op(Operators.Impl),
            BinaryOperator.Equal => Op(Operators.Equal),
            BinaryOperator.NotEqual => Op(Operators.NotEqual),
            BinaryOperator.LessThan => Op(Operators.LessThan),
            BinaryOperator.LessThanOrEqual => Op(Operators.LessThanOrEqual),
            BinaryOperator.GreaterThan => Op(Operators.GreaterThan),
            BinaryOperator.GreaterThanOrEqual => Op(Operators.GreaterThanOrEqual),
        };

        NixThunk Op(Func<NixValue2, NixValue2, NixValue2> fn) => new(
            new(async () => fn(await aValue.UnThunk, await bValue.UnThunk)
        ));
    }

    private static NixThunk Compile(NixScope scope, NixExpr.UnaryOp_ unaryOp)
    {
        var aValue = Compile(scope, unaryOp.Expr);
        return unaryOp.Operator.Operator switch
        {
            UnaryOperator.Negate => Op(Operators.Negate),
            UnaryOperator.Not => Op(Operators.Not),
        };

        NixThunk Op(Func<NixValue2, NixValue2> fn) => new(
            new(async () => fn(await aValue.Strict))
        );
    }

    private static NixList Compile(NixScope scope, NixExpr.List_ list) => new(list.Items.Select(x => Compile(scope, x)).ToList());

    private static NixAttrs Compile(NixScope scope, NixExpr.Attrs_ attrs) => new(
        attrs.Statements
            .Select(x => x.Match(
                assign: assign =>
                    new KeyValuePair<string, NixValue2>(assign.Path.Identifier.Text, Compile(scope, assign.Expression))
            ))
            .ToDictionary()
    );

    private static NixValue2 CompileLet(NixScope scope, NixExpr.LetBinding_ let)
    {
        NixScope letScope = default!;
        letScope = new NixScope(scope, let.Statements.Select(s => s.Match(
            // ReSharper disable once AccessToModifiedClosure
            assign: assign => new KeyValuePair<string, NixValue2>(assign.Path.Identifier.Text, Thunk(() => Compile(letScope, assign.Expression))))
        ).ToDictionary());
        return Compile(letScope, let.Expression);
    }

    private static NixValue2 Compile(NixScope scope, NixExpr.Identifier_ identifier) =>
        scope.Get(identifier.Value.Text).IfNone(() => throw new Exception($"{identifier.Value.Text} not in scope"));

    private static NixValue2 Compile(NixScope scope, NixExpr.Function_ function)
    {
        return new NixFunction(arg =>
        {
            var fnScope = new NixScope(scope, function.Arg.Match<IReadOnlyDictionary<string, NixValue2>>(
                simple: simple => Map((simple.Name.Text, arg))
            ));
            var result = Compile(fnScope, function.Body);
            return Task.FromResult(result);
        });
    }

    private static NixValue2 Compile(NixScope scope, NixExpr.Apply_ apply)
    {
        var func = Compile(scope, apply.Func);
        var arg = Compile(scope, apply.Arg);
        return new NixThunk(new(async () =>
        {
            var fn = (NixFunction) await func.Strict;
            return await fn.Fn(arg);
        }));
    }

    private static NixValue2 Compile(NixScope scope, NixExpr.With_ with)
    {
        return new NixThunk(new AsyncLazy<NixValue2>(async () =>
        {
            var bindValue = await Compile(scope, with.BindExpr).Strict; // TODO need .Unthunk or similar
            if (bindValue is not NixAttrs bindAttrs)
                throw new InvalidOperationException();
            
            var withScope = new NixScope(
                scope,
                bindAttrs.Items
            );
            return Compile(withScope, with.Expression);
        }));
    }

    private static NixThunk Thunk(Func<NixValue2> fn) => new(new(() => Task.FromResult(fn())));
}