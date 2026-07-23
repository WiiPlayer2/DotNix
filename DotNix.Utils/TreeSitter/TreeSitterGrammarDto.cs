using System.Text.Json.Serialization;
using LanguageExt;

namespace DotNix.Utils.TreeSitter;

internal record TreeSitterGrammarDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("word")] string Word,
    [property: JsonPropertyName("rules")] IReadOnlyDictionary<string, TreeSitterGrammarRuleDto> Rules,
    [property: JsonPropertyName("extras")] IReadOnlyList<TreeSitterGrammarRuleDto> Extras,
    [property: JsonPropertyName("conflicts")] IReadOnlyList<object> Conflicts,
    [property: JsonPropertyName("precedences")] IReadOnlyList<object> Precedences,
    [property: JsonPropertyName("externals")] IReadOnlyList<TreeSitterGrammarRuleDto> Externals,
    [property: JsonPropertyName("inline")] IReadOnlyList<object> Inline,
    [property: JsonPropertyName("supertypes")] IReadOnlyList<string> Supertypes
);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Choice), "CHOICE")]
[JsonDerivedType(typeof(Field), "FIELD")]
[JsonDerivedType(typeof(Symbol), "SYMBOL")]
[JsonDerivedType(typeof(Blank), "BLANK")]
[JsonDerivedType(typeof(Pattern), "PATTERN")]
[JsonDerivedType(typeof(Seq), "SEQ")]
[JsonDerivedType(typeof(Alias), "ALIAS")]
[JsonDerivedType(typeof(Repeat), "REPEAT")]
[JsonDerivedType(typeof(Repeat1), "REPEAT1")]
[JsonDerivedType(typeof(String), "STRING")]
[JsonDerivedType(typeof(Prec), "PREC")]
[JsonDerivedType(typeof(PrecLeft), "PREC_LEFT")]
[JsonDerivedType(typeof(PrecRight), "PREC_RIGHT")]
[JsonDerivedType(typeof(ImmediateToken), "IMMEDIATE_TOKEN")]
[JsonDerivedType(typeof(Token), "TOKEN")]
internal abstract record TreeSitterGrammarRuleDto
{
    public record Choice(
        [property: JsonPropertyName("members")] Lst<TreeSitterGrammarRuleDto> Members
    ) : TreeSitterGrammarRuleDto;

    public record Field(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto
    {
        public override string ToString() => $"field({Name}, {Content})";
    }

    public record Symbol(
        [property: JsonPropertyName("name")] string Name
    ) : TreeSitterGrammarRuleDto
    {
        public override string ToString() => $"$.{Name}";
    }
    
    public record Blank : TreeSitterGrammarRuleDto;

    public record Pattern(
        [property: JsonPropertyName("value")] string Value
    ) : TreeSitterGrammarRuleDto
    {
        public override string ToString() => $"/{Value}/";
    }

    public record Seq(
        [property: JsonPropertyName("members")] Lst<TreeSitterGrammarRuleDto> Members
    ) : TreeSitterGrammarRuleDto
    {
        public override string ToString() => $"seq({Members})";
    }
    
    public record Alias(
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content,
        [property: JsonPropertyName("named")] bool Named,
        [property: JsonPropertyName("value")] string Value
    ) : TreeSitterGrammarRuleDto;
    
    public record Repeat(
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;
    
    public record Repeat1(
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;

    public record String(
        [property: JsonPropertyName("value")] string Value
    ) : TreeSitterGrammarRuleDto
    {
        public override string ToString() => $"\"{Value}\"";
    }

    public record Prec(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;

    public record PrecLeft(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;

    public record PrecRight(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;
    
    public record ImmediateToken(
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;
    
    public record Token(
        [property: JsonPropertyName("content")] TreeSitterGrammarRuleDto Content
    ) : TreeSitterGrammarRuleDto;
}

internal record TreeSitterNodeTypeDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("named")] bool Named,
    [property: JsonPropertyName("subtypes")] Lst<TreeSitterNodeTypeDto>? Subtypes,
    [property: JsonPropertyName("fields")] IReadOnlyDictionary<string, TreeSitterFieldTypeDto>? Fields,
    [property: JsonPropertyName("children")] TreeSitterFieldTypeDto? Children
);

internal record TreeSitterFieldTypeDto(
    [property: JsonPropertyName("multiple")] bool Multiple,
    [property: JsonPropertyName("required")] bool Required,
    [property: JsonPropertyName("types")] Lst<TreeSitterNodeTypeRefDto> Types
);

internal record TreeSitterNodeTypeRefDto(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("named")] bool Named
);