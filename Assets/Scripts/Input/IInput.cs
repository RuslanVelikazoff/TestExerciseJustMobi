using System;
using UnityEngine;

public interface IInput
{
    public event Action CubeDragged;
    public event Action CubeDropped;
    public event Action<float> CubesScrolled;
    public Vector2 CursorPosition { get; }
    public bool IsActive { get; set; }
}
