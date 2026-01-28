namespace DotNix.Compiling;

public record NixInteger(long Value) : NixNumber;

public abstract record NixNumber : NixValue2;