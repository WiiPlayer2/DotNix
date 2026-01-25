using System;
using FunicularSwitch.Generators;

namespace DotNix;

[UnionType]
public abstract partial record NixValue
{
    public record Integer_(long Value) : NixValue;
}
