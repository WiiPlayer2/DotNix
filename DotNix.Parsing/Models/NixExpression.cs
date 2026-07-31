using FunicularSwitch.Generators;
using LanguageExt;

namespace DotNix.Parsing.Models;

[UnionType]
public abstract partial record NixExpression
{
    public record Variable_(string Name) : NixExpression;
    
    public record Integer_(long Value) : NixExpression;
    
    public record Float_(double Value) : NixExpression;

    public record String_(params Lst<NixStringFragment> Fragments) : NixExpression;
    
    public record Path_(params Lst<NixStringFragment> Fragments) : NixExpression;
    
    public record Attrs_(params Lst<NixBinding> Bindings) : NixExpression;
    
    public record List_(params Lst<NixExpression> Items) : NixExpression;

    public static NixExpression String(string text) => String(NixStringFragment.Text(text));
    
    public static NixExpression Path(string text) => Path(NixStringFragment.Text(text));
}