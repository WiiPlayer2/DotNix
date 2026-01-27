namespace DotNix.Parsing;

internal static class PNixExprExtension
{
    public static (A, PosSpan) ToPosSpan<A>(this (A, Pos, Pos, int, int) tuple) =>
        (tuple.Item1, new PosSpan(tuple.Item2, tuple.Item3));

    public static PNixExpr ToPNixExpr(this NixExpr expr) =>
        (expr, expr.Span.Begin, expr.Span.End, default, default);
}
