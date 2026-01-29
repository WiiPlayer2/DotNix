using DotNix.Parsing;
using LanguageExt;

namespace DotNix.Compiling;

public class NixCompiler
{
    public static NixValue2 Compile(NixScope scope, NixExpr expr) => expr.Match(
        binaryOp: x => Compile(scope, x),
        literal: Compile,
        list: x => Compile(scope, x),
        attrs: x => Compile(scope, x),
        letBinding: x => Compile(scope, x),
        identifier: x => Compile(scope, x)
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
        };

        NixThunk Op(Func<NixValue2, NixValue2, NixValue2> fn) => new(
            new(async () => fn(await aValue.Strict, await bValue.Strict)
        ));
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

    private static NixValue2 Compile(NixScope scope, NixExpr.LetBinding_ let)
    {
        var letScope = new NixScope(scope, let.Statements.Select(s => s.Match(
            assign: assign => new KeyValuePair<string, NixValue2>(assign.Identifier.Text, Compile(scope, assign.Expression)))
        ).ToDictionary());
        return Compile(letScope, let.Expression);
    }

    private static NixValue2 Compile(NixScope scope, NixExpr.Identifier_ identifier) =>
        scope.Get(identifier.Value.Text).IfNone(() => throw new Exception());
}