using UnityEngine;
using Zenject;

public class GameSceneBuilder : MonoInstaller
{
    [Header("> GameObjects")]
    [RequireInterface(typeof(IPlayer))]
    [SerializeField] Object playerObj;
    [SerializeField] GameObject obstacle;
    [SerializeField] GameObject mainCamera;

    public override void InstallBindings()
    {
        /// __ COMPONENTS __ ///

        // IJumpInput
        Container.Bind<IJumpInput>()
            .To<InputManager>()
            .AsSingle();


        /// __ GAMEOBJECTS __ /// 

        // Player
        var playerInstance = Container.InstantiatePrefab(playerObj);
        var player = playerInstance.GetComponent<Player>();
        Container.Bind<IPlayer>()
            .FromInstance(player)
            .NonLazy();

        // Camera
        Container.InstantiatePrefab(mainCamera);
    }
}

public interface ISpawnFactory
{
    GameObject Create();
}

public class SpawnFactory : ISpawnFactory
{
    readonly DiContainer container;
    readonly GameObject prefab;

    public SpawnFactory(DiContainer container, GameObject prefab)
    {
        this.container = container;
        this.prefab = prefab;
    }

    public GameObject Create()
    {
        return container.InstantiatePrefab(prefab);
    }
}
