namespace DotNix.Parsing;

public partial class NixParser
{
    private static class Literals
    {
        public static Parser<PNixExpr> Any => field ??= Integer;
        
        private static Parser<PNixExpr> Integer => TokenParser.Integer
            .Map(x => (NixExpr.Literal(NixValue.Integer(x.Value)), x.BeginPos, x.EndPos, x.BeginIndex, x.EndIndex));
    }
}