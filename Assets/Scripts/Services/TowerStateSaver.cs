using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public interface ITowerStateSaver
{
    void SaveTowerState();
    void LoadTowerState();
}

public class TowerStateSaver : ITowerStateSaver
{
    private readonly IGameState _gameState;
    private readonly ICubeFactory _cubeFactory;
    private readonly Canvas _canvas;

    [Inject]
    public TowerStateSaver(IGameState gameState, ICubeFactory cubeFactory, Canvas canvas)
    {
        _gameState = gameState;
        _cubeFactory = cubeFactory;
        _canvas = canvas;
    }
    
    private static readonly string SaveFolder = Path.Combine(Application.dataPath, "SavedData");

    public static string GetSaveFilePath(string fileName)
    {
        if (!Directory.Exists(SaveFolder))
        {
            Directory.CreateDirectory(SaveFolder);
        }
        return Path.Combine(SaveFolder, fileName);
    }

    public void SaveTowerState()
    {
        var cubeDataList = new List<CubeData>();

        foreach (var cube in _gameState.TowerCubes)
        {
            var rectTransform = cube.GetComponent<RectTransform>();
            var cubeView = cube.GetComponent<CubeView>();

            var data = new CubeData
            {
                Position = rectTransform.anchoredPosition,
                Color = cubeView.Color
            };

            cubeDataList.Add(data);
        }

        var json = JsonUtility.ToJson(new SerializableList<CubeData>(cubeDataList));
        string filePath = GetSaveFilePath("TowerState.json");

        File.WriteAllText(filePath, json);
    }

    public void LoadTowerState()
    {
        string filePath = GetSaveFilePath("TowerState.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file not found at: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        Debug.Log($"Loaded tower state from: {filePath}\nData: {json}");

        var cubeDataList = JsonUtility.FromJson<SerializableList<CubeData>>(json).Items;

        _gameState.Reset();

        foreach (var data in cubeDataList)
        {
            var cube = _cubeFactory.CreateCube(_canvas.transform, data.Color);
            var rectTransform = cube.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = data.Position;

            _gameState.TowerCubes.Add(cube);
            _gameState.LastCubePosition = data.Position;
        }
    }

    [System.Serializable]
    private class SerializableList<T>
    {
        public List<T> Items;

        public SerializableList(List<T> items) => Items = items;
    }
}

