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
    private IGameConfig _gameConfig;
    private ICubePool _cubePool;
    
    private HoleAreaProvider _holeAreaProvider;

    public event Action<GameObject> OnCubeDropped;

    [Inject]
    public void Construct(
        ICubePool cubePool,
        IGameConfig gameConfig,
        HoleAreaProvider holeAreaProvider)
    {
        _cubePool = cubePool;
        _gameConfig = gameConfig;
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
        return true;
    }

    public void DropCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        var startPosition = rectTransform.anchoredPosition;
        var endPosition = startPosition + Vector2.down * (_gameConfig.CubeSize * 3);

        rectTransform.DOAnchorPos(endPosition, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                OnCubeDropped?.Invoke(cube);
                _cubePool.Return(cube);
            });
    }
}