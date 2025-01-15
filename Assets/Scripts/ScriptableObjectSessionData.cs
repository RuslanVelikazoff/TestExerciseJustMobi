using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SessionData", menuName = "Session data", order = 0)]
public class ScriptableObjectSessionData : ScriptableObject
{
    public IReadOnlyList<Color> CubesColors => _CubesColors;
    public int CubesInSelectorCount => _CubesInSelectorCount;
    public List<CubeInTowerData> CubesInTower => _CubesInTower;
    public Vector2 FirstCubeInTowerPosition => _FirstCubeInTowerPosition;
    public int DistanceBetweenCubes => _DistanceBetweenCubes;
    [SerializeField] private List<Color> _CubesColors;
    [SerializeField, Min(0)] private int _CubesInSelectorCount;
    [SerializeField] private List<CubeInTowerData> _CubesInTower;
    [SerializeField] private Vector2 _FirstCubeInTowerPosition;
    [SerializeField, Min(0)] private int _DistanceBetweenCubes;

    public void Save()
    {

    }
}
