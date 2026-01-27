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

    private static Parser<NixExpr> Expression => field ??=
        Expr.buildExpressionParser(
            Operators.Table,
            either(TokenParser.Parens(lazyp(() => Expression).Map(x => x.ToPNixExpr())), Literals.Any.Map(x => x.ToPNixExpr())).Map(x => x.Item1));

    private static GenLanguageDef Language => field ??= GenLanguageDef.Empty.With(
        ReservedOpNames: ["+", "-"]
    );

    private static GenTokenParser2 TokenParser => field ??= Token2.makeTokenParser(Language);
}