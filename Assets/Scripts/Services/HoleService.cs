using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using Zenject;
using Random = UnityEngine.Random;

public interface IHoleService
{
    bool CanDropCube(GameObject cube);
    void DropCube(GameObject cube);
    event Action<GameObject> OnCubeDropped;
}

public class HoleService : IHoleService
{
    private readonly IGameConfig _gameConfig;
    private readonly ICubePool _cubePool;
    private readonly IAnimationService _animationService;
    private readonly HoleAreaProvider _holeAreaProvider;

    public event Action<GameObject> OnCubeDropped;

    [Inject]
    public HoleService(
        ICubePool cubePool,
        IGameConfig gameConfig,
        IAnimationService animationService,
        HoleAreaProvider holeAreaProvider)
    {
        _cubePool = cubePool;
        _gameConfig = gameConfig;
        _animationService = animationService;
        _holeAreaProvider = holeAreaProvider;
    }

    public bool CanDropCube(GameObject cube)
    {
        Vector3[] corners = new Vector3[4];
        cube.GetComponent<RectTransform>().GetWorldCorners(corners);
    
        foreach(Vector3 corner in corners)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(_holeAreaProvider.HoleArea, corner))
                return false;
        }
        
        cube.transform.SetParent(_holeAreaProvider.HoleArea);
        return true;
    }

    public void DropCube(GameObject cube)
    {
        _animationService.PlayHoleDropAnimation(cube, () =>
        {
            OnCubeDropped?.Invoke(cube);
            _cubePool.Return(cube);
        });
    }
}