using GenP = LanguageExt.Parsec.Parser<object?>;

namespace DotNix.Parsing;

partial class NixParser
{
    // https://github.com/NixOS/nix/blob/master/src/libexpr/parser.y
    public static class Expressions2
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

        private static GenP generic<A>(Parser<A> p) => p.Map(x => (object?)x);

        private static GenP keyword(string name) => generic(Tokens.Reserved(name));

        private static Operator<object?> BinOp(Assoc assoc, string op) => Operator.Infix(assoc, Tokens.ReservedOp(op).Map<Unit, Func<object?, object?, object?>>(x => object? (a, b) => Array(a, op, b)));

        private static Operator<object?> UnOp(string op) => Operator.Prefix(Tokens.ReservedOp(op).Map<Unit, Func<object?, object?>>(x => a => Array(op, a)));

        private static GenP LET => field ??= keyword("let");
        
        private static GenP IN_KW => field ??= keyword("in");
        
        private static GenP ID => field ??= generic(Tokens.Identifier);
        
        private static GenP OR_KW => field ??= keyword("or");
        
        private static GenP INT_LIT => field ??= generic(Tokens.Float); // TODO
        
        private static GenP FLOAT_LIT => field ??= generic(Tokens.Float); // TODO

        public static GenP Start => field ??= Expr;
        
        public static GenP Expr => field ??= ExprFunction;
        
        public static GenP ExprFunction => field ??=
            generic(choice((GenP[])[
                // ID ':' expr_function
                // formal_set ':' expr_function[body]
                // formal_set '@' ID ':' expr_function[body]
                // ID '@' formal_set ':' expr_function[body]
                // ASSERT expr ';' expr_function
                // WITH expr ';' expr_function
                from _10 in LET
                from _20 in Binds
                from _30 in IN_KW
                from _40 in ExprFunction
                select (object?)Array(_10, _20, _30, _40),
                ExprIf,
            ]));

        public static GenP ExprIf => field ??=
            generic(choice((GenP[]) [
                // IF expr THEN expr ELSE expr
                // expr_pipe_from
                // expr_pipe_into
                ExprOp,
            ]));

        public static GenP ExprOp => field ??=
            buildExpressionParser(
                [
                    [UnOp("-")], // 3
                    [BinOp(Assoc.Left, "*"), BinOp(Assoc.Left, "/")], // 6
                    [BinOp(Assoc.Left, "+"),BinOp(Assoc.Left, "-")], // 7
                ],
                ExprApp
            );

        public static GenP ExprApp => field ??=
            generic(choice((GenP[]) [
                // expr_app expr_select
                ExprSelect,
            ]));

        public static GenP ExprSelect => field ??=
            generic(choice((GenP[]) [
                // expr_simple '.' attrpath
                // expr_simple '.' attrpath OR_KW expr_select
                // expr_simple OR_KW
                ExprSimple,
            ]));

        public static GenP ExprSimple => field ??=
            generic(choice((GenP[]) [
                ID,
                INT_LIT,
                FLOAT_LIT,
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
                from _20 in Binds
                from _30 in Tokens.Symbol("}")
                select (object?)Array(_10, _20, _30),
                // '{' '}'
                from _10 in Tokens.Symbol("[")
                from _20 in List
                from _30 in Tokens.Symbol("]")
                select (object?)Array(_10, _20, _30),
            ]));

        public static GenP Binds => field ??= generic(many(Binds1));
        
        public static GenP Binds1 => field ??=
            generic(choice((GenP[]) [
                // binds1[accum] attrpath '=' expr ';'
                // binds[accum] INHERIT attrs ';'
                // binds[accum] INHERIT '(' expr ')' attrs ';'
                from _10 in Attrpath
                from _20 in Tokens.Symbol("=")
                from _30 in Expr
                from _40 in Tokens.Semi
                select (object?)Array(_10, _20, _30, _40),
            ]));

        public static GenP Attrpath => field ??=
            generic(choice((GenP[]) [
                // attrpath '.' attr
                // attrpath '.' string_attr
                Attr,
                // string_attr
            ]));

        public static GenP Attr => field ??=
            generic(choice((GenP[]) [
                ID,
                OR_KW
            ]));

        public static GenP List => field ??= generic(many(ExprSelect));
    }
}
