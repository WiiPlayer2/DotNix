using TreeSitter;

namespace DotNix.Parsing;

internal static class Utils
{
    extension(Node node)
    {        
        public Node GetField(string key) => node.Fields.First(x => x.Key == key).Value;
        
        public IEnumerable<Node> GetFields(string key) => node.Fields.Where(x => x.Key == key).Select(x => x.Value);
    }
}