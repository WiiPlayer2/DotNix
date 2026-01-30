namespace DotNix.Compiling;

public static class Operators
{
    public static NixValue2 Plus(NixValue2 a, NixValue2 b) => Builtins.Add((NixNumber)a, (NixNumber)b);

    public static NixValue2 Minus(NixValue2 a, NixValue2 b) => Builtins.Sub((NixNumber)a, (NixNumber)b);
    
    public static NixValue2 Mul(NixValue2 a, NixValue2 b) => Builtins.Mul((NixNumber)a, (NixNumber)b);
    
    public static NixValue2 Div(NixValue2 a, NixValue2 b) => Builtins.Div((NixNumber)a, (NixNumber)b);

    public static NixValue2 Negate(NixValue2 a) =>
        a switch
        {
            NixInteger aInt => new NixInteger(-aInt.Value),
            NixFloat aFloat => new NixFloat(-aFloat.Value),
            _ => throw new NotSupportedException(),
        };
}