using System;
using System.Threading.Tasks;
using Zenject;

public abstract class DropZone : RectObject
{
    public IReadOnlyCubeCollection Cubes => _Cubes;
    protected virtual bool AddCubeOnDrop { get; } = true;
    protected INotificationSender NotificationSender => _NotificationSender;
    protected ILanguageProvider LanguageProvider => _LanguageProvider;
    private INotificationSender _NotificationSender;
    private ILanguageProvider _LanguageProvider;
    private CubeCollection _Cubes = new CubeCollection();

    public async Task<bool> TryDrop(Cube cube)
    {
        if (!await TryDropInternal(cube))
            return false;

        if (!AddCubeOnDrop)
            return true;

        AddCube(cube, true);
        return true;
    }

    protected void AddCube(Cube cube, bool removeOnDragged = false)
    {
        _Cubes.Add(cube);

        if (!removeOnDragged)
            return;

        Action<Cube> onCubeDragged = null;
        onCubeDragged = (newCube) =>
        {
            _Cubes.Remove(cube);
            cube.Dragged -= onCubeDragged;
        };
        cube.Dragged += onCubeDragged;
    }

    protected abstract Task<bool> TryDropInternal(Cube cube);

    [Inject]
    private void Initialize(INotificationSender notificationSender, ILanguageProvider languageProvider)
    {
        _NotificationSender = notificationSender;
        _LanguageProvider = languageProvider;
    }
}
