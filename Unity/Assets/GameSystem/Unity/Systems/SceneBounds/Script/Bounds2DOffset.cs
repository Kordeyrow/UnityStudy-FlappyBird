using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Bounds2DOffset
{
    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Down { get; }

    public Bounds2DOffset(float left, float right, float top, float down)
    {
        Left = left;
        Right = right;
        Top = top;
        Down = down;
    }
}
