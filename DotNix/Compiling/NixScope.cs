using DotNix.Types;

namespace DotNix.Compiling;

public record NixScope(Option<NixScope> Parent, IReadOnlyDictionary<string, NixValueThunked> Items)
{
    public static NixScope Empty => field ??= new(None, Map<string, NixValueThunked>());

    public static NixScope Default => field ??= new(None, Map<string, NixValueThunked>(
        ("builtins", Builtins.AsAttrs),
        ("true", Builtins.True),
        ("false", Builtins.False)
    ));
    
    public Option<NixValueThunked> Get(string name) =>
        Items.TryGetValue(name)
            .Map(Some)
            .IfNone(() => Parent.Bind(p => p.Get(name)));
}