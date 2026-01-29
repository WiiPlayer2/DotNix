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
            Attrs,
            LetBinding,
            IdentifierExpr
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

        private static Parser<(A Value, PosSpan Span)> WithSpan<A>(Parser<A> p) =>
            from beginPos in getPos
            from value in p
            from endPos in getPos
            select (value, new PosSpan(beginPos, endPos));

        public static Parser<NixExpr> LetBinding => field ??= TokenParser.Lexeme(attempt(
            from beginPos in getPos
            from _10 in TokenParser.Reserved("let")
            from statements in manyUntil(LetStmt, TokenParser.Reserved("in"))
            from expression in Expression
            from endPos in getPos
            select NixExpr.LetBinding(new(beginPos, endPos), statements.ToLst(), expression)
        ));

        private static Parser<NixLetStmt> LetStmt => field ??= TokenParser.Lexeme(attempt(
            from beginPos in getPos
            from identifier in Identifier
            from _10 in TokenParser.Symbol("=")
            from expression in Expression
            from _20 in TokenParser.Semi
            from endPos in getPos
            select NixLetStmt.Assign(new(beginPos, endPos), identifier, expression)
        ));

        public static Parser<NixExpr> IdentifierExpr => Identifier.Map(NixExpr.Identifier);
        
        public static Parser<NixIdentifier> Identifier => field ??= TokenParser.Lexeme(attempt(
            from text in WithSpan(TokenParser.Identifier)
            select new NixIdentifier(text.Span, text.Value)
        ));
    }
}