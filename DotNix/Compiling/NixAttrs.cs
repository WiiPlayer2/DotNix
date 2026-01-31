namespace DotNix.Compiling;

public record NixAttrs(IReadOnlyDictionary<string, NixValueThunked> Items) : NixValue
{
    public override AsyncLazy<NixValueStrict> Strict { get; } =
        new(async () =>
            new NixAttrsStrict(
                (
                    await Task.WhenAll(
                        Items.Select(async kv => KeyValuePair.Create(kv.Key, await kv.Value.Strict))
                    )
                ).ToDictionary(x => x.Key, x => x.Value)
            )
        );
}

public record NixAttrsStrict(IReadOnlyDictionary<string, NixValueStrict> Items) : NixValueStrict
{
    public NixAttrs ToUnstrict() =>
        new(Items.ToDictionary(x => x.Key, NixValueThunked (x) => x.Value));
}