using System;
using FunicularSwitch.Generators;

namespace DotNix.Parsing.Models;

[UnionType]
public abstract partial record NixAttrPathSegment
{
    public record Identifier_(string Name) : NixAttrPathSegment;
    
    public record Interpolation_(NixExpression Expression) : NixAttrPathSegment;
}
