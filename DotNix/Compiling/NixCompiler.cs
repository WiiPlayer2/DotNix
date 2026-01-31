using DotNix.Parsing;
using DotNix.Types;
using LanguageExt;

namespace DotNix.Compiling;

public class NixCompiler
{
    public static NixValueThunked Compile(NixScope scope, NixExpr expr) => expr.Match(
        unaryOp: x => CompileUnaryOp(scope, x),
        binaryOp: x => CompileBinaryOp(scope, x),
        literal: CompileLiteral,
        list: x => CompileList(scope, x),
        attrs: x => CompileAttrs(scope, x),
        letBinding: x => CompileLet(scope, x),
        identifier: x => CompileIdentifier(scope, x),
        function: x => CompileFunction(scope, x),
        apply: x => CompileApply(scope, x),
        with: x => CompileWith(scope, x),
        selection: x => CompileSelection(scope, x),
        hasAttr: x => CompileHasAttr(scope, x),
        @if: x => CompileIf(scope, x)
    );

    private static NixValueThunked CompileLiteral(NixExpr.Literal_ literalExpr) => NixValueThunked.Value(literalExpr.Value);

    private static NixThunk CompileBinaryOp(NixScope scope, NixExpr.BinaryOp_ binaryOp)
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
            BinaryOperator.Concat => Op(Operators.Concat),
            BinaryOperator.Update => Op(Operators.Update),
        };

        NixThunk Op(Func<NixValue, NixValue, NixValue> fn) => new(
            new(async () => NixValueThunked.Value(fn(await aValue.UnThunk, await bValue.UnThunk))
        ));
    }

    private static NixThunk CompileUnaryOp(NixScope scope, NixExpr.UnaryOp_ unaryOp)
    {
        var aValue = Compile(scope, unaryOp.Expr);
        return unaryOp.Operator.Operator switch
        {
            UnaryOperator.Negate => Op(Operators.Negate),
            UnaryOperator.Not => Op(Operators.Not),
        };

        NixThunk Op(Func<NixValue, NixValue> fn) => new(
            new(async () => NixValueThunked.Value(fn(await aValue.Strict)))
        );
    }

    private static NixValueThunked CompileList(NixScope scope, NixExpr.List_ list) =>
        NixValueThunked.Value(new NixList(list.Items.Select(x => Compile(scope, x)).ToList()));

    private static NixValueThunked CompileAttrs(NixScope scope, NixExpr.Attrs_ attrs) => NixValueThunked.Value(new NixAttrs(
        attrs.Statements
            .Select(x => x.Match(
                assign: assign =>
                    new KeyValuePair<string, NixValueThunked>(assign.Path.Identifier.Text, Compile(scope, assign.Expression))
            ))
            .ToDictionary()
    ));

    private static NixValueThunked CompileLet(NixScope scope, NixExpr.LetBinding_ let)
    {
        NixScope letScope = default!;
        letScope = new NixScope(scope, let.Statements.Select(s => s.Match(
            // ReSharper disable once AccessToModifiedClosure
            assign: assign => new KeyValuePair<string, NixValueThunked>(assign.Path.Identifier.Text, Helper.Thunk(() => Compile(letScope, assign.Expression))))
        ).ToDictionary());
        return Compile(letScope, let.Expression);
    }

    private static NixValueThunked CompileIdentifier(NixScope scope, NixExpr.Identifier_ identifier) =>
        scope.Get(identifier.Value.Text).IfNone(() => throw new Exception($"{identifier.Value.Text} not in scope"));

    private static NixValueThunked CompileFunction(NixScope scope, NixExpr.Function_ function) =>
        NixValueThunked.Value(new NixFunction(arg =>
        {
            var fnScope = new NixScope(scope, function.Arg.Match<IReadOnlyDictionary<string, NixValueThunked>>(
                simple: simple => Map((simple.Name.Text, arg))
            ));
            var result = Compile(fnScope, function.Body);
            return Task.FromResult(result);
        }));

    private static NixValueThunked CompileApply(NixScope scope, NixExpr.Apply_ apply)
    {
        var func = Compile(scope, apply.Func);
        var arg = Compile(scope, apply.Arg);
        return new NixThunk(new(async () =>
        {
            var fn = (NixFunction) await func.Strict;
            return await fn.Fn(arg);
        }));
    }

    private static NixValueThunked CompileWith(NixScope scope, NixExpr.With_ with)
    {
        return new NixThunk(new(async () =>
        {
            var bindValue = await Compile(scope, with.BindExpr).UnThunk;
            var items = bindValue switch
            {
                NixAttrs bindAttrs => bindAttrs.Items,
                NixAttrsStrict bindAttrsStrict => bindAttrsStrict.ToUnstrict().Items,
                _ => throw new InvalidOperationException(),
            };
            
            var withScope = new NixScope(
                scope,
                items
            );
            return Compile(withScope, with.Expression);
        }));
    }

    private static NixValueThunked CompileSelection(NixScope scope, NixExpr.Selection_ selection)
    {
        var attrs = Compile(scope, selection.Expr);
        return Helper.Thunk(async () => ((NixAttrs) await attrs.UnThunk).Items[selection.AttrsPath.Identifier.Text]);
    }

    private static NixValueThunked CompileHasAttr(NixScope scope, NixExpr.HasAttr_ hasAttr)
    {
        var attrs = Compile(scope, hasAttr.Expr);
        return Helper.Thunk(async () => NixValueThunked.Value((NixBool) ((NixAttrs) await attrs.UnThunk).Items.ContainsKey(hasAttr.AttrsPath.Identifier.Text)));
    }

    private static NixValueThunked CompileIf(NixScope scope, NixExpr.If_ @if)
    {
        var cond = Compile(scope, @if.IfCond);
        return Helper.Thunk(async () => ((NixBool) await cond.UnThunk).Value ? Compile(scope, @if.Then) : Compile(scope, @if.Else));
    }
}