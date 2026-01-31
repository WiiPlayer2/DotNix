namespace DotNix.Compiling;

public record NixAttrs(IReadOnlyDictionary<string, NixValue2> Items) : NixValue2
{
    public override AsyncLazy<NixValue2> Strict { get; } =
        new(async () =>
            new NixAttrs(
                (
                    await Task.WhenAll(
                        Items.Select(async kv => KeyValuePair.Create(kv.Key, await kv.Value.Strict))
                    )
                ).ToDictionary(x => x.Key, x => x.Value)
            )
        );
}