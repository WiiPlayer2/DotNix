namespace DotNix.Compiling;

public record NixAttrs(IReadOnlyDictionary<string, NixValue2> Items) : NixValue2;