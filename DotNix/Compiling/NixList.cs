namespace DotNix.Compiling;

public record NixList(IReadOnlyList<NixValue2> Items) : NixValue2;