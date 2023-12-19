
using System;
using UnityEngine;

public interface IPlayer
{
    event Action OnDie;
    event Action<int> OnPointsUpdated;

    void Die();
    void AddPoints(int points);

    Vector3 LeftMostPosition();
}

//public interface IDestroyable
//{
//    event Action OnObjectDestroied;
//}