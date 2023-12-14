using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneReseter : MonoBehaviour
{
    [Inject] readonly IPlayer player;

    void OnEnable()
    {
        player.OnDie += ResetScene;
    }

    void OnDisable()
    {
        player.OnDie -= ResetScene;
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
