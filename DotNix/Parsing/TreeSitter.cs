namespace DotNix.Parsing;

partial class TreeSitter
{
    public class MyRules : Rules
    {
        public override Parser<object?> string_fragment => TODO;

        public override Parser<object?> _indented_string_fragment => TODO;

        public override Parser<object?> _path_start => field ??=
            from _10 in ch('.')
            from _20 in optional(ch('.'))
            select (object?) $"{_10}{_20}";

        public override Parser<object?> _indented_dollar_escape => TODO;

        public override Parser<object?> dollar_escape => TODO;

        public override Parser<object?> path_fragment => TODO;

        public override Parser<object?> _expr_op => field ??= Expr.buildExpressionParser(
            [
                [BinaryOp(Assoc.Right, "->")],
                [BinaryOp(Assoc.Left, "||")],
                [BinaryOp(Assoc.Left, "&&")],
                [BinaryOp(Assoc.Left, "=="), BinaryOp(Assoc.Left, "!=")],
                [BinaryOp(Assoc.Left, "<"), BinaryOp(Assoc.Left, "<="), BinaryOp(Assoc.Left, ">"), BinaryOp(Assoc.Left, ">=")],
                [BinaryOp(Assoc.Right, "//")],
                [UnaryOp("!")],
                [BinaryOp(Assoc.Left, "+"), BinaryOp(Assoc.Left, "-")],
                [BinaryOp(Assoc.Left, "*"), BinaryOp(Assoc.Left, "/")],
                [BinaryOp(Assoc.Left, "++")],
                // has_attr_expression
                [UnaryOp("-")],
            ],
            lazyp(() => _expr_apply_expression)
        );
        
        public override Parser<object?> _expr_apply_expression => field ??=
            choice((Parser<object?>[])[
                attempt(lazyp(() => _expr_select_expression)),
                attempt(lazyp(() => apply_expression)),
            ]);

        private Operator<object?> UnaryOp(string op) => Operator.Prefix(
            str(op).Map<string, Func<object?, object?>>(_ => identity)
        );
        
        private Operator<object?> BinaryOp(Assoc assoc, string op) => Operator.Infix(
            assoc,
            str(op).Map<string, Func<object?, object?, object?>>(_ => (a, b) => List(a, b))
        );
    }
}