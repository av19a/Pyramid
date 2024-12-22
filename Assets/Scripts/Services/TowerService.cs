using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface ITowerService
{
    bool CanAddCube(GameObject cube);
    void AddCube(GameObject cube);
    void RemoveCube(GameObject cube);
    int GetCurrentHeight();
    event Action<GameObject> OnCubeAdded;
    event Action<GameObject> OnCubeRemoved;
}

public class TowerService : ITowerService
{
    private IGameConfig _gameConfig;
    private IGameState _gameState;
    
    private TowerAreaProvider _towerAreaProvider;

    public event Action<GameObject> OnCubeAdded;
    public event Action<GameObject> OnCubeRemoved;

    [Inject]
    public void Construct(
        IGameConfig gameConfig,
        IGameState gameState,
        TowerAreaProvider towerAreaProvider)
    {
        _gameConfig = gameConfig;
        _gameState = gameState;
        _towerAreaProvider = towerAreaProvider;
    }

    public bool CanAddCube(GameObject cube)
    {
        if (_gameState.TowerCubes.Count == 0)
        {
            Vector3[] corners = new Vector3[4];
            cube.GetComponent<RectTransform>().GetWorldCorners(corners);
    
            foreach(Vector3 corner in corners)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(_towerAreaProvider.TowerArea, corner))
                    return false;
            }
            return true;
        }

        var currentCubeRect = cube.GetComponent<RectTransform>();
        var previousCubeRect = _gameState.TowerCubes[^1].GetComponent<RectTransform>();

        float horizontalDistance = Mathf.Abs(currentCubeRect.anchoredPosition.x - previousCubeRect.anchoredPosition.x);
        float verticalDistance = currentCubeRect.anchoredPosition.y - previousCubeRect.anchoredPosition.y;
        bool isHorizontallyAligned = horizontalDistance <= _gameConfig.CubeSize * (1 + _gameConfig.MaxHorizontalOffset);
        bool isVerticallyStacked = verticalDistance >= _gameConfig.CubeSize;

        return isHorizontallyAligned && isVerticallyStacked;
    }

    public void AddCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        float randomOffset = Random.Range(
            -_gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize,
            _gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize
        );
        
        Vector2 newPosition = _gameState.TowerCubes.Count == 0 
            ? rectTransform.anchoredPosition 
            : _gameState.LastCubePosition + new Vector2(randomOffset, _gameConfig.CubeSize);

        rectTransform.anchoredPosition = newPosition;
        _gameState.LastCubePosition = newPosition;
        _gameState.TowerCubes.Add(cube);
        
        OnCubeAdded?.Invoke(cube);
    }

    public void RemoveCube(GameObject cube)
    {
        int index = _gameState.TowerCubes.IndexOf(cube);
        if (index >= 0)
        {
            _gameState.TowerCubes.RemoveAt(index);
            
            for (int i = index; i < _gameState.TowerCubes.Count; i++)
            {
                var cubeRect = _gameState.TowerCubes[i].GetComponent<RectTransform>();
                Vector2 newPos = cubeRect.anchoredPosition;
                newPos.y -= _gameConfig.CubeSize;
                cubeRect.anchoredPosition = newPos;
            }
            
            if (_gameState.TowerCubes.Count > 0)
                _gameState.LastCubePosition = _gameState.TowerCubes[^1].GetComponent<RectTransform>().anchoredPosition;
                
            OnCubeRemoved?.Invoke(cube);
        }
    }

    public int GetCurrentHeight() => _gameState.CurrentHeight;
}
