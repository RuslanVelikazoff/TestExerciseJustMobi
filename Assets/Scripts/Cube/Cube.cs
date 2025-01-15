using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Cube : RectObject
{
    public Color Color
    {
        get => _Image.color;
        set => _Image.color = value;
    }
    public CubeState State => _State;
    public event Action<Cube> Dragged;
    public event Action Dropped;
    public Vector2 ReferenceSize => _ReferenceSize;
    public Color ReferenceColor => _ReferenceColor;
    public DropZone DropZone => _DropZone;
    [SerializeField] private Image _Image;
    [SerializeField, Range(0, 1)] private float _DragScaleFactor;
    [SerializeField, Range(0, 1)] private float _DragAlpha;
    private CubeState _State = CubeState.Idle;
    private DropZone _DropZone;
    private Vector2 _ReferenceSize;
    private Color _ReferenceColor;
    private AnimationPlayer _AnimationPlayer;
    private string _OnDropAnimation = "OnDrop";
    private string _OnDestroyAnimation = "OnDestroy";
    private string _OnThrowAnimation = "OnThrow";
    private string _IdleAnimation = "Idle";

    protected override void Start()
    {
        base.Start();
        _AnimationPlayer = GetComponentInChildren<AnimationPlayer>();
    }

    public void Initialize(DropZone dropZone)
    {
        if (dropZone == null)
            throw new ArgumentNullException(nameof(dropZone));

        _DropZone = dropZone;
    }

    public bool TryDrag(out Cube cube, Transform parent = null)
    {
        cube = null;

        if (!CanDrag())
            return false;

        cube = OnDrag(parent);
        cube._DropZone = null;
        Dragged?.Invoke(cube);
        return true;
    }

    public Cube Drag(Transform parent = null)
    {
        if (!TryDrag(out Cube cube, parent))
            throw new Exception("Can not drag cube");

        return cube;
    }

    public async Task Drop(CubeTowerZone dropZone)
    {
        if (!await TryDrop(dropZone))
            throw new Exception("Can not drop cube");
    }

    public async Task DestroyCube()
    {
        await OnCubeDestroy();
        Destroy(gameObject);
    }

    public async Task<bool> TryDrop(CubeTowerZone dropZone)
    {
        if (dropZone == null)
            return false;

        if (!CanDrop(dropZone))
            return false;

        await OnDrop(dropZone);
        _State = CubeState.Dropped;
        _DropZone = dropZone;
        Dropped?.Invoke();
        return true;
    }

    public virtual Task OnThrowStart(DropHoleZone dropHoleZone)
    {
        SetReferenceState();
        _AnimationPlayer.Play(_OnThrowAnimation);
        return Task.CompletedTask;
    }

    public virtual async Task OnThrowEnd(DropHoleZone dropHoleZone)
    {
        await _AnimationPlayer.PlayAwaitable(_IdleAnimation);
    }

    protected virtual bool CanDrag() => _DropZone is CubeSelectorZone || _DropZone is CubeTowerZone;

    protected virtual bool CanDrop(CubeTowerZone dropZone) => true;

    protected virtual Cube OnDrag(Transform parent)
    {

        bool instantiateNew = _DropZone is CubeSelectorZone;

        if (!instantiateNew)
            transform.SetParent(parent);

        Cube cube = instantiateNew ? Instantiate(this, parent) : this;
        cube._ReferenceColor = Color;
        cube._ReferenceSize = RectTransform.sizeDelta;
        Color color = Color;
        color.a = _DragAlpha;
        cube.Color = color;

        Vector2 size = cube.RectTransform.sizeDelta;
        cube.RectTransform.sizeDelta = size * _DragScaleFactor;

        cube._State = CubeState.Dragged;
        return cube;
    }

    protected virtual async Task OnDrop(CubeTowerZone dropZone)
    {
        SetReferenceState();
        await _AnimationPlayer.PlayAwaitable(_OnDropAnimation);
    }

    protected virtual async Task OnCubeDestroy() => await _AnimationPlayer.PlayAwaitable(_OnDestroyAnimation);

    private void SetReferenceState()
    {
        Color = _ReferenceColor;
        RectTransform.sizeDelta = _ReferenceSize;
    }
}