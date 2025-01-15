using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class CubeInTowerData
{
    public CubeInTowerData(Color color, float offset)
    {
        Color = color;
        Offset = offset;
    }

    public CubeInTowerData()
    {

    }

    [JsonProperty] public Color Color { get; private set; }
    [JsonProperty] public float Offset { get; private set; }
}
