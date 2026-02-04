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
    
    public record Thunk_(AsyncLazy<NixValueThunked> LazyValue) : NixValueThunked
    {
        public override AsyncLazy<NixValueStrict> Strict { get; } = new(async () => await (await LazyValue).Strict);

        public override AsyncLazy<NixValue> UnThunk { get; } = new(async () => await (await LazyValue).UnThunk);
    }
}