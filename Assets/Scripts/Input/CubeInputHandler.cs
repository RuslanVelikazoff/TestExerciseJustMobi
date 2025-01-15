using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class CubeInputHandler : MonoBehaviour
{
    [SerializeField] private Canvas _Canvas;
    private Cube _Cube;
    private IInput _Input;
    private IEnumerator _CursorFollowRoutine;
    private RectTransform _CanvasRect;
    private DropZone[] _DropZones;
    private CubeSelectorScroller _CubeSelectorScroller;
    private bool _IsInputActive = true;
    private INotificationSender _NotificationSender;
    private ILanguageProvider _LanguageProvider;

    private void Start()
    {
        _CanvasRect = (RectTransform)_Canvas.transform;
        _DropZones = FindObjectsOfType<DropZone>();
        _CubeSelectorScroller = GetComponent<CubeSelectorScroller>();
    }

    [Inject]
    private void Initialize(IInput input, INotificationSender notificationSender, ILanguageProvider languageProvider)
    {
        _Input = input;
        _NotificationSender = notificationSender;
        _LanguageProvider = languageProvider;

        _Input.CubeDragged += OnCubeDragged;
        _Input.CubeDropped += OnCubeDropped;
    }

    private void OnCubeDragged()
    {
        if (!_IsInputActive)
            return;

        Cube cube = null;

        foreach (DropZone dropZone in _DropZones)
        {
            cube = dropZone.Cubes.FirstOrDefault(x => x.IsScreenPositionInsideRect(_Input.CursorPosition));

            if (cube != null)
                break;
        }

        if (cube == null)
            return;

        if (!cube.TryDrag(out _Cube, _Canvas.transform))
            return;

        _CursorFollowRoutine = FollowCursor();
        StartCoroutine(_CursorFollowRoutine);
        _CubeSelectorScroller.IsActive = false;
    }

    private async void OnCubeDropped()
    {
        if (!_IsInputActive)
            return;

        if (_Cube == null)
            return;

        _IsInputActive = false;

        DropZone dropZone = _DropZones.FirstOrDefault(x => x.IsInsideRect(_Cube));

        StopCoroutine(_CursorFollowRoutine);

        if (dropZone == null || !await dropZone.TryDrop(_Cube))
        {
            await _Cube.DestroyCube();
            _NotificationSender.ShowNotification(_LanguageProvider["CubeDestroyed"]);
        }
        
        _Cube = null;
        _IsInputActive = true;
        _CubeSelectorScroller.IsActive = true;
    }

    private IEnumerator FollowCursor()
    {
        while (true)
        {
            _Cube.RectTransform.anchoredPosition = ScreenToRectPosition(_Input.CursorPosition);
            yield return null;
        }
    }

    private Vector2 ScreenToRectPosition(Vector2 screenPosition)
    {
        screenPosition.x /= Screen.width;
        screenPosition.y /= Screen.height;

        screenPosition *= 2;
        screenPosition.x -= 1;
        screenPosition.y -= 1;

        screenPosition.x *= _CanvasRect.sizeDelta.x / 2;
        screenPosition.y *= _CanvasRect.sizeDelta.y / 2;

        return screenPosition;
    }
}
