using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

public class PipeObstacle : MonoBehaviour, ISpawn, IServiceContainerConsumer<IPlayer>
{
    [SerializeField] int pointsToAddWhenPassByPlayer = 1;
    [SerializeField] float minXCoord;
    [SerializeField] CollisionEventEmitter collisionEventEmitter;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Movement2DTransform movement;

    public event Action<ISpawn> OnReadyToBackToPool;

    readonly Dictionary<IEnumerator, Coroutine> coroutineFromIEnumerator = new();

    ServiceContainer<IPlayer?> playerServiceContainer => PlayerServiceContainer.Instance;

    void ISpawn.Activate(Vector3 pos, bool isSpawnGroupLeader)
    {
        /// Components setup
        /// 

        transform.position = pos;

        collider2D.enabled = true;
        spriteRenderer.enabled = true;

        movement.StartMoving();

        collisionEventEmitter.OnCollide += OnCollide;


        /// Coroutines setup

        SafeStartCoroutine(CheckingForReadyToDeactivate());

        if (isSpawnGroupLeader)
            SafeStartCoroutine(CheckingForPlayerPassedBy());


        /// Services setup

        playerServiceContainer.AddConsumer(this);
    }

    void OnPlayerDied()
    {
        SafeStopCoroutine(CheckingForPlayerPassedBy());
    }

    void ISpawn.Deactivate()
    {
        transform.position = Vector3.zero;

        collider2D.enabled = false;
        spriteRenderer.enabled = false;

        movement.StopMoving();

        collisionEventEmitter.OnCollide -= OnCollide;

        SafeStopCoroutine(CheckingForReadyToDeactivate());
        SafeStopCoroutine(CheckingForPlayerPassedBy());

        playerServiceContainer.RemoveConsumer(this);
    }

    void IServiceContainerConsumer<IPlayer>.OnServiceUpdated(IPlayer oldService, IPlayer newPlayer)
    {
        if (oldService != null)
            oldService.OnDie -= OnPlayerDied;

        if (newPlayer != null)
            newPlayer.OnDie += OnPlayerDied;
    }

    void SafeStopCoroutine(IEnumerator ie)
    {
        if (coroutineFromIEnumerator.ContainsKey(ie) && coroutineFromIEnumerator[ie] != null)
            StopCoroutine(coroutineFromIEnumerator[ie]);
    }

    void SafeStartCoroutine(IEnumerator ie)
    {
        SafeStopCoroutine(ie);
        var coroutine = StartCoroutine(ie);
        coroutineFromIEnumerator[ie] = coroutine;
    }

    void OnCollide(GameObject other)
    {
        if (other.TryGetComponent<IPlayer>(out var IPlayer))
        {
            IPlayer.Die();
        }
    }

    IEnumerator CheckingForPlayerPassedBy()
    {
        while (true)
        {
            if (HasPlayerPassedBy())
            {
                OnPlayerPassedBy();
                break;
            }
            yield return null;
        }
    }

    void OnPlayerPassedBy()
    {
        AddPointToPlayer();
    }

    bool HasPlayerPassedBy()
    {
        IPlayer player = playerServiceContainer.GetService();
        if (player == null)
            return false;
        return RightMostPosition().x < player.LeftMostPosition().x;
    }

    Vector3 RightMostPosition()
    {
        return transform.position + new Vector3(transform.localScale.x / 2, 0, 0);
    }

    void AddPointToPlayer()
    {
        IPlayer player = playerServiceContainer.GetService();
        if (player == null)
            return;
        player.AddPoints(pointsToAddWhenPassByPlayer);
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

}
