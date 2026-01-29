namespace DotNix.Parsing;

public partial class NixParser
{
    public static class Expressions
    {
        public static Parser<NixExpr> Any => field ??= choice(
            LetBinding,
            IdentifierExpr
        );

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