namespace DotNix.Compiling;

public record NixThunk(AsyncLazy<NixValue2> LazyValue) : NixValue2
{
    public override AsyncLazy<NixValue2> Strict { get; } = new(async () => await (await LazyValue).Strict);
}