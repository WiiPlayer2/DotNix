using FunicularSwitch.Generators;

namespace DotNix.Parsing.Models;

[UnionType]
public abstract partial record NixExpression
{
    public record Variable_(string Name) : NixExpression;
    
    public record Integer_(long Value) : NixExpression;
}