using System;
using DotNix.Helpers;
using FunicularSwitch.Generators;

namespace DotNix.Types;

[UnionType(CaseOrder = CaseOrder.Alphabetic)]
public abstract partial record NixValueThunked
{
    public abstract AsyncLazy<NixValueStrict> Strict { get; }
    
    public abstract AsyncLazy<NixValue> UnThunk { get; }

    public record Value_(NixValue Value__) : NixValueThunked
    {
        public sealed override AsyncLazy<NixValue> UnThunk { get; } = new(() => Task.FromResult(Value__));
        
        public sealed override AsyncLazy<NixValueStrict> Strict { get; } = Value__.Strict;
    }
}
