using DotNix.Compiling;
using LanguageExt.Traits;
using Char = LanguageExt.Parsec.Char;

namespace DotNix.Parsing;

public partial class NixParser
{
    public static class Literals
    {
        public static Parser<NixExpr> Any => field ??= choice(
            Number,
            List,
            Attrs
        );

        public static Parser<NixExpr> Number => field ??=
            attempt(
                from beginPos in getPos
                from either in TokenParser.NaturalOrFloat
                from endPos in getPos
                let value = either.Match<NixNumber>(
                    i => new NixInteger(i),
                    f => new NixFloat(f)
                )
                select NixExpr.Literal(new PosSpan(beginPos, endPos), value)
            );

        public static Parser<NixExpr> List => field ??=
        (
            from beginPos in TokenParser.Lexeme(attempt(from pos in getPos from _ in ch('[') select pos))
            from exprs in manyUntil(Term, ch(']'))
            from endPos in getPos
            select NixExpr.List(new PosSpan(beginPos, endPos), exprs.ToLst())
        ).label<NixExpr>("list");

        public static Parser<NixExpr> Attrs => field ??=
        (
            from beginPos in TokenParser.Lexeme(attempt(from pos in getPos from _ in ch('{') select pos))
            from statements in manyUntil(AttrsStatement, ch('}'))
            from endPos in getPos
            select NixExpr.Attrs(new(beginPos, endPos), statements.ToList())
        ).label("attrs");

        private static Parser<NixAttrsStmt> AttrsStatement => choice(AttrsAssignment);

        private static Parser<NixAttrsStmt> AttrsAssignment => TokenParser.Lexeme(attempt(
            from beginPos in getPos
            from identifier in WithSpan(TokenParser.Identifier)
            from _ in TokenParser.Symbol("=")
            from expr in Expression
            from _2 in TokenParser.Semi
            from endPos in getPos
            select NixAttrsStmt.Assign(new(beginPos, endPos),
                new(identifier.Span, new NixIdentifier(identifier.Span, identifier.Value)), expr)
        ));
    }
}