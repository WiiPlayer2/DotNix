namespace DotNix.Compiling;

public record NixScope(Option<NixScope> Parent, IReadOnlyDictionary<string, NixValue2> Items)
{
    public static NixScope Empty => field ??= new(None, Map<string, NixValue2>());
    
    public Option<NixValue2> Get(string name) =>
        Items.TryGetValue(name)
            .Map(Some)
            .IfNone(() => Parent.Bind(p => p.Get(name)));
}