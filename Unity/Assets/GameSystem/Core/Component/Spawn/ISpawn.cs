using System;
using UnityEngine;

public interface ISpawn
{
    void Activate(Vector3 pos);
    void Deactivate();
    event Action<ISpawn> OnReadyToBackToPool;
}
