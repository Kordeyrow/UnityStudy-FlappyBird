using System.Collections.Generic;
using UnityEngine;

public class SceneBounds : MonoBehaviour, ISceneBounds
{
    [SerializeField] Camera mainCamera;
    [SerializeField] BoxCollider2D sceneBounds;

    ServiceContainer<ISceneBounds?> sceneBoundsServiceContainer => SceneBoundsServiceContainer.Instance;

    void OnEnable()
    {
        sceneBoundsServiceContainer.SetService(this);
    }

    void OnDisable()
    {
        sceneBoundsServiceContainer.RemoveService(this);
    }

    HashSet<EOuterBounds> ISceneBounds.OuterBounds(Collider2D other, bool inner = false, Bounds2DOffset cameraOffset = default)
    {
        var cameraBounds = ((ISceneBounds)this).GetCurrentCameraBoxBounds();

        HashSet<EOuterBounds> outerBounds = new();

        var otherLeft  = other.bounds.center.x - other.bounds.extents.x;
        var otherRight = other.bounds.center.x + other.bounds.extents.x;
        var otherTop   = other.bounds.center.y + other.bounds.extents.y;
        var otherDown  = other.bounds.center.y - other.bounds.extents.y;

        if (inner)
        {
            otherLeft  += other.bounds.extents.x * 2;
            otherRight += other.bounds.extents.x * -2;
            otherTop   += other.bounds.extents.y * -2;
            otherDown  += other.bounds.extents.y * 2;
        }

        if (otherRight < cameraBounds.Left + cameraOffset.Left)
            outerBounds.Add(EOuterBounds.Left);
        if (otherLeft > cameraBounds.Right + cameraOffset.Right)
            outerBounds.Add(EOuterBounds.Right);
        if (otherDown > cameraBounds.Top + cameraOffset.Top)
            outerBounds.Add(EOuterBounds.Top);
        if (otherTop < cameraBounds.Down + cameraOffset.Down)
            outerBounds.Add(EOuterBounds.Down);

        return outerBounds;
    }

    Bounds2D ISceneBounds.GetCurrentCameraBoxBounds()
    {
        Vector2 lDCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 0f, mainCamera.nearClipPlane));
        Vector2 rUCorner = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, mainCamera.nearClipPlane));
        return new Bounds2D(lDCorner.x, rUCorner.x, rUCorner.y, lDCorner.y);
    }
}
