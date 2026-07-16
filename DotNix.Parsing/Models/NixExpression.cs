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

    public static NixExpression String(string text) => String(NixStringFragment.Text(text));
}