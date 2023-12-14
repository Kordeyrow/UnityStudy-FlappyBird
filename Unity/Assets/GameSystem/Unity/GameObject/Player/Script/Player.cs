using System;
using UnityEngine;
using UnityLayer;

public class Player : MonoBehaviour, IPlayer
{
    [SerializeField]
    [RequireInterface(typeof(IPlayerContainer))]
    UnityEngine.Object playerContainerObj;
    IPlayerContainer playerContainer;
    IPlayer thisPlayer;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Jump2DRB jump;
    IJump2D jump2D;

    public event Action OnDie;

    private void Awake()
    {
        jump2D = GetComponent<IJump2D>();
        thisPlayer = this;
        playerContainer = playerContainerObj as IPlayerContainer;
    }

    void Start()
    {
        jump2D?.Execute();
        playerContainer?.SetPlayer(thisPlayer);
    }

    void IPlayer.Die()
    {
        OnDie?.Invoke();
        StopMovement();
        Freeze();
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
