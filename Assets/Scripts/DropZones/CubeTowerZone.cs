using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CubeTowerZone : DropZone
{
    public float MaxRandomOffset 
    {
        get => _MaxRandomOffset;
        set
        {
            if (value < 0)
                throw new ArgumentException(nameof(value), "Value must be bigger or equals than 0");
            _MaxRandomOffset = value;
        }
    }
    [SerializeField] private float _MaxRandomOffset;
    [SerializeField, Min(0)] private float _MoveDownSpeed;
    [SerializeField] private Cube _CubePrefab;
    private bool _IsMovingDown = false;
    private ISessionData _SessionData;

    protected override void Start()
    {
        base.Start();
        Cubes.CubeRemoved += OnCubeRemoved;
    }
    
    protected override async Task<bool> TryDropInternal(Cube cube)
    {
        if (_IsMovingDown)
            return false;

        Transform parent = cube.transform;
        cube.transform.SetParent(transform);

        if (!IsTowerHeightValid())
        {
            NotificationSender.ShowNotification(LanguageProvider["HeightLimit"]);
            SetDefaultParent();
            return false;
        }

        if (!IsCubePositionValid(cube, out Vector2 position))
        {
            SetDefaultParent();
            return false;
        }

        cube.AnchoredPosition = position;

        if (!await cube.TryDrop(this))
        {
            SetDefaultParent();
            return false;
        }

        NotificationSender.ShowNotification(LanguageProvider["CubeDropped"]);
        return true;

        void SetDefaultParent() => cube.transform.SetParent(parent);
    }

    [Inject]
    private void Initialzie(ISessionData sessionData)
    {
        _SessionData = sessionData;
        LoadCubes();
        _SessionData.SaveStart += SaveCubes;
    }

    private void LoadCubes()
    {
        List<CubeInTowerData> cubes = _SessionData.CubesInTower;
        int cubesCount = cubes.Count;

        if (cubesCount == 0)
            return;

        CubeInTowerData firstCube = cubes[0];
        Vector2 position = _SessionData.FirstCubeInTowerPosition;
        CreateCube(position, firstCube.Color);

        for (int i = 1; i < cubes.Count; i++)
        {
            CubeInTowerData cube = cubes[i];
            position.x += cube.Offset;
            position.y += _CubePrefab.Height;
            CreateCube(position, cube.Color);
        }

        void CreateCube(Vector2 localPosition, Color color)
        {
            Cube cube = Instantiate(_CubePrefab, transform);
            cube.Initialize(this);
            cube.LocalPositon = localPosition;
            cube.Color = color;
            AddCube(cube, true);
        }
    }

    private void SaveCubes()
    {
        if (!Cubes.HasCubes)
        {
            _SessionData.CubesInTower.Clear();
            _SessionData.FirstCubeInTowerPosition = Vector2.zero;
            return;
        }
        
        Cube firstCube = Cubes.FirstCube;
        _SessionData.FirstCubeInTowerPosition = firstCube.LocalPositon;
        List<CubeInTowerData> cubesInTower = new List<CubeInTowerData>
        {
            new CubeInTowerData(firstCube.Color, 0)
        };
        
        Cube previousCube = firstCube;

        for (int i = 1; i < Cubes.Count; i++)
        {
            Cube cube = Cubes[i];
            Color color = cube.Color;
            float offset = cube.LocalPositon.x - previousCube.LocalPositon.x;

            cubesInTower.Add(new CubeInTowerData(color, offset));
            previousCube = cube;
        }

        _SessionData.CubesInTower.Clear();
        _SessionData.CubesInTower.AddRange(cubesInTower);
    }

    private bool IsTowerHeightValid()
    {
        if (!Cubes.HasCubes)
            return true;
        
        if (Cubes.LastCube.Top + Cubes.LastCube.Height > Rect.yMax)
            return false;

        return true;
    }

    private void OnCubeRemoved(RemovedCubeData data)
    {
        if (data.NewCube == null)
            return;

        StartCoroutine(MoveCubesDown(data));

    }

    private IEnumerator MoveCubesDown(RemovedCubeData data)
    {
        _IsMovingDown = true;

        int index = data.Index;
        Cube previousCube = data.PreviousCube;
        Cube newCube = data.NewCube;
        Cube firstCube = Cubes.FirstCube;
        float targetY = previousCube == null ? firstCube.LocalPositon.y - firstCube.Height : previousCube.LocalPositon.y;

        if (previousCube != null)
            targetY += newCube.Height;

        float currentY = newCube.AnchoredPosition.y;

        while (currentY > targetY)
        {
            MoveCubesDownOnce(index, currentY);
            currentY -= _MoveDownSpeed * Time.deltaTime;
            yield return null;
        }

        MoveCubesDownOnce(index, targetY);

        _IsMovingDown = false;
    }

    private void MoveCubesDownOnce(int startIndex, float targetY)
    {
        Cube previousCube = Cubes[startIndex];
        Vector2 startPosition = previousCube.AnchoredPosition;
        startPosition.y = targetY;
        previousCube.AnchoredPosition = startPosition;

        if (startIndex == Cubes.Count - 1)
            return;

        for (int i = startIndex + 1; i < Cubes.Count; i++)
        {
            Cube cube = Cubes[i];
            Vector2 position = cube.AnchoredPosition;
            position.y = previousCube.AnchoredPosition.y + cube.Height;
            cube.AnchoredPosition = position;
            previousCube = cube;
        }
    }

    private bool IsCubePositionValid(Cube cube, out Vector2 position)
    {
        position = cube.RectTransform.anchoredPosition;

        Rect rect = RectTransform.rect;

        float halfWidth = cube.ReferenceSize.x / 2;
        float halfHeight = cube.ReferenceSize.y / 2;

        float minX = rect.x + halfWidth;
        float maxX = rect.x + rect.width - halfWidth;
        float minY = rect.y + halfHeight;
        float maxY = rect.y + rect.height - halfHeight;

        Vector2 min = new Vector2(minX, minY);
        Vector2 max = new Vector2(maxX, maxY);

        if (!Cubes.HasCubes)
        {
            position = Clamp(position, min, max);
            return true;
        }

        Cube lastCube = Cubes.LastCube;

        float maxOffset = Mathf.Min(_MaxRandomOffset, halfWidth);
        float offset = UnityEngine.Random.Range(-maxOffset, maxOffset);

        if (cube.Bottom < lastCube.Top)
            return false;

        Vector2 lastCubePosition = lastCube.AnchoredPosition;
        position = lastCubePosition + new Vector2(offset, cube.ReferenceSize.y);
        position.x = Mathf.Clamp(position.x, minX, maxX);

        return true;
    }

    private Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => 
        new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
}


