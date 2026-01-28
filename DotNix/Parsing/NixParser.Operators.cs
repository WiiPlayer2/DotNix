using LanguageExt.Parsec;

namespace DotNix.Parsing;

public partial class NixParser
{
    public static class Operators
    {
        private static Operator<NixExpr> Binary(Assoc assoc, string op, BinaryOperator @operator)
        {
            return Operator.Infix(
                assoc,
                from beginPos in attempt(from pos in getPos from _ in TokenParser.ReservedOp(op) select pos)
                from endPos in getPos
                select Op(new PosSpan(beginPos, endPos))
            );

            Func<NixExpr, NixExpr, NixExpr> Op(PosSpan opSpan) => (left, right) =>
                NixExpr.BinaryOp(new BinaryOperatorSymbol(opSpan, @operator), left, right);
        }

        public static Operator<NixExpr>[][] Table => field ??=
        [
            [Mul, Div],
            [Add, Sub],
        ];
        
        public static Operator<NixExpr> Add => field ??= Binary(Assoc.Left, "+", BinaryOperator.Plus);
        
        public static Operator<NixExpr> Sub => field ??= Binary(Assoc.Left, "-", BinaryOperator.Minus);
        
        public static Operator<NixExpr> Mul => field ??= Binary(Assoc.Left, "*", BinaryOperator.Mul);
        
        public static Operator<NixExpr> Div => field ??= Binary(Assoc.Left, "/", BinaryOperator.Div);
    }
}