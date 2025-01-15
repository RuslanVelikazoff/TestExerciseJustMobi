using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour, IInput
{
    public bool IsActive
    {
        get => _IsActive;
        set => _IsActive = value;
    }
    public Vector2 CursorPosition => Input.GetTouch(0).position;
    public event Action CubeDragged;
    public event Action CubeDropped;
    public event Action<float> CubesScrolled;
    private bool _IsActive = true;
    private Vector2 _CursorPosition;
    
    void Update()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            CubeDragged?.Invoke();

        if (touch.phase == TouchPhase.Ended)
            CubeDropped?.Invoke();

        if (IsCubeScrolled(touch, out float scroll))
            CubesScrolled?.Invoke(scroll);

        _CursorPosition = CursorPosition;
    }

    private bool IsCubeScrolled(Touch touch, out float scroll)
    {
        scroll = 0;

        if (touch.phase != TouchPhase.Moved)
            return false;

        scroll = touch.position.x - _CursorPosition.x;
        scroll /= Screen.width;

        return true;
    }
}
