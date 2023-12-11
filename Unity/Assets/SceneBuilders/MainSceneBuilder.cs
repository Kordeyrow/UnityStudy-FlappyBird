using UnityEngine;
using Zenject;

public class MainSceneBuilder : MonoInstaller
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject player;

    public override void InstallBindings()
    {
        // Components
        Container.Bind<IJumpInput>()
            .To<InputManager>()
            .AsSingle();

        // GameObjects
        Container.InstantiatePrefab(player);
        Container.InstantiatePrefab(mainCamera);
    }
}
