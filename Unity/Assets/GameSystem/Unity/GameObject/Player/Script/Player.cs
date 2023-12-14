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

    public event Action OnDie;

    private void Awake()
    {
        jump2D = GetComponent<IJump2D>();
    }

    void Start()
    {
        jump2D?.Execute();
    }

    void IPlayer.Die()
    {
        StopMovement();
        Freeze();
        OnDie?.Invoke();
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
