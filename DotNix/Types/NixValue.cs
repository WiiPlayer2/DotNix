using System;
using DotNix.Compiling;
using DotNix.Helpers;

namespace DotNix.Types;

public abstract record NixValue : NixValueThunked
{
    public NixValue()
    {
        UnThunk = new(() => Task.FromResult(this));
    }

    public sealed override AsyncLazy<NixValue> UnThunk { get; }
}

public abstract record NixValueThunked
{
    public abstract AsyncLazy<NixValueStrict> Strict { get; }
    
    public abstract AsyncLazy<NixValue> UnThunk { get; }
}

public abstract record NixValueStrict : NixValue
{
    public NixValueStrict()
    {
        Strict = new(() => Task.FromResult(this));
    }

    public sealed override AsyncLazy<NixValueStrict> Strict { get; }
}