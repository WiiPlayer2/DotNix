namespace DotNix.Compiling;

public record NixThunk(AsyncLazy<NixValueThunked> LazyValue) : NixValueThunked
{
    public override AsyncLazy<NixValueStrict> Strict { get; } = new(async () => await (await LazyValue).Strict);

    public override AsyncLazy<NixValue> UnThunk { get; } = new(async () => await (await LazyValue).UnThunk);
}