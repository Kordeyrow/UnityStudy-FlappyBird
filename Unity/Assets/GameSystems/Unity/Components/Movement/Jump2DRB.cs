using UnityEngine;
using UnityLayer;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump2DRB : MonoBehaviour, IJump2D
{
    [SerializeField] float force;
    Rigidbody2D rb;
    IJumpInput jumpInput;

    [Inject]
    public void Construct(IJumpInput jumpInput)
    {
        this.jumpInput = jumpInput;
        this.jumpInput.JumpEvent += ((IJump2D)this).Execute;
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

    private void OnDestroy()
    {
        jumpInput.JumpEvent -= ((IJump2D)this).Execute;
    }
}
