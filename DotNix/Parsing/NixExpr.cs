using System;
using DotNix.Compiling;
using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixExpr
{
    public record Literal_(NixValue2 Value) : NixExpr;

    public record BinaryOp_(BinaryOperatorSymbol Operator, NixExpr Left, NixExpr Right) : NixExpr;
    
    public record UnaryOp_(UnaryOperatorSymbol Operator, NixExpr Expr) : NixExpr;

    public record List_(IReadOnlyList<NixExpr> Items) : NixExpr;

    public record Attrs_(IReadOnlyList<NixBind> Statements) : NixExpr;

    public record LetBinding_(IReadOnlyList<NixBind> Statements, NixExpr Expression) : NixExpr;

    public record Identifier_(NixIdentifier Value) : NixExpr;

    public record Function_(NixFunctionArg Arg, NixExpr Body) : NixExpr;
    
    public record Apply_(NixExpr Func, NixExpr Arg) : NixExpr;
}