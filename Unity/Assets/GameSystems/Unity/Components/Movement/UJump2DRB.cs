using UnityEngine;
using UnityLayer;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class UJump2DRB : MonoBehaviour, IJump2D
{
    [SerializeField] float force;
    Rigidbody2D rb;

    [Inject]
    public void Construct(IJumpInput jumpInput)
    {
        jumpInput.JumpEvent += ((IJump2D)this).Execute;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void IJump2D.Execute()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * force);
    }
}
