using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;

namespace ParserGen;

internal class LstConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType  && typeToConvert.GetGenericTypeDefinition() == typeof(Lst<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter?) Activator.CreateInstance(typeof(Converter<>).MakeGenericType(typeToConvert.GenericTypeArguments[0]));
    
    private class Converter<T> : JsonConverter<Lst<T>>
    {
        public override Lst<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => toList(JsonSerializer.Deserialize<T[]>(ref reader, options));

        public override void Write(Utf8JsonWriter writer, Lst<T> value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
    }
}
