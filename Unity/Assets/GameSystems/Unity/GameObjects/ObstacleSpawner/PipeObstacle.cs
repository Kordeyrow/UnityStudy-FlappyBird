using System;
using System.Collections;
using UnityEngine;

public class PipeObstacle : MonoBehaviour, ISpawn
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Movement2DTransform movement;
    [SerializeField] float minXCoord;

    public event Action<ISpawn> OnReadyToPool;

    Coroutine checkingForReadyToDeactivateCo;

    void ISpawn.Activate(Vector3 pos)
    {
        transform.position = pos;
        spriteRenderer.enabled = true;
        movement.StartMoving();
        checkingForReadyToDeactivateCo = StartCoroutine(CheckingForReadyToDeactivate());
    }

    IEnumerator CheckingForReadyToDeactivate()
    {
        while (true)
        {
            if (IsReadyToDeactivate())
            {
                OnReadyToPool.Invoke(this);
                break;
            }
            yield return null;
        }
    }

    bool IsReadyToDeactivate() => transform.position.x < minXCoord;

    void ISpawn.Deactivate()
    {
        transform.position = Vector3.zero;
        spriteRenderer.enabled = false;
        movement.StopMoving();
        if (checkingForReadyToDeactivateCo != null)
            StopCoroutine(checkingForReadyToDeactivateCo);
    }
}
