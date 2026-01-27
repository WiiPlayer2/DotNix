namespace DotNix.Parsing;

public partial class NixParser
{
    private static class Literals
    {
        public static Parser<NixExpr> Any => field ??= Integer;
        
        private static Parser<NixExpr> Integer => TokenParser.Integer
            .Map(x => NixExpr.Literal(x.ToPosSpan().Item2, NixValue.Integer(x.Value)));
    }
}