using System;
using UnityEngine;
using UnityLayer;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump2DRB : MonoBehaviour, IJump2D
{
    [SerializeField] float force;
    Rigidbody2D rb;
    IJumpInput jumpInput;
    bool activated = true;
    public event Action OnExecute;

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
        if (activated == false)
            return;
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * force);
        OnExecute.Invoke();
    }

    public void Deactivate()
    {
        activated = false;
    }

    private void OnDisable()
    {
        jumpInput.JumpEvent -= ((IJump2D)this).Execute;
    }
}
