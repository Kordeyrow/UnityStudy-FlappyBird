using System;
using System.Collections;
using UnityEngine;
using UnityLayer;

public class Player : MonoBehaviour, IPlayer
{
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Jump2DRB jump;
    [SerializeField] Animator animator;
    IJump2D jump2D;
    
    ServiceContainer<IPlayer?> playerService => PlayerServiceContainer.Instance;
    ServiceContainer<ISceneBounds?> sceneBounds => SceneBoundsServiceContainer.Instance;

    public event Action OnDie;
    public event Action<int> OnPointsUpdated;
    int currentPoints;
    bool dead;

    private void Awake()
    {
        jump2D = GetComponent<IJump2D>();
        jump2D.OnExecute += PlayJumpAnimation;
    }

    private void OnEnable()
    {
        playerService.SetService(this);
    }

    private void OnDisable()
    {
        playerService.RemoveService(this);
    }

    void Start()
    {
        jump2D?.Execute();
        SetPoints(0);
    }

    void PlayJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    private void Update()
    {
        if (dead) 
            return;

        KeepBelowScreenTopBound();
        CheckDiedByFall();
    }

    bool IsOutOfScreenBoundsTop()
    {
        return sceneBounds.GetService()?.OuterBounds(collider2D, true).Contains(EOuterBounds.Top) == true;
    }

    void CheckDiedByFall()
    {
        if (sceneBounds.GetService()?.OuterBounds(collider2D).Contains(EOuterBounds.Down) == true)
        {
            ((IPlayer)this).Die();
        }
    }


    void KeepBelowScreenTopBound()
    {
        if (IsOutOfScreenBoundsTop())
        {
            transform.transform.position = new Vector3(0, ScreenTop() - (collider2D.bounds.extents.y), 0);
        }
    }

    float ScreenTop()
    {
        var sceneBoundsService = sceneBounds.GetService();
        if (sceneBoundsService == null)
            return 0f;

        return sceneBoundsService.GetCurrentCameraBoxBounds().Top;
    }

    void IPlayer.Die()
    {
        StopMovement();
        Freeze();
        OnDie?.Invoke();
        dead = true;
    }

    void IPlayer.AddPoints(int points)
    {
        if (points <= 0)
            return;

        SetPoints(currentPoints + points);
    }

    void SetPoints(int points)
    {
        currentPoints = points;
        OnPointsUpdated.Invoke(currentPoints);
    }

    Vector3 IPlayer.LeftMostPosition()
    {
        return transform.position + new Vector3(- transform.localScale.x / 2, 0, 0);
    }

    void StopMovement()
    {
        jump.Deactivate();
    }

    void Freeze()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.velocity = Vector2.zero;
    }
}
