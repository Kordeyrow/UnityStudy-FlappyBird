using UnityEngine;
using UnityLayer;

public class Player : MonoBehaviour
{
    IJump2D jump2D;

    private void Awake()
    {
        jump2D = GetComponent<IJump2D>();
    }

    void Start()
    {
        jump2D?.Execute();
    }
}
