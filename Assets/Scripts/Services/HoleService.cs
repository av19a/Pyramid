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
        Vector3[] cubeCorners = new Vector3[4];
        Vector3[] holeCorners = new Vector3[4];
    
        cube.GetComponent<RectTransform>().GetWorldCorners(cubeCorners);
        _holeAreaProvider.HoleArea.GetWorldCorners(holeCorners);
    
        Vector2 cubeLeftCorner = RectTransformUtility.WorldToScreenPoint(Camera.main, cubeCorners[0]);
        Vector2 cubeRightCorner = RectTransformUtility.WorldToScreenPoint(Camera.main, cubeCorners[3]);
        Vector2 holeLeftCorner = RectTransformUtility.WorldToScreenPoint(Camera.main, holeCorners[0]);
        Vector2 holeRightCorner = RectTransformUtility.WorldToScreenPoint(Camera.main, holeCorners[3]);
    
        if (cubeLeftCorner.x < holeLeftCorner.x || cubeRightCorner.x > holeRightCorner.x)
        {
            return false;
        }
    
        Vector2 cubeCenter = RectTransformUtility.WorldToScreenPoint(Camera.main, cube.transform.position);
        Vector2 holeCenter = RectTransformUtility.WorldToScreenPoint(Camera.main, _holeAreaProvider.HoleArea.position);

        float a = _holeAreaProvider.HoleArea.rect.width / 2;
        float b = _holeAreaProvider.HoleArea.rect.height / 2;

        float dx = cubeCenter.x - holeCenter.x;
        float dy = cubeCenter.y - holeCenter.y;

        bool isAboveOrInside = cubeCenter.y >= holeCenter.y;
        bool isInside = (dx * dx) / (a * a) + (dy * dy) / (b * b) <= 1;

        if (isInside && isAboveOrInside)
        {
            cube.transform.SetParent(_holeAreaProvider.HoleArea);
            return true;
        }
        return false;
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