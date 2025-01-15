using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonSessionData : ISessionData
{
    public JsonSessionData()
    {
        
    }

    [JsonProperty] public List<Color> CubesColors { get; private set; } = new List<Color>();
    [JsonProperty] public int CubesInSelectorCount { get; set; }
    [JsonProperty] public List<CubeInTowerData> CubesInTower { get; private set; } = new List<CubeInTowerData>();
    [JsonProperty] public Vector2 FirstCubeInTowerPosition { get; set; }
    [JsonProperty] public int DistanceBetweenCubes { get; set; }
    private static JsonConverter[] _Converters = new JsonConverter[]
    {
        new Vector2Converter(),
        new ColorConverter()
    };
    public event Action SaveStart;

    public void Save(string fullPath)
    {
        SaveStart?.Invoke();
        File.WriteAllText(fullPath, JsonConvert.SerializeObject(this, _Converters));
    }

    public static JsonSessionData Load(string fullPath)
    {
        if (!File.Exists(fullPath))
            return new JsonSessionData();

        return JsonConvert.DeserializeObject<JsonSessionData>(File.ReadAllText(fullPath), _Converters) ?? new JsonSessionData();
    }
}
