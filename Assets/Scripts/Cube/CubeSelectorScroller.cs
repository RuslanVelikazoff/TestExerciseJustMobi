using UnityEngine;
using Zenject;

public class CubeSelectorScroller : MonoBehaviour
{
    public bool IsActive
    {
        get => _IsActive;
        set
        {
            if (value == _IsActive)
                return;

            if (value)
                _Input.CubesScrolled += ScrollCubes;
            else
                _Input.CubesScrolled -= ScrollCubes;

            _IsActive = value;
        }
    }
    [SerializeField, Min(0)] private float _ScrollSpeed;
    private IInput _Input;
    private CubeSelectorZone _CubeSelectorZone;
    private CubeGenerator _CubeGenerator;
    private bool _IsActive = true;
    private Cube _FirstCube;
    private Cube _LastCube;
    private IReadOnlyCubeCollection _Cubes;
    private float _RightLimit;
    private float _LeftLimit;

    private void Start()
    {
        _CubeSelectorZone = GetComponent<CubeSelectorZone>();
        _CubeGenerator = GetComponent<CubeGenerator>();
        _Cubes = _CubeSelectorZone.Cubes;

        if (_Cubes.HasCubes)
        {
            _FirstCube = _Cubes.FirstCube;
            _LastCube = _Cubes.LastCube;
            
            float width = _CubeSelectorZone.Rect.width;
            float cubeHalfWidth = _FirstCube.Rect.width / 2;
            
            _RightLimit = _CubeSelectorZone.Rect.xMax - cubeHalfWidth - _CubeGenerator.DistanceBetweenCubes;
            _LeftLimit = _CubeSelectorZone.Rect.x + cubeHalfWidth + _CubeGenerator.DistanceBetweenCubes;

            _Input.CubesScrolled += ScrollCubes;
        }
    }

    [Inject]
    private void Initialize(IInput input)
    {
        _Input = input;
    }

    private void ScrollCubes(float scroll)
    {
        if (!_CubeSelectorZone.IsScreenPositionInsideRect(_Input.CursorPosition))
            return;

        scroll *= _CubeSelectorZone.Rect.width;

        if (_LastCube.LocalPositon.x + scroll < _RightLimit || _FirstCube.LocalPositon.x + scroll > _LeftLimit)
            scroll = 0;

        foreach (Cube cube in _Cubes)
        {
            Vector2 position = cube.LocalPositon;
            position.x += scroll;
            cube.LocalPositon = position;
        }    
    }
}
