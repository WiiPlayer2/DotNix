using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;

namespace DotNix.Utils.TreeSitter;

internal class LstJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Lst<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) => (JsonConverter) Activator.CreateInstance(typeof(LstJsonConvert<>).MakeGenericType(typeToConvert.GenericTypeArguments[0]))!;

    private class LstJsonConvert<T> : JsonConverter<Lst<T>>
    {
        public override Lst<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            Prelude.toList(JsonSerializer.Deserialize<IReadOnlyList<T>>(ref reader, options) ?? throw new NotSupportedException());

        public override void Write(Utf8JsonWriter writer, Lst<T> value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize<IReadOnlyList<T>>(writer, value, options);
    }
}
