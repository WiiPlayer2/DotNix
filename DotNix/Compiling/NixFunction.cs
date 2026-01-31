namespace DotNix.Compiling;

public record NixFunction(NixFunc Fn) : NixValueStrict;