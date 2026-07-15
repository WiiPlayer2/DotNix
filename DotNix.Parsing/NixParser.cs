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
        var result = Prim.parse(VariableExpression, code);
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

    #endregion
}