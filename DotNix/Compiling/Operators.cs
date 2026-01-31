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

    public static NixValue2 And(NixValue2 a, NixValue2 b) => BoolOp(a, b, (a, b) => a && b);
    
    public static NixValue2 Or(NixValue2 a, NixValue2 b) => BoolOp(a, b, (a, b) => a || b);
    
    public static NixValue2 Impl(NixValue2 a, NixValue2 b) => BoolOp(a, b, (a, b) => !a || b);
    
    private static NixBool BoolOp(NixValue2 a, NixValue2 b, Func<bool, bool, bool> fn) =>
        a switch
        {
            NixBool aBool => b switch
            {
                NixBool bBool => (NixBool) fn(aBool.Value, bBool.Value),
            },
        };

    public static NixValue2 Not(NixValue2 a) =>
        a switch
        {
            NixBool aInt => (NixBool) !(aInt.Value),
        };
}