using System.Threading.Tasks;
using UnityEngine;

public class CubeSelectorZone : DropZone
{
    protected override bool AddCubeOnDrop => false;

    protected override async Task<bool> TryDropInternal(Cube cube)
    {
        await cube.DestroyCube();
        NotificationSender.ShowNotification(LanguageProvider["CubeDestroyed"]);
        return true;
    }

    public Cube CreateCube(Cube prefab, Transform parent = null)
    {
        Cube cube = Instantiate(prefab, parent);
        cube.Initialize(this);
        AddCube(cube);
        return cube;
    }
}
