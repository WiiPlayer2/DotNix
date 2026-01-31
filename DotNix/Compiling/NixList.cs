namespace DotNix.Compiling;

public record NixList(IReadOnlyList<NixValueThunked> Items) : NixValue
{
    public override AsyncLazy<NixValueStrict> Strict { get; } =
        new(async () => new NixListStrict(await Task.WhenAll(Items.Select(async x => await x.Strict))));
}

public record NixListStrict(IReadOnlyList<NixValueStrict> Items) : NixValueStrict;