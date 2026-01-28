namespace DotNix.Compiling;

public static class Builtins
{
    public static NixNumber Add(NixNumber a, NixNumber b) =>
        a is NixInteger aInt && b is NixInteger bInt
            ? Add(aInt, bInt)
            : throw new NotImplementedException();

    public static NixNumber Sub(NixNumber a, NixNumber b) =>
        a is NixInteger aInt && b is NixInteger bInt
            ? Sub(aInt, bInt)
            : throw new NotImplementedException();
    
    public static NixInteger Add(NixInteger a, NixInteger b) => new(a.Value + b.Value);
    
    public static NixInteger Sub(NixInteger a, NixInteger b) => new(a.Value - b.Value);
}