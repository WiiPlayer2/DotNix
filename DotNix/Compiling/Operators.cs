namespace DotNix.Compiling;

public static class Operators
{
    public static NixValue2 Plus(NixValue2 a, NixValue2 b) => Builtins.Add((NixNumber)a, (NixNumber)b);

    public static NixValue2 Minus(NixValue2 a, NixValue2 b) => Builtins.Sub((NixNumber)a, (NixNumber)b);
}