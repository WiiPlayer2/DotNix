using System.Text;
using System.Text.Json.Serialization;

record Extensible
{
    [JsonExtensionData]
    public IDictionary<string, object?>? ExtensionData { get; init; }

    protected virtual bool PrintMembers(StringBuilder builder) => false;
}