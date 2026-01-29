using System.Text.Json.Serialization;
using FunicularSwitch.Generators;
using LanguageExt;

[UnionType(CaseOrder = CaseOrder.Alphabetic)]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Choice_), "CHOICE")]
[JsonDerivedType(typeof(Symbol_), "SYMBOL")]
[JsonDerivedType(typeof(Pattern_), "PATTERN")]
[JsonDerivedType(typeof(Field_), "FIELD")]
[JsonDerivedType(typeof(Seq_), "SEQ")]
[JsonDerivedType(typeof(String_), "STRING")]
[JsonDerivedType(typeof(Prec_), "PREC")]
[JsonDerivedType(typeof(ImmediateToken_), "IMMEDIATE_TOKEN")]
[JsonDerivedType(typeof(Repeat1_), "REPEAT1")]
[JsonDerivedType(typeof(Token_), "TOKEN")]
[JsonDerivedType(typeof(Blank_), "BLANK")]
[JsonDerivedType(typeof(PrecLeft_), "PREC_LEFT")]
[JsonDerivedType(typeof(PrecRight_), "PREC_RIGHT")]
[JsonDerivedType(typeof(Alias_), "ALIAS")]
[JsonDerivedType(typeof(Repeat_), "REPEAT")]
partial record RuleDto() : Extensible
{
    public record Choice_(
        [property: JsonPropertyName("members")] IReadOnlyList<RuleDto> Members
    ) : RuleDto;

    public record Symbol_(
        [property: JsonPropertyName("name")] string Name
    ) : RuleDto;
    
    public record Pattern_(
        [property: JsonPropertyName("value")] string Value
    ) : RuleDto;
    
    public record Field_(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record Seq_(
        [property: JsonPropertyName("members")] Lst<RuleDto> Members
    ) : RuleDto;
    
    public record String_(
        [property: JsonPropertyName("value")] string Value
    ) : RuleDto;
    
    public record Prec_(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record ImmediateToken_(
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record Repeat1_(
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record Token_(
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record Blank_ : RuleDto;
    
    public record PrecLeft_(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
    
    public record PrecRight_(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;

    public record Alias_(
        [property: JsonPropertyName("value")] string Value,
        [property: JsonPropertyName("named")] bool Named,
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;

    public record Repeat_(
        [property: JsonPropertyName("content")] RuleDto Content
    ) : RuleDto;
}