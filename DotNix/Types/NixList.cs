using System;
using DotNix.Compiling;
using DotNix.Helpers;

namespace DotNix.Types;

public record NixList(IReadOnlyList<NixValueThunked> Items) : NixValue(NixValueKind.List)
{
    public override AsyncLazy<NixValueStrict> Strict { get; } =
        new(async () => new NixListStrict(await Task.WhenAll(Items.Select(async x => await x.Strict))));
}

public record NixListStrict(IReadOnlyList<NixValueStrict> Items) : NixValueStrict(NixValueKind.List);