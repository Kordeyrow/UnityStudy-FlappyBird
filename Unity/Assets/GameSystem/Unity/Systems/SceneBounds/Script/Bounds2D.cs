using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Bounds2D
{
    public float Left { get; }
    public float Right { get; }
    public float Top { get; }
    public float Down { get; }
    public Vector2 Center { get; }

    public Bounds2D(float left, float right, float top, float down)
    {
        Left = left;
        Right = right;
        Top = top;
        Down = down;
        Center = new Vector2((right - left) / 2, (top - down) / 2);
    }
}
