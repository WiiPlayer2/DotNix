using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixBind
{
    public record Assign_(NixAttrsPath Path, NixExpr Expression) : NixBind;
}