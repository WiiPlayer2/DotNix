using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixLetStmt(PosSpan Span)
{
    public record Assign_(PosSpan Span, NixIdentifier Identifier, NixExpr Expression) : NixLetStmt(Span);
}