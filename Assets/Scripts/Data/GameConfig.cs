using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject, IGameConfig
{
    [SerializeField] private int numberOfCubes = 20;
    [SerializeField] private Color[] cubeColors;
    [SerializeField] private float maxHorizontalOffset = 0.5f;
    [SerializeField] private float cubeSize = 100f;
    [SerializeField] private float scrollSpeed = 10f;

    // Implement all IGameConfig properties
    public int NumberOfCubes => numberOfCubes;
    public Color[] CubeColors => cubeColors;
    public float MaxHorizontalOffset => maxHorizontalOffset;
    public float CubeSize => cubeSize;
    public float ScrollSpeed => scrollSpeed;
}

public interface IGameConfig
{
    int NumberOfCubes { get; }        // Number of cubes in scroll view
    Color[] CubeColors { get; }       // Available cube colors
    float MaxHorizontalOffset { get; } // Maximum tower stacking offset (0.5 = 50%)
    float CubeSize { get; }           // Size of cube in UI units
    float ScrollSpeed { get; }        // Scroll view sensitivity
}
