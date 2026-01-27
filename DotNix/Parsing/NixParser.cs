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
            : result.Reply.Result.Item1;
    }

    private static Parser<PNixExpr> Expression => field ??=
        Expr.buildExpressionParser(
            Operators.Table,
            either(TokenParser.Parens(lazyp(() => Expression)), Literals.Any));

    private static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
        ReservedOpNames: ["+", "-"]
    );

    private static GenTokenParser2 TokenParser => field ??= Token2.makeTokenParser(Language);
}