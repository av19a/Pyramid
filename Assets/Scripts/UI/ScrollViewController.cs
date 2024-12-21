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
    private GameObject _cubePrefab;
    
    [Inject]
    public void Construct(
        IGameConfig gameConfig,
        [Inject(Id = "CubePrefab")] GameObject cubePrefab
    )
    {
        _gameConfig = gameConfig;
        _cubePrefab = cubePrefab;
        InitializeScrollView();
    }

    private void InitializeScrollView()
    {
        // Set up scroll view properties
        scrollRect.horizontal = true;
        scrollRect.vertical = false;
        scrollRect.scrollSensitivity = _gameConfig.ScrollSpeed;
        
        // Create initial cubes
        for (int i = 0; i < _gameConfig.NumberOfCubes; i++)
        {
            CreateCube(i);
        }
        
        // Calculate content width
        float totalWidth = _gameConfig.NumberOfCubes * _gameConfig.CubeSize;
        content.sizeDelta = new Vector2(totalWidth, _gameConfig.CubeSize);
    }

    private void CreateCube(int index)
    {
        var cube = Instantiate(_cubePrefab, content);
        var color = _gameConfig.CubeColors[index % _gameConfig.CubeColors.Length];
        cube.GetComponent<CubeView>().Construct(color);
        
        // Position cube in scroll view
        var rectTransform = cube.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(
            index * _gameConfig.CubeSize,
            0
        );
    }
}
