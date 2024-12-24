using DG.Tweening;
using Zenject;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private ScrollViewController scrollViewPrefab;
    [SerializeField] private RectTransform towerArea;
    [SerializeField] private RectTransform holeArea;
    [SerializeField] private TMP_Text message;

    public override void InstallBindings()
    {
        DOTween.SetTweensCapacity(500, 50);
        
        InstallConfigs();
        InstallFactories();
        InstallServices();
        InstallViews();
        InstallProviders();
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
        
        Container.Bind<TMP_Text>()
            .WithId("Message")
            .FromInstance(message)
            .AsSingle();
    }

    private void InstallFactories()
    {
        Container.Bind<ICubePool>()
            .To<CubePool>()
            .AsSingle();
        
        Container.Bind<ICubeFactory>()
            .To<CubeFactory>()
            .AsSingle();
    }

    private void InstallServices()
    {
        Container.Bind<IGameStateService>()
            .To<GameStateService>()
            .AsSingle();
        
        Container.Bind<IGameState>()
            .To<GameState>()
            .AsSingle();

        Container.Bind<ITowerService>()
            .To<TowerService>()
            .AsSingle();
        
        Container.Bind<IHoleService>()
            .To<HoleService>()
            .AsSingle();

        Container.Bind<IMessageService>()
            .To<MessageService>()
            .AsSingle();
        
        Container.Bind<IAnimationService>()
            .To<AnimationService>()
            .AsSingle();
    }
    
    private void InstallProviders()
    {
        Container.Bind<TowerAreaProvider>()
            .AsSingle()
            .WithArguments(towerArea);
        
        Container.Bind<HoleAreaProvider>()
            .AsSingle()
            .WithArguments(holeArea);
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