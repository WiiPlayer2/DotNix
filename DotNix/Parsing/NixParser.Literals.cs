using Char = LanguageExt.Parsec.Char;

namespace DotNix.Parsing;

public partial class NixParser
{
    private static class Literals
    {
        public static Parser<NixExpr> Any => field ??= choice(Integer, List);
        
        private static Parser<NixExpr> Integer => TokenParser.Integer
            .Map(x => NixExpr.Literal(x.ToPosSpan().Item2, NixValue.Integer(x.Value)));

        private static Parser<NixExpr> List =>
            from start in TokenParser.Lexeme(Char.ch('[')).ToPosSpan()
            from exprs in TokenParser.SepBy<NixExpr, Unit>(Expression.Map(x => x.ToPNixExpr()), TokenParser.WhiteSpace)
                .ToPosSpan()
            from end in TokenParser.Lexeme(Char.ch(']')).ToPosSpan()
            select NixExpr.List(start.Span | exprs.Span | end.Span, exprs.Value.ToLst());
    }
}