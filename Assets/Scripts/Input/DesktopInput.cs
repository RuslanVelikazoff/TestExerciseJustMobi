using System;
using UnityEngine;
using Zenject;

public class DesktopInput : MonoBehaviour, IInput
{
    public Vector2 CursorPosition => Input.mousePosition;
    public bool IsActive 
    {
        get => _IsActive;
        set => _IsActive = value; 
    }
    public event Action CubeDragged;
    public event Action CubeDropped;
    public event Action<float> CubesScrolled;
    private bool _IsMouseButtonDown = false;
    private bool _IsActive = true;
    private Vector2 _CursorPosition; 

    private void Start()
    {
        CubeDragged += () => _IsMouseButtonDown = true;
        CubeDropped += () => _IsMouseButtonDown = false;
    }

    private void Update()
    {
        if (!_IsActive)
            return;

        if (Input.GetMouseButtonDown(0))
            CubeDragged?.Invoke();

        if (Input.GetMouseButtonUp(0))
            CubeDropped?.Invoke();

        if (IsCubeScrolledByMouse(out float scroll))
            CubesScrolled?.Invoke(scroll);

        _CursorPosition = CursorPosition;
    }

    private bool IsCubeScrolledByMouse(out float scroll)
    {
        scroll = Input.mousePosition.x - _CursorPosition.x;
        scroll /= Screen.width;
        
        if (!_IsMouseButtonDown || scroll == 0)
            return false;
        
        return true;
    }
}