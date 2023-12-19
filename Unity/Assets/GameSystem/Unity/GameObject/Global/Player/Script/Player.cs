using System;
using UnityEngine;
using UnityLayer;

public class Player : MonoBehaviour, IPlayer
{
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Jump2DRB jump;
    IJump2D jump2D;
    
    Service<IPlayer?> IPlayerService => PlayerService.Instance;

    public event Action OnDie;
    public event Action<int> OnPointsUpdated;
    private int currentPoints;

    public event Action OnObjectDestroied;

    private void Awake()
    {
        jump2D = GetComponent<IJump2D>();
        IPlayerService.UpdateService(this);
    }

    void Start()
    {
        jump2D?.Execute();
        SetPoints(0);
    }

    void IPlayer.Die()
    {
        StopMovement();
        Freeze();
        OnDie?.Invoke();
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
