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
            List
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
    }
}