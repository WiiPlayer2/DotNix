namespace DotNix.Compiling;

public static class Builtins
{
    public static NixNumber Add(NixNumber a, NixNumber b) =>
        CalcOp(Add, Add, a, b);

    public static NixNumber Sub(NixNumber a, NixNumber b) =>
        CalcOp(Sub, Sub, a, b);
    
    public static NixNumber Mul(NixNumber a, NixNumber b) =>
        CalcOp(Mul, Mul, a, b);

    public static NixNumber Div(NixNumber a, NixNumber b) =>
        CalcOp(Div, Div, a, b);

    private static NixNumber CalcOp(Func<NixInteger, NixInteger, NixInteger> integerFn, Func<NixFloat, NixFloat, NixFloat> floatFn, NixNumber a, NixNumber b) =>
        a is NixInteger aInt
            ? b is NixInteger bInt
                ? integerFn(aInt, bInt)
                : floatFn(aInt.AsFloat, (NixFloat) b)
            : b is NixInteger bInt2
                ? floatFn((NixFloat) a, bInt2.AsFloat)
                : floatFn((NixFloat) a, (NixFloat) b);
    
    public static NixInteger Add(NixInteger a, NixInteger b) => new(a.Value + b.Value);
    
    public static NixInteger Sub(NixInteger a, NixInteger b) => new(a.Value - b.Value);
    
    public static NixInteger Mul(NixInteger a, NixInteger b) => new(a.Value * b.Value);
    
    public static NixInteger Div(NixInteger a, NixInteger b) => new(a.Value / b.Value);
    
    public static NixFloat Add(NixFloat a, NixFloat b) => new(a.Value + b.Value);
    
    public static NixFloat Sub(NixFloat a, NixFloat b) => new(a.Value - b.Value);
    
    public static NixFloat Mul(NixFloat a, NixFloat b) => new(a.Value * b.Value);
    
    public static NixFloat Div(NixFloat a, NixFloat b) => new(a.Value / b.Value);
}