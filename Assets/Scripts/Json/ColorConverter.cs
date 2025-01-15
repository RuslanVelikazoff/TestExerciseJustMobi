using System;
using UnityEngine;
using Newtonsoft.Json;

public class ColorConverter : JsonConverter<Color>
{
    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        float? r = (float?)reader.ReadAsDouble();
        float? g = (float?)reader.ReadAsDouble();
        float? b = (float?)reader.ReadAsDouble();
        float? a = (float?)reader.ReadAsDouble();
        reader.Read();

        return new Color(r ?? 0, g ?? 0, b ?? 0, a ?? 1);
    }

    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.r);
        writer.WriteValue(value.g);
        writer.WriteValue(value.b);
        writer.WriteValue(value.a);
        writer.WriteEndArray();
    }
}
