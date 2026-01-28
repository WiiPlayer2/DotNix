using DotNix.Parsing;
using LanguageExt;

namespace DotNix.Compiling;

public class NixCompiler
{
    public static NixValue2 Compile(NixExpr expr) => expr.Match(
        binaryOp: Compile,
        literal: Compile,
        list: Compile,
        attrs: Compile
    );

    private static NixValue2 Compile(NixExpr.Literal_ literalExpr) => literalExpr.Value;

    private static NixThunk Compile(NixExpr.BinaryOp_ binaryOp)
    {
        var aValue = Compile(binaryOp.Left);
        var bValue = Compile(binaryOp.Right);
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

    private static NixList Compile(NixExpr.List_ list) => new(list.Items.Select(Compile).ToList());

    private static NixAttrs Compile(NixExpr.Attrs_ attrs) => new(
        attrs.Statements
            .Select(x => x.Match(
                assign: assign =>
                    new KeyValuePair<string, NixValue2>(assign.Path.Identifier.Text, Compile(assign.Expression))
            ))
            .ToDictionary()
    );
}