using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISessionData
{
    public List<Color> CubesColors { get; }
    public int CubesInSelectorCount { get; set; }
    public List<CubeInTowerData> CubesInTower { get; }
    public Vector2 FirstCubeInTowerPosition { get; set; }
    public int DistanceBetweenCubes { get; set; }
    public void Save(string fullPath);
    public event Action SaveStart;
}
