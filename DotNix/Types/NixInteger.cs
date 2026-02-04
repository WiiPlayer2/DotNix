using System;

namespace DotNix.Types;

public record NixInteger(long Value) : NixNumber(NixValueKind.Integer)
{
    public NixFloat AsFloat => new(Value);
}

public record NixFloat(double Value) : NixNumber(NixValueKind.Float);

public abstract record NixNumber(NixValueKind Kind) : NixValueStrict(Kind);