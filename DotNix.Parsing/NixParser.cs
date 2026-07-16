using DotNix.Parsing.Models;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;

namespace DotNix.Parsing;

public static class NixParser
{
    public static ParserResult<NixExpression> Parse(string code, CancellationToken cancellationToken)
    {
        var result = parse(ExprSimple, code);
        return result.Match(
            ConsumedOK: (expression, s, arg3) => ParserResult.Ok(expression),
            ConsumedError: error => ParserResult.Error(new ParserError(error.Msg)),
            EmptyOK: (expression, s, arg3) => ParserResult.Error(new ParserError("Empty input")),
            EmptyError: error => ParserResult.Error(new ParserError(error.Msg)));
    }
    
    // https://github.com/nix-community/tree-sitter-nix/blob/master/grammar.js
    #region Parser

    private static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
        IdentStart: either(letter, ch('_')),
        IdentLetter: either(alphaNum, oneOf("_'-"))
    );
    
    private static GenTokenParser Tokens => field ??= Token.makeTokenParser(Language);

    private static Parser<string> Identifier => field ??= Tokens.Identifier.label("identifier");
    
    private static Parser<NixExpression> VariableExpression => field ??= Identifier.Map(NixExpression.Variable);

    private static Parser<NixExpression> IntegerExpression => field ??=
        asString(many1(oneOf("0123456789"))).Map(x => NixExpression.Integer(long.Parse(x)));

    private static Parser<NixExpression> ExprSimple => field ??= choice(
        VariableExpression,
        IntegerExpression
        // ...
    );

    #endregion
}