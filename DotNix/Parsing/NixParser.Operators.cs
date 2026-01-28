using LanguageExt.Parsec;

namespace DotNix.Parsing;

public partial class NixParser
{
    private static class Operators
    {
        private static Operator<NixExpr> Binary(Assoc assoc, string op, BinaryOperator @operator)
        {
            return Operator.Infix(
                assoc,
                TokenParser.ReservedOp(op).Map(x => Op(x.ToPosSpan().Item2))
            );

            Func<NixExpr, NixExpr, NixExpr> Op(PosSpan opSpan) => (left, right) =>
                NixExpr.BinaryOp(new BinaryOperatorSymbol(opSpan, @operator), left, right);
        }

        public static Operator<NixExpr>[][] Table => field ??=
        [
            [Add, Sub],
        ];
        
        public static Operator<NixExpr> Add => field ??= Binary(Assoc.Left, "+", BinaryOperator.Plus);
        
        public static Operator<NixExpr> Sub => field ??= Binary(Assoc.Left, "-", BinaryOperator.Minus);
    }
}