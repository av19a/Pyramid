using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    
    private IGameConfig _gameConfig;
    private ICubeFactory _cubeFactory;
    
    [Inject]
    public void Construct(IGameConfig gameConfig, ICubeFactory cubeFactory)
    {
        _gameConfig = gameConfig;
        _cubeFactory = cubeFactory;
        InitializeScrollView();
    }

    private void InitializeScrollView()
    {
        ConfigureScrollView();
        CreateInitialCubes();
        SetContentSize();
    }

    private void ConfigureScrollView()
    {
        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        scrollRect.scrollSensitivity = _gameConfig.ScrollSpeed;
    }

    private void CreateInitialCubes()
    {
        for (int i = 0; i < _gameConfig.NumberOfCubes; i++)
        {
            var color = _gameConfig.CubeColors[i % _gameConfig.CubeColors.Length];
            var cube = _cubeFactory.CreateCube(content, color);
            PositionCube(cube, i);
        }
    }

    private void PositionCube(GameObject cube, int index)
    {
        var rectTransform = cube.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(index * _gameConfig.CubeSize, 0);
    }

    private void SetContentSize()
    {
        float totalWidth = _gameConfig.NumberOfCubes * _gameConfig.CubeSize;
        content.sizeDelta = new Vector2(totalWidth, _gameConfig.CubeSize);
    }
}
