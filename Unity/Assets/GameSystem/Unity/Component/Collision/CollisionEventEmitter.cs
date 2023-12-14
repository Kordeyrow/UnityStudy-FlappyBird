using System;
using UnityEngine;

public class CollisionEventEmitter : MonoBehaviour
{
    public event Action<GameObject> OnCollide;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollide?.Invoke(collision.gameObject);
    }
}
