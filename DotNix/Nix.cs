using System.Collections.ObjectModel;
using DotNix.Compiling;
using DotNix.Parsing;
using DotNix.Types;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValueStrict> EvalExpr(string code)
    {
        var expr = NixParser.Parse(code);
        var lazyValue = NixCompiler.Compile(NixScope.Default, expr);
        var value = await lazyValue.Strict;
        return value;
    }
}