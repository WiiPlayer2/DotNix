using FunicularSwitch.Generators;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DotNix.Utils.TreeSitter;

public record TreeSitterNode(Lst<string> Types, string Content, Map<string, Lst<TreeSitterNode>> Fields)
{
    public string Type => Types.Count == 0 ? string.Empty : Types[0];
    
    public static TreeSitterNode operator +(TreeSitterNode a, TreeSitterNode b) => a with
    {
        Types = a.Types + b.Types,
        Content = a.Content + b.Content,
        Fields = toMap(a.Fields.Keys.Concat(b.Fields.Keys).Distinct().Map(field => (field, a.Fields.GetValueOrDefault(field, []) + b.Fields.GetValueOrDefault(field, [])))),
    };

    public static TreeSitterNode Blank => field ??= new([], string.Empty, []);

    public static TreeSitterNode Token(string content) => Blank with {Content = content};

    public static TreeSitterNode Token(string type, string content) => Token(content).AddType(type);

    public static TreeSitterNode Field(string name, TreeSitterNode value) => Token(value.Content) with
    {
        Fields = [(name, [value])],
    };

    public TreeSitterNode AddType(string type) => this + Blank with {Types = [type]};

    public TreeSitterNode AddField(string name, TreeSitterNode value) => this + Blank with
    {
        Fields = [(name, [value])],
    };
}