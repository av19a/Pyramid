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
    private IMessageService _messageService;
    private IAnimationService _animationService;
    
    private TowerAreaProvider _towerAreaProvider;

    public event Action<GameObject> OnCubeAdded;
    public event Action<GameObject> OnCubeRemoved;

    [Inject]
    public void Construct(
        IGameConfig gameConfig,
        IGameState gameState,
        IMessageService messageService,
        IAnimationService animationService,
        TowerAreaProvider towerAreaProvider)
    {
        _gameConfig = gameConfig;
        _gameState = gameState;
        _messageService = messageService;
        _animationService = animationService;
        _towerAreaProvider = towerAreaProvider;
    }

    public bool CanAddCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        
        if (_gameState.TowerCubes.Count == 0)
        {
            Vector3[] corners = new Vector3[4];
            cube.GetComponent<RectTransform>().GetWorldCorners(corners);
    
            foreach(Vector3 corner in corners)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(_towerAreaProvider.TowerArea, corner))
                {
                    return false;
                }
            }

            return true;
        }
        
        var lastCube = _gameState.TowerCubes[^1];
        
        Vector3[] towerCorners = new Vector3[4];
        Vector3[] cubeCorners = new Vector3[4];
    
        _towerAreaProvider.TowerArea.GetWorldCorners(towerCorners);
        lastCube.GetComponent<RectTransform>().GetWorldCorners(cubeCorners);
    
        float towerTopY = towerCorners[1].y;
        float cubeTopY = cubeCorners[1].y;

        if (cubeTopY + 14 > towerTopY)
        {
            return false;
        }

        foreach (var towerCube in _gameState.TowerCubes)
        {
            var towerCubeRect = towerCube.GetComponent<RectTransform>();
        
            float verticalDistance = Mathf.Abs(rectTransform.anchoredPosition.y - towerCubeRect.anchoredPosition.y);
            float horizontalDistance = Mathf.Abs(rectTransform.anchoredPosition.x - towerCubeRect.anchoredPosition.x);

            bool hasOverlap = verticalDistance < _gameConfig.CubeSize * 0.8f && 
                              horizontalDistance < _gameConfig.CubeSize * 0.8f;
                         
            if (hasOverlap)
                return true;
        }
    
        return false;
    }

    public void AddCube(GameObject cube)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        float randomOffset = Random.Range(
            -_gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize,
            _gameConfig.MaxHorizontalOffset * _gameConfig.CubeSize
        );
        
        Vector2 oldPosition = rectTransform.anchoredPosition;
        
        Vector2 newPosition = _gameState.TowerCubes.Count == 0 
            ? rectTransform.anchoredPosition 
            : _gameState.LastCubePosition + new Vector2(randomOffset, _gameConfig.CubeSize);

        _animationService.PlayTowerPlacementAnimation(cube, oldPosition, newPosition, () =>
        {
            rectTransform.anchoredPosition = newPosition;
            _gameState.LastCubePosition = newPosition;
            _gameState.TowerCubes.Add(cube);
        
            OnCubeAdded?.Invoke(cube);
        });
    }

    public void RemoveCube(GameObject cube)
    {
        int index = _gameState.TowerCubes.IndexOf(cube);
        if (index >= 0)
        {
            _gameState.TowerCubes.RemoveAt(index);
            
            int remainingAnimations = _gameState.TowerCubes.Count - index;
            if (remainingAnimations <= 0)
            {
                if (_gameState.TowerCubes.Count > 0)
                    _gameState.LastCubePosition = _gameState.TowerCubes[^1].GetComponent<RectTransform>().anchoredPosition;
                    
                OnCubeRemoved?.Invoke(cube);
                return;
            }

            for (int i = index; i < _gameState.TowerCubes.Count; i++)
            {
                var cubeToDrop = _gameState.TowerCubes[i];
                var cubeRect = cubeToDrop.GetComponent<RectTransform>();
                Vector2 currentPos = cubeRect.anchoredPosition;
                Vector2 targetPos = new Vector2(currentPos.x, currentPos.y - _gameConfig.CubeSize);

                _animationService.PlayCubeDropAnimation(
                    cubeToDrop, 
                    targetPos,
                    () => {
                        remainingAnimations--;
                        if (remainingAnimations == 0)
                        {
                            if (_gameState.TowerCubes.Count > 0)
                                _gameState.LastCubePosition = _gameState.TowerCubes[^1].GetComponent<RectTransform>().anchoredPosition;
                                
                            OnCubeRemoved?.Invoke(cube);
                        }
                    }
                );
            }
        }
        
        _messageService.ShowMessage("cube_removed");
    }

    public int GetCurrentHeight() => _gameState.CurrentHeight;
}
