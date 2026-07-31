using LanguageExt;
using TreeSitter;

namespace DotNix.Parsing;

internal static class Utils
{
    extension(Node node)
    {        
        public Node GetField(string key) => node.Fields.First(x => x.Key == key).Value;

        public Option<Node> TryGetField(string key) => Prelude.Optional(node.GetFields(key).FirstOrDefault());
        
        public IEnumerable<Node> GetFields(string key) => node.Fields.Where(x => x.Key == key).Select(x => x.Value);
    }
}