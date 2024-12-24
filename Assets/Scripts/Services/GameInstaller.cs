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
    [SerializeField] private Canvas mainCanvas;

    public override void InstallBindings()
    {
        Container.Bind<Canvas>().FromInstance(mainCanvas).AsSingle();
        Container.Bind<ITowerStateSaver>().To<TowerStateSaver>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameState>().AsSingle();
        Container.BindInterfacesAndSelfTo<TowerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<HoleService>().AsSingle();
        Container.BindInterfacesAndSelfTo<MessageService>().AsSingle();
        Container.BindInterfacesAndSelfTo<AnimationService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CubeFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<CubePool>().AsSingle();

        Container.Bind<IGameConfig>().To<GameConfig>().FromScriptableObject(gameConfig).AsSingle();
        Container.Bind<GameObject>().WithId("CubePrefab").FromInstance(cubePrefab).AsSingle();
        Container.Bind<TMP_Text>().WithId("Message").FromInstance(message).AsSingle();
        
        Container.Bind<TowerAreaProvider>().AsSingle().WithArguments(towerArea);
        Container.Bind<HoleAreaProvider>().AsSingle().WithArguments(holeArea);
        
        Container.Bind<ScrollViewController>()
            .FromComponentInNewPrefab(scrollViewPrefab)
            .AsSingle();
    }
}