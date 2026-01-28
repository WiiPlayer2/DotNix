namespace DotNix.Compiling;

public record NixInteger(long Value) : NixNumber
{
    public NixFloat AsFloat => new(Value);
}

public record NixFloat(double Value) : NixNumber;

public abstract record NixNumber : NixValue2;