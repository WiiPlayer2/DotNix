using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using Char = LanguageExt.Parsec.Char;

namespace DotNix.Parsing;

public static partial class NixParser
{
    public static NixExpr Parse(string code)
    {
        // var test = parse(TreeSitter.Rules.source_code, code);
        
        var result = parse(Expression, code);
        return result.IsFaulted
            ? throw new Exception(result.Reply.Error?.ToString())
            : result.Reply.Result!;
    }

    public static Parser<NixExpr> Term => field ??=
        choice(TokenParser.Parens(lazyp(() => Expression)), Literals.Any, Expressions.Any);
    
    public static Parser<NixExpr> Expression => field ??=
        Expr.buildExpressionParser(
            Operators.Table,
            Term);

    public static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
        ReservedOpNames: ["+", "-"],
        ReservedNames: ["let", "in"],
        IdentLetter: choice(alphaNum, oneOf("-_")),
        IdentStart: choice(letter, ch('_'))
    );

    public static GenTokenParser TokenParser => field ??= Token.makeTokenParser(Language);

    private static Parser<(A Value, PosSpan Span)> WithSpan<A>(Parser<A> p) =>
        from beginPos in getPos
        from value in p
        from endPos in getPos
        select (value, new PosSpan(beginPos, endPos));
}