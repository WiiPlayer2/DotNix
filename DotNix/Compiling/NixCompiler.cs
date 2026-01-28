using DotNix.Parsing;
using LanguageExt;

namespace DotNix.Compiling;

public class NixCompiler
{
    public static NixValue2 Compile(NixExpr expr) => expr.Match(
        binaryOp: Compile,
        literal: Compile
    );

    private static NixValue2 Compile(NixExpr.Literal_ literalExpr) => literalExpr.Value.Match(
        integer: Compile
    );

    private static NixInteger Compile(NixValue.Integer_ integer) => new(integer.Value);

    private static NixThunk Compile(NixExpr.BinaryOp_ binaryOp)
    {
        var aValue = Compile(binaryOp.Left);
        var bValue = Compile(binaryOp.Right);
        return binaryOp.Operator.Operator switch
        {
            BinaryOperator.Plus => Op(Operators.Plus),
            BinaryOperator.Minus => Op(Operators.Minus),  
        };

        NixThunk Op(Func<NixValue2, NixValue2, NixValue2> fn) => new(
            new(async () => fn(await aValue.Strict, await bValue.Strict)
        ));
    }
}