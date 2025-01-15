using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CubeGenerator : MonoBehaviour
{
    public float DistanceBetweenCubes => _DistanceBetweenCubes;
    [SerializeField] private Cube _CubePrefab;
    private float _DistanceBetweenCubes;
    private int _CubesCount;
    private List<Color> _Colors;
    private CubeSelectorZone _CubeSelectorZone;
    private ISessionData _SessionData;

    private void Start()
    {
        _CubeSelectorZone = GetComponent<CubeSelectorZone>();
        GenerateCubes();
    }

    [Inject]
    private void Initialzie(ISessionData sessionData)
    {
        _SessionData = sessionData;
        _Colors = _SessionData.CubesColors;
        _CubesCount = _SessionData.CubesInSelectorCount;
        _DistanceBetweenCubes = _SessionData.DistanceBetweenCubes;
    }

    private void GenerateCubes()
    {
        float cubeWidth = _CubePrefab.Width;
        float halfCubeWidth = _CubePrefab.HalfWidth;
        float startX = _CubeSelectorZone.Rect.x + _DistanceBetweenCubes + halfCubeWidth;

        float offset = cubeWidth + _DistanceBetweenCubes;
        int colorIndex = 0;
        
        for (int i = 0; i < _CubesCount; i++)
        {
            Cube cube = _CubeSelectorZone.CreateCube(_CubePrefab, transform);
            float x = startX + (offset * i);
            cube.LocalPositon = new Vector2(x, 0);
            
            colorIndex++;
            colorIndex = colorIndex < _Colors.Count ? colorIndex : 0;

            cube.Color = _Colors[colorIndex];
        }
    }
}
