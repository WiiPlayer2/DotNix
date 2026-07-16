using System;
using FunicularSwitch.Generators;

namespace DotNix.Parsing.Models;

[UnionType]
public abstract partial record NixStringFragment
{
    public record Text_(string Value) : NixStringFragment;
    
    public record Interpolation_(NixExpression Expression) : NixStringFragment;
}
