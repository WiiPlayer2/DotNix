using FunicularSwitch.Generators;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValue> EvalExpr(string code)
    {
        return NixValue.Integer(long.Parse(code));
    }
}

[UnionType]
public abstract partial record NixValue
{
    public record Integer_(long Value) : NixValue;
}