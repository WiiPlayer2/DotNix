using DotNix.Parsing;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValue> EvalExpr(string code)
    {
        var expr = NixParser.Parse(code);
        var value = await EvaluateToValue(expr);
        return value;
    }

    private static Task<NixExpr> Evaluate(NixExpr expr) => expr.Match<Task<NixExpr>>(
        addOp: op =>
            from left in EvaluateToValue(op.Left)
            from right in EvaluateToValue(op.Right)
            select left.Match(
                integer: a => right.Match(
                    integer: b =>  NixExpr.Literal(NixValue.Integer(a.Value + b.Value))
                )
            ),
        subOp: op =>
            from left in EvaluateToValue(op.Left)
            from right in EvaluateToValue(op.Right)
            select left.Match(
                integer: a => right.Match(
                    integer: b =>  NixExpr.Literal(NixValue.Integer(a.Value - b.Value))
                )
            ),
        literal: _ => Task.FromResult(expr)
    );

    private static async Task<NixValue> EvaluateToValue(NixExpr expr)
    {
        var currentExpr = expr;
        while(currentExpr is not NixExpr.Literal_)
            currentExpr = await Evaluate(currentExpr);
        return ((NixExpr.Literal_) currentExpr).Value;
    }
}