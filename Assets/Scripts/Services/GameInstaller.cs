using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private ScrollViewController scrollViewPrefab;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallFactories();
        InstallServices();
        InstallViews();
    }

    private void InstallConfigs()
    {
        Container.Bind<IGameConfig>()
            .To<GameConfig>()
            .FromScriptableObject(gameConfig)
            .AsSingle();

        Container.Bind<GameObject>()
            .WithId("CubePrefab")
            .FromInstance(cubePrefab)
            .AsSingle();
    }

    private void InstallFactories()
    {
        Container.Bind<ICubeFactory>()
            .To<CubeFactory>()
            .AsSingle();
    }

    private void InstallServices()
    {
        Container.Bind<IGameState>()
            .To<GameState>()
            .AsSingle();

        Container.Bind<ITowerService>()
            .To<TowerService>()
            .AsSingle();
    }

    private void InstallViews()
    {
        Container.Bind<ScrollViewController>()
            .FromComponentInNewPrefab(scrollViewPrefab)
            .AsSingle();

        Container.Bind<CubeDragController>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}