using FunicularSwitch.Generators;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValue> EvalExpr(string code)
    {
        if(code.Contains('+'))
        {
            var cst = NixLang.Cst.Parse(code);
            var value = cst.Match(
                expr => expr switch
                {
                    NixLang.Cst.Expr__InfixExpr__Node { InfixExpr0: {} infixExpr } =>
                        long.Parse(infixExpr.Expr0.ToString()) + long.Parse(infixExpr.Expr4.ToString()),
                },
                _ => default
            );
            return NixValue.Integer(value);
        }

        return NixValue.Integer(long.Parse(code));
    }
}

[UnionType]
public abstract partial record NixValue
{
    public record Integer_(long Value) : NixValue;
}
