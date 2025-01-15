using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DropHoleZone : DropZone
{
    protected override bool AddCubeOnDrop => false;
    [SerializeField] private RectObject _HoleRect;
    [SerializeField, Min(0)] private float _FallSpeed;
    private bool _Falling = false;
    private int _Delay = 10;

    protected override async Task<bool> TryDropInternal(Cube cube)
    {
        Transform parent = cube.transform.parent;
        cube.transform.SetParent(transform);

        await cube.OnThrowStart(this);

        if (!IsCubeHigherThanHole(cube))
        {
            await DropCube(false);
            return true;
        }

        await MoveCubeDownAwaitable(cube);

        if (!IsCubeAboveHole(cube))
        {
            await DropCube(false);
            return true;
        }
        
        await DropCube(true);

        return true;

        async Task DropCube(bool successfully)
        {
            cube.transform.SetParent(parent);
            await cube.OnThrowEnd(this);
            await cube.DestroyCube();

            if (successfully)
            {
                NotificationSender.ShowNotification(LanguageProvider["CubeThrown"]);
                return;
            }

            NotificationSender.ShowNotification(LanguageProvider["DropHoleMiss"]);
        }
    }

    private async Task MoveCubeDownAwaitable(Cube cube)
    {
        if (!IsCubeHigherThanHole(cube))
            return;

        StartCoroutine(MoveCubeDown(cube));

        while (_Falling)
            await Task.Delay(_Delay);
    }

    private IEnumerator MoveCubeDown(Cube cube)
    {
        _Falling = true;
        while (IsCubeHigherThanHole(cube))
        {
            Vector2 position = cube.LocalPositon;
            position.y -= _FallSpeed * Time.deltaTime;
            cube.LocalPositon = position;
            yield return null;
        }

        cube.LocalPositon = new Vector2(cube.LocalPositon.x, _HoleRect.LocalPositon.y);

        _Falling = false;
    }

    private bool IsCubeHigherThanHole(Cube cube) => cube.LocalPositon.y > _HoleRect.LocalPositon.y;

    private bool IsCubeAboveHole(Cube cube) => cube.LocalPositon.x > _HoleRect.Rect.x && cube.LocalPositon.x < _HoleRect.Rect.xMax;
}
