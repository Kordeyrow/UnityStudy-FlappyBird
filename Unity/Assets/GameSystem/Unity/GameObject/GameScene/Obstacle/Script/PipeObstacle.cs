using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeObstacle : MonoBehaviour, ISpawn, IServiceContainerConsumer<IPlayer>
{
    [SerializeField] int pointsToAddWhenPassByPlayer = 1;
    [SerializeField] float minXCoord;
    [SerializeField] CollisionEventEmitter[] collisionEventEmitters;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Collider2D col2D;
    [SerializeField] Movement2DTransform movement;

    public event Action<ISpawn> OnReadyToBackToPool;
    readonly Dictionary<IEnumerator, Coroutine> coroutineFromIEnumerator = new();
    ServiceContainer<IPlayer?> playerServiceContainer => PlayerServiceContainer.Instance;
    ServiceContainer<ISceneBounds?> sceneBoundsServiceContainer => SceneBoundsServiceContainer.Instance;

    bool flipped;

    void ISpawn.Activate(Vector3 pos, bool isSpawnGroupLeader)
    {
        /// Components setup
        /// 

        transform.position = pos;

        col2D.enabled = true;

        foreach (var s in spriteRenderers)
            s.enabled = true;

        movement.StartMoving();

        foreach (var c in collisionEventEmitters)
            c.OnCollide += OnCollide;

        /// Coroutines setup
        /// 

        SafeStartCoroutine(CheckingForReadyToDeactivate());

        if (isSpawnGroupLeader)
            SafeStartCoroutine(CheckingForPlayerPassedBy());


        /// Services setup
        /// 

        playerServiceContainer.AddConsumer(this);
    }

    void OnPlayerDied()
    {
        SafeStopCoroutine(CheckingForPlayerPassedBy());
    }

    void ISpawn.Deactivate()
    {
        if (flipped)
            UnFlip();

        transform.position = Vector3.zero;

        col2D.enabled = false;
        foreach (var s in spriteRenderers)
            s.enabled = false;

        movement.StopMoving();

        foreach (var c in collisionEventEmitters)
        {
            c.OnCollide -= OnCollide;
        }

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
        if (other.TryGetComponent<IColliderOwnerContainer>(out var IColliderOwnerContainer))
        {
            if (IColliderOwnerContainer.Owner.TryGetComponent<IPlayer>(out var IPlayer))
            {
                IPlayer.Die();
            }
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

    bool IsReadyToDeactivate() => IsOutOfScreenBoundsLeft();

    bool IsOutOfScreenBoundsLeft()
    {
        var sceneBounds = sceneBoundsServiceContainer.GetService();
        if (sceneBounds == null)
            return false;

        return sceneBounds.OuterBounds(col2D).Contains(EOuterBounds.Left);
    }

    public void Flip()
    {
        transform.localScale *= -1;
        flipped = true;
    }

    public void UnFlip()
    {
        transform.localScale *= -1;
        flipped = false;
    }
}
