using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    // This field must be assigned in Unity Inspector
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private ScrollViewController scrollViewPrefab;

    public override void InstallBindings()
    {
        // The most important part - binding GameConfig to IGameConfig interface
        // Make sure this binding happens BEFORE any other bindings that depend on IGameConfig
        Container.Bind<IGameConfig>()
            .To<GameConfig>()
            .FromScriptableObject(gameConfig)
            .AsSingle();

        // Bind the cube prefab
        Container.Bind<GameObject>()
            .WithId("CubePrefab")
            .FromInstance(cubePrefab)
            .AsSingle();

        // Bind all other services
        // Container.Bind<IGameStateService>()
        //     .To<GameStateService>()
        //     .AsSingle();
        
        Container.Bind<ITowerService>()
        .To<TowerService>()
        .AsSingle();
        
        // Container.Bind<IMessageService>()
        //     .To<MessageService>()
        //     .AsSingle();

        // Bind ScrollViewController
        Container.Bind<ScrollViewController>()
            .FromComponentInNewPrefab(scrollViewPrefab)
            .AsSingle();
    }
}