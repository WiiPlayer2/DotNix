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
        asString(many1(digit)).Map(x => NixExpression.Integer(long.Parse(x)));
    
    // TODO: Implement 0.xxx and E formats
    // /(([1-9][0-9]*\.[0-9]*)|(0?\.[0-9]+))([Ee][+-]?[0-9]+)?/
    private static Parser<NixExpression> FloatExpression => field ??=
        asString(
            from firstDigit in oneOf("123456789")
            from beforeDecimalPoint in many(digit)
            from dot in ch('.')
            from afterDecimalPoint in many(digit)
            select firstDigit + beforeDecimalPoint + dot + afterDecimalPoint
        ).Map(x => NixExpression.Float(double.Parse(x)));

    private static Parser<NixExpression> StringExpression => field ??=
        from _ in unitp
        let quote = ch('"')
        let fragment = choice(
            StringFragment,
            Interpolation
            // TODO: escaped interpolation
            // choice(
            //     EscapeSequence,
            //     chain(DollarEscape, StringFragment.label("$"))))
        )
        from start in quote
        from fragments in many(fragment)
        from end in quote
        select NixExpression.String([..fragments]);
    
    // [^"$\\]|\$(?!\{)|\\.
    private static Parser<NixStringFragment> StringFragment
    {
        get
        {
            var stringChar = StringChar();
            return field ??= asString(many1(stringChar)).Map(NixStringFragment.Text);

            static Parser<char> StringChar() => choice(
                noneOf("\"$\\").Map(x => x),
                from _dollar in ch('$')
                from _10 in notFollowedBy(ch('{'))
                select _dollar,
                ch('\\')
            );
        }
    }

    private static Parser<NixStringFragment> Interpolation => field ??= failure<NixStringFragment>("TODO");

    private static Parser<NixExpression> ExprSimple => field ??= choice(
        VariableExpression,
        attempt(FloatExpression),
        IntegerExpression,
        StringExpression
        // ...
    );

    #endregion
}