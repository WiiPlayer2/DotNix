using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;

namespace DotNix.Parsing;

public static partial class NixParser
{
    public static NixExpr Parse(string code)
    {
        var result = parse(Expression, code);
        return result.IsFaulted
            ? throw new Exception(result.Reply.Error?.ToString())
            : result.Reply.Result!;
    }

    public static Parser<NixExpr> Term => field ??=
        either(TokenParser.Parens(lazyp(() => Expression)), Literals.Any);
    
    public static Parser<NixExpr> Expression => field ??=
        Expr.buildExpressionParser(
            Operators.Table,
            Term);

    public static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
        ReservedOpNames: ["+", "-"]
        // OpStart: oneOf("+-"),
        // OpLetter: oneOf("+-")
    );

    public static GenTokenParser TokenParser => field ??= Token.makeTokenParser(Language);
}