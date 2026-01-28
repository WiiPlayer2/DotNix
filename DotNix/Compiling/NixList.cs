namespace DotNix.Compiling;

public record NixList(IReadOnlyList<NixValue2> Items) : NixValue2
{
    public override AsyncLazy<NixValue2> Strict { get; } =
        new(async () => new NixList(await Task.WhenAll(Items.Select(async x => await x.Strict))));
}