using DotNix.Compiling;
using ExprP = LanguageExt.Parsec.Parser<DotNix.Parsing.NixExpr>;
using KwP = LanguageExt.Parsec.Parser<string>;

namespace DotNix.Parsing;

partial class NixParser
{
    // https://github.com/NixOS/nix/blob/master/src/libexpr/parser.y
    public static class Expressions
    {
        public static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
            CommentLine: "#",
            CommentStart: "/*",
            CommentEnd: "*/",
            NestedComments: false,
            
            ReservedOpNames: [
                "+",
                "-",
                "*",
                "/",
            ],
            ReservedNames: [
                "let",
                "in",
                "or",
            ],
            
            IdentStart: either(letter, ch('_')),
            IdentLetter: either(alphaNum, oneOf("_'-")),
            
            OpStart: oneOf("+-*/"),
            OpLetter: oneOf("+-*/"),
            
            CaseSensitive: true
        );

        public static GenTokenParser Tokens => field ??= Token.makeTokenParser(Language);

        private static KwP keyword(string name) => Tokens.Reserved(name);

        private static Operator<NixExpr> BinOp(Assoc assoc, string op, BinaryOperator op2) =>
            Operator.Infix(
                assoc, 
                Tokens.ReservedOp(op)
                    .Map<Unit, Func<NixExpr, NixExpr, NixExpr>>(x => (a, b) => NixExpr.BinaryOp(new BinaryOperatorSymbol(op2), a, b)));

        private static Operator<NixExpr> UnOp(string op, UnaryOperator op2) =>
            Operator.Prefix(
                Tokens.ReservedOp(op).Map<Unit, Func<NixExpr, NixExpr>>(x => a => NixExpr.UnaryOp(new UnaryOperatorSymbol(op2), a))
            );

        private static KwP LET => field ??= keyword("let");
        
        private static KwP IN_KW => field ??= keyword("in");

        private static Parser<NixIdentifier> ID => field ??= Tokens.Identifier.Map(x => new NixIdentifier(x));
        
        private static KwP OR_KW => field ??= keyword("or");
        
        private static Parser<NixInteger> INT_LIT => field ??=
            from n in Tokens.Integer
            select new NixInteger(n);

        private static Parser<NixFloat> FLOAT_LIT => field ??=
            from n in Tokens.Float
            select new NixFloat(n);

        private static Parser<NixNumber> NUMBER_LIT => field ??=
            from n in Tokens.NaturalOrFloat
            select n.Match<NixNumber>(
                i => new NixInteger(i),
                f => new NixFloat(f)
            );

        public static ExprP Start => field ??= Expr;
        
        public static ExprP Expr => field ??= ExprFunction;
        
        public static ExprP ExprFunction => field ??=
            choice((ExprP[])[
                // ID ':' expr_function
                // formal_set ':' expr_function[body]
                // formal_set '@' ID ':' expr_function[body]
                // ID '@' formal_set ':' expr_function[body]
                // ASSERT expr ';' expr_function
                // WITH expr ';' expr_function
                from _10 in LET
                from binds in Binds
                from _30 in IN_KW
                from expr in ExprFunction
                select NixExpr.LetBinding(toList(binds), expr),
                ExprIf,
            ]);

        public static ExprP ExprIf => field ??=
            choice((ExprP[]) [
                // IF expr THEN expr ELSE expr
                // expr_pipe_from
                // expr_pipe_into
                ExprOp,
            ]);

        public static ExprP ExprOp => field ??=
            buildExpressionParser(
                [
                    [UnOp("-", UnaryOperator.Negate)], // 3
                    [BinOp(Assoc.Left, "*", BinaryOperator.Mul), BinOp(Assoc.Left, "/", BinaryOperator.Div)], // 6
                    [BinOp(Assoc.Left, "+", BinaryOperator.Plus),BinOp(Assoc.Left, "-", BinaryOperator.Minus)], // 7
                ],
                ExprApp
            );

        public static ExprP ExprApp => field ??=
            choice((ExprP[]) [
                // expr_app expr_select
                ExprSelect,
            ]);

        public static ExprP ExprSelect => field ??=
            choice((ExprP[]) [
                // expr_simple '.' attrpath
                // expr_simple '.' attrpath OR_KW expr_select
                // expr_simple OR_KW
                ExprSimple,
            ]);

        public static ExprP ExprSimple => field ??=
            choice((ExprP[]) [
                ID.Map(NixExpr.Identifier),
                NUMBER_LIT.Map(NixExpr.Literal), // INT_LIT + FLOAT_INT
                // '"' string_parts '"'
                // IND_STRING_OPEN ind_string_parts IND_STRING_CLOSE
                // path_start PATH_END
                // path_start string_parts_interpolated PATH_END
                // SPATH
                // URI
                Tokens.Parens(lazyp(() => Expr)),
                // LET '{' binds '}'
                // REC '{' binds '}'
                from _10 in Tokens.Symbol("{")
                from binds in Binds
                from _30 in Tokens.Symbol("}")
                select NixExpr.Attrs(toList(binds)),
                // '{' '}'
                from _10 in Tokens.Symbol("[")
                from list in List
                from _30 in Tokens.Symbol("]")
                select NixExpr.List(toList(list)),
            ]);

        public static Parser<Seq<NixBind>> Binds => field ??= many(Binds1);
        
        public static Parser<NixBind> Binds1 => field ??=
            choice((Parser<NixBind>[]) [
                // binds1[accum] attrpath '=' expr ';'
                // binds[accum] INHERIT attrs ';'
                // binds[accum] INHERIT '(' expr ')' attrs ';'
                from attrpath in Attrpath
                from _20 in Tokens.Symbol("=")
                from expr in Expr
                from _40 in Tokens.Semi
                select NixBind.Assign(attrpath, expr),
            ]);

        public static Parser<NixAttrsPath> Attrpath => field ??=
            choice((Parser<NixAttrsPath>[]) [
                // attrpath '.' attr
                // attrpath '.' string_attr
                Attr,
                // string_attr
            ]);

        public static Parser<NixAttrsPath> Attr => field ??=
            choice(
                ID,
                OR_KW.Map(x => new NixIdentifier(x))
            ).Map(x => new NixAttrsPath(x));

        public static Parser<Seq<NixExpr>> List => field ??= many(ExprSelect);
    }
}
