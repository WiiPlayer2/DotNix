using LanguageExt.Parsec;

namespace DotNix.Parsing;

public partial class NixParser
{
    private static class Operators
    {
        private static Operator<PNixExpr> Binary(Assoc assoc, string op, Func<NixExpr, NixExpr, NixExpr> fn)
        {
            return Operator.Infix(
                assoc,
                TokenParser.ReservedOp(op).Map(_ => new Func<PNixExpr, PNixExpr, PNixExpr>(Op))
            );

            PNixExpr Op(PNixExpr left, PNixExpr right) =>
                (fn(left.Expr, right.Expr), left.BeginPos, right.EndPos, left.BeginIndex, right.EndIndex);
        }

        public static Operator<PNixExpr>[][] Table => field ??=
        [
            [Add, Sub],
        ];
        
        public static Operator<PNixExpr> Add => field ??= Binary(Assoc.Left, "+", NixExpr.AddOp);
        
        public static Operator<PNixExpr> Sub => field ??= Binary(Assoc.Left, "-", NixExpr.SubOp);
    }
}