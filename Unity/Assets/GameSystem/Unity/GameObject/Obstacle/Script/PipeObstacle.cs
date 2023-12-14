using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityLayer;

public class PipeObstacle : MonoBehaviour, ISpawn
{
    [SerializeField] float minXCoord;
    [SerializeField] CollisionEventEmitter collisionEventEmitter;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Movement2DTransform movement;

    public event Action<ISpawn> OnReadyToBackToPool;

    Coroutine checkingForReadyToDeactivateCo;

    void ISpawn.Activate(Vector3 pos)
    {
        collider2D.enabled = true;
        transform.position = pos;
        spriteRenderer.enabled = true;
        movement.StartMoving();
        checkingForReadyToDeactivateCo = StartCoroutine(CheckingForReadyToDeactivate());
        collisionEventEmitter.OnCollide += OnCollide;
    }

    void OnCollide(GameObject other)
    {
        if (other.TryGetComponent<IPlayer>(out var IPlayer))
        {
            IPlayer.Die();
        }
    }

    IEnumerator CheckingForReadyToDeactivate()
    {
        while (true)
        {
            if (IsReadyToDeactivate())
            {
                OnReadyToBackToPool.Invoke(this);
                break;
            }
            yield return null;
        }
    }

    bool IsReadyToDeactivate() => transform.position.x < minXCoord;

    void ISpawn.Deactivate()
    {
        collider2D.enabled = false;
        transform.position = Vector3.zero;
        spriteRenderer.enabled = false;
        movement.StopMoving();
        if (checkingForReadyToDeactivateCo != null)
            StopCoroutine(checkingForReadyToDeactivateCo);
    }
}
