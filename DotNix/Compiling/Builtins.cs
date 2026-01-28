namespace DotNix.Compiling;

public static class Builtins
{
    public static NixInteger Add(NixInteger a, NixInteger b) => new(a.Value + b.Value);
    
    public static NixInteger Sub(NixInteger a, NixInteger b) => new(a.Value - b.Value);
}