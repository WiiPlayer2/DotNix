using System;
using DotNix.Compiling;
using DotNix.Helpers;

namespace DotNix.Types;

public abstract record NixValue
{
    public abstract AsyncLazy<NixValueStrict> Strict { get; }
}

public abstract record NixValueStrict : NixValue
{
    public NixValueStrict()
    {
        Strict = new(() => Task.FromResult(this));
    }

    public sealed override AsyncLazy<NixValueStrict> Strict { get; }
}