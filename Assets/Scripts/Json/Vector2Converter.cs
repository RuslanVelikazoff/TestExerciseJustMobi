using System;
using UnityEngine;
using Newtonsoft.Json;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        float? x = (float?)reader.ReadAsDouble();
        float? y = (float?)reader.ReadAsDouble();
        reader.Read();

        return new Vector2(x ?? 0, y ?? 0);
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteEndArray();
    }
}
