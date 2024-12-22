using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITowerService
{
    bool CanAddCube(GameObject cube);
    void AddCube(GameObject cube);
    void RemoveCube(GameObject cube);
    int GetCurrentHeight();
    event System.Action<GameObject> OnCubeAdded;
    event System.Action<GameObject> OnCubeRemoved;
}

public class TowerService : ITowerService
{
    private readonly IGameConfig _gameConfig;
    private readonly IGameState _gameState;

    public event System.Action<GameObject> OnCubeAdded;
    public event System.Action<GameObject> OnCubeRemoved;

    public TowerService(IGameConfig gameConfig, IGameState gameState)
    {
        _gameConfig = gameConfig;
        _gameState = gameState;
    }

    public bool CanAddCube(GameObject cube)
    {
        if (_gameState.TowerCubes.Count == 0)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(cube.transform.position);
            return screenPoint.x > Screen.width / 2;
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
