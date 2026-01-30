using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using Char = LanguageExt.Parsec.Char;

namespace DotNix.Parsing;

public static partial class NixParser
{
    public static NixExpr Parse(string code)
    {
        var result = parse(Expressions.Start, code);
        return result.IsFaulted
            ? throw new Exception(result.Reply.Error?.ToString())
            : result.Reply.Result!;
    }
}