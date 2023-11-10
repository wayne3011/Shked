using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SkedGroupsService.Application.Extensions.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";
    // public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer) => writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
    //
    // public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue,
    //     JsonSerializer serializer) =>  DateOnly.ParseExact((string)reader.Value, Format, CultureInfo.InvariantCulture);

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.ParseExact((string)reader.GetString(), Format, CultureInfo.InvariantCulture);
    }
    
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}