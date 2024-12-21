using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITowerService
{
    bool CanAddCube(Vector2 position);
    void AddCube(GameObject cube);
    void RemoveCube(GameObject cube);
    int GetCurrentHeight();
    event System.Action<GameObject> OnCubeAdded;
    event System.Action<GameObject> OnCubeRemoved;
}

public class TowerService : ITowerService
{
    private readonly IGameConfig _gameConfig;
    private readonly List<GameObject> _towerCubes = new();
    private Vector2 _lastCubePosition;

    public event System.Action<GameObject> OnCubeAdded;
    public event System.Action<GameObject> OnCubeRemoved;

    public TowerService(IGameConfig gameConfig)
    {
        _gameConfig = gameConfig;
    }

    public bool CanAddCube(Vector2 position)
    {
        // Check if position is within screen bounds
        var screenPoint = Camera.main.WorldToScreenPoint(position);
        return screenPoint.y < Screen.height - _gameConfig.CubeSize;
    }

    public void AddCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        float randomOffset = Random.Range(
            -_gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize,
            _gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize
        );

        Vector2 newPosition = _towerCubes.Count == 0 
            ? rectTransform.anchoredPosition 
            : _lastCubePosition + new Vector2(randomOffset, _gameConfig.CubeSize);

        rectTransform.anchoredPosition = newPosition;
        _lastCubePosition = newPosition;
        _towerCubes.Add(cube);
        
        Debug.Log("Add cube");
        
        OnCubeAdded?.Invoke(cube);
    }

    public void RemoveCube(GameObject cube)
    {
        int index = _towerCubes.IndexOf(cube);
        if (index >= 0)
        {
            _towerCubes.RemoveAt(index);
            // Adjust positions of cubes above
            for (int i = index; i < _towerCubes.Count; i++)
            {
                var cubeRect = _towerCubes[i].GetComponent<RectTransform>();
                Vector2 newPos = cubeRect.anchoredPosition;
                newPos.y -= _gameConfig.CubeSize;
                cubeRect.anchoredPosition = newPos;
            }
            
            if (_towerCubes.Count > 0)
                _lastCubePosition = _towerCubes[^1].GetComponent<RectTransform>().anchoredPosition;
                
            OnCubeRemoved?.Invoke(cube);
        }
    }

    public int GetCurrentHeight() => _towerCubes.Count;
}
