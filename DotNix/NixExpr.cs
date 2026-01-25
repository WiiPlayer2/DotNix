using System;
using FunicularSwitch.Generators;

namespace DotNix;

[UnionType]
public abstract partial record NixExpr
{
    public record Literal_(NixValue Value) : NixExpr;
    
    public record AddOp_(NixExpr Left, NixExpr Right) : BinaryOp(Left, Right);
    
    public record SubOp_(NixExpr Left, NixExpr Right) : BinaryOp(Left, Right);
    
    public abstract record BinaryOp(NixExpr Left, NixExpr Right) : NixExpr;
}
