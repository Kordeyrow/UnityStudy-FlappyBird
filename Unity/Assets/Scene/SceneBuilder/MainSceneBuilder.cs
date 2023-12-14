using UnityEngine;
using Zenject;

public class MainSceneBuilder : MonoInstaller
{
    [Header("> GameObjects")]
    [RequireInterface(typeof(IPlayer))]
    [SerializeField] Object playerObj;
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
