namespace DotNix.Parsing;

internal static class PNixExprExtension
{
    public static (A, PosSpan) ToPosSpan<A>(this (A, Pos, Pos, int, int) tuple) =>
        (tuple.Item1, new PosSpan(tuple.Item2, tuple.Item3));

    public static PNixExpr ToPNixExpr(this NixExpr expr) =>
        (expr, expr.Span.Begin, expr.Span.End, default, default);

    public static Parser<(A Value, PosSpan Span)> ToPosSpan<A>(this Parser<(A, Pos, Pos, int, int)> p) =>
        p.Map(ToPosSpan);

    public static Parser<NixExpr> ToNixExpr(this Parser<PNixExpr> p) =>
        p.Map(x => x.Expr);
}
