using DotNix.Compiling;
using DotNix.Parsing;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValue2> EvalExpr(string code)
    {
        var expr = NixParser.Parse(code);
        var lazyValue = NixCompiler.Compile(expr);
        var value = await lazyValue.Strict;
        return value;
    }
}