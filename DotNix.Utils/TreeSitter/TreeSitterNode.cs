using LanguageExt;

namespace DotNix.Utils.TreeSitter;

public record TreeSitterNode(string Name, string Type, Map<string, TreeSitterNode> Fields)
{
    public static TreeSitterNode Blank => field ??= new(string.Empty, string.Empty, []);
}