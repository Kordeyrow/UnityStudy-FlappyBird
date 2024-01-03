using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneBounds
{
    HashSet<EOuterBounds> OuterBounds(Collider2D other, bool inner = default, Bounds2DOffset offset = default);
    Bounds2D GetCurrentCameraBoxBounds();
}
