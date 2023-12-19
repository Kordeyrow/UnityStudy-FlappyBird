using System;
using UnityEngine;

public interface ISpawn
{
    void Activate(Vector3 pos, bool isSpawnGroupLeader);
    void Deactivate();
    event Action<ISpawn> OnReadyToBackToPool;
}
