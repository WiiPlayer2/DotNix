using DotNix.Types;

namespace DotNix.Compiling;

public record NixScope(Option<NixScope> Parent, IReadOnlyDictionary<string, NixValueThunked> Items)
{
    public static NixScope Empty => field ??= new(None, Map<string, NixValueThunked>());

    public static NixScope Default => field ??= new(None, Map(
        ("builtins", NixValueThunked.Value(Builtins.AsAttrs)),
        ("true", NixValueThunked.Value(Builtins.True)),
        ("false", NixValueThunked.Value(Builtins.False))
    ));
    
    public Option<NixValueThunked> Get(string name) =>
        Items.TryGetValue(name)
            .Map(Some)
            .IfNone(() => Parent.Bind(p => p.Get(name)));
}