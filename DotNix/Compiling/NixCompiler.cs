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
            BinaryOperator.Add =>
                new NixThunk(new(async Task<NixValue2> () => Builtins.Add((NixInteger)(await aValue.Strict), (NixInteger)(await bValue.Strict)))),
            BinaryOperator.Sub =>
                new NixThunk(new(async Task<NixValue2> () => Builtins.Sub((NixInteger)(await aValue.Strict), (NixInteger)(await bValue.Strict)))),  
        };
    }
}