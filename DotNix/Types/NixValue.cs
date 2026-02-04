using System;
using DotNix.Compiling;
using DotNix.Helpers;

namespace DotNix.Types;

public abstract record NixValue(NixValueKind Kind)
{
    public abstract AsyncLazy<NixValueStrict> Strict { get; }
}

public abstract record NixValueStrict : NixValue
{
    public NixValueStrict(NixValueKind kind) : base(kind)
    {
        Strict = new(() => Task.FromResult(this));
    }

    public sealed override AsyncLazy<NixValueStrict> Strict { get; }
}