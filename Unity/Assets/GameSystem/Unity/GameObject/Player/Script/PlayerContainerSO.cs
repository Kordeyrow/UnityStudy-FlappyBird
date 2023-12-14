using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerContainerSO", menuName = "ContainerSO/NewPlayerContainerSO")]
public class PlayerContainerSO : ScriptableObject, IPlayerContainer
{
    [SerializeField]
    [RequireInterface(typeof(IPlayer))]
    UnityEngine.Object playerObj;
    IPlayer player;

    public IPlayer? Player => player;
    public event Action<IPlayer> OnPlayerChanged;

    private void OnEnable()
    {
        if (player != null)
            return;
        player = playerObj as IPlayer;
    }

    public void SetPlayer(IPlayer? newPlayer)
    {
        player = newPlayer;
    }
}
