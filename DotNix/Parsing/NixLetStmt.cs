using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixLetStmt
{
    public record Assign_(NixIdentifier Identifier, NixExpr Expression) : NixLetStmt;
}