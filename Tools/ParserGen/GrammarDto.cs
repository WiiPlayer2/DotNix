using System.Text.Json.Serialization;

record GrammarDto(
    [property: JsonPropertyName("rules")] IDictionary<string, RuleDto> Rules,
    [property: JsonPropertyName("word")] string Word,
    [property: JsonPropertyName("extras")] IReadOnlyList<RuleDto> Extras,
    [property: JsonPropertyName("externals")] IReadOnlyList<RuleDto> Externals,
    [property: JsonPropertyName("supertypes")] IReadOnlyList<string> Supertypes
) : Extensible;