using System;
using DotNix.Compiling;
using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixExpr(PosSpan Span)
{
    public record Literal_(PosSpan Span, NixValue2 Value) : NixExpr(Span);

    public record BinaryOp_(BinaryOperatorSymbol Operator, NixExpr Left, NixExpr Right) : NixExpr(Left.Span | Operator.Span | Right.Span);

    public record List_(PosSpan Span, IReadOnlyList<NixExpr> Items) : NixExpr(Span);

    public record Attrs_(PosSpan Span, IReadOnlyList<NixAttrsStmt> Statements) : NixExpr(Span);

    public record LetBinding_(PosSpan Span, IReadOnlyList<NixLetStmt> Statements, NixExpr Expression) : NixExpr(Span);

    public record Identifier_(NixIdentifier Value) : NixExpr(Value.Span);
}