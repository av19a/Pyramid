using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateService
{
    void SaveGameState();
    void LoadGameState();
}

public class GameStateService : IGameStateService
{
    private const string SAVE_KEY = "GameState";
    private readonly ITowerService _towerService;
    private GameState _currentState = new GameState();

    public event System.Action<GameState> OnGameStateChanged;

    public GameStateService(ITowerService towerService)
    {
        _towerService = towerService;
        // Subscribe to tower events
        _towerService.OnCubeAdded += _ => SaveGameState();
        _towerService.OnCubeRemoved += _ => SaveGameState();
    }

    public void SaveGameState()
    {
        // Convert current tower state to save data
        _currentState.TowerCubes.Clear();
        // Save logic implementation
        string json = JsonUtility.ToJson(_currentState);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        OnGameStateChanged?.Invoke(_currentState);
    }

    public void LoadGameState()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            _currentState = JsonUtility.FromJson<GameState>(json);
            OnGameStateChanged?.Invoke(_currentState);
        }
    }
}
