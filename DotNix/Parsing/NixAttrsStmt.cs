using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixAttrsStmt(PosSpan Span)
{
    public record Assign_(PosSpan Span, NixAttrsPath Path, NixExpr Expression) : NixAttrsStmt(Span);
}