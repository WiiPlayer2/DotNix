using TreeSitter;

namespace DotNix.Parsing;

internal static class Utils
{
    extension(Node node)
    {        
        public Node GetField(string key) => node.Fields.First(x => x.Key == key).Value;
    }
}