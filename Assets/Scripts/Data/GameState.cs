using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IGameState
{
    int CurrentHeight { get; }
    List<GameObject> TowerCubes { get; }
    Vector2 LastCubePosition { get; set; }
    void Reset();
}

public class GameState : IGameState
{
    private List<GameObject> _towerCubes = new();

    public int CurrentHeight => _towerCubes.Count;
    public List<GameObject> TowerCubes => _towerCubes;
    public Vector2 LastCubePosition { get; set; }

    public void Reset()
    {
        _towerCubes.Clear();
        LastCubePosition = Vector2.zero;
    }
}
