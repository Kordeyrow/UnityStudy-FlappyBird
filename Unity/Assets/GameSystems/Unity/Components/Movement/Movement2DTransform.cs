using System.Collections;
using UnityEngine;

public class Movement2DTransform : MonoBehaviour
{
    [SerializeField] float speedX;
    Coroutine movingCo;

    void Start()
    {
        StartMoving();
    }

    IEnumerator Moving()
    {
        while (true)
        {
            transform.position += new Vector3(speedX * Time.deltaTime, 0, 0);
            yield return null;
        }
    }

    public void StartMoving()
    {
        StopMoving();
        movingCo = StartCoroutine(Moving());
    }

    public void StopMoving()
    {
        if (movingCo != null)
            StopCoroutine(movingCo);
    }
}
