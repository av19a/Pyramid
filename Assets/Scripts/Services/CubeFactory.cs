using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICubeFactory
{
    GameObject CreateCube(Transform parent, Color color);
    GameObject CreateDraggedCube(Transform parent, GameObject original);
}

public class CubeFactory : ICubeFactory
{
    private readonly DiContainer _container;
    private readonly GameObject _cubePrefab;
    private readonly ICubePool _cubePool;

    public CubeFactory(
        DiContainer container,
        [Inject(Id = "CubePrefab")] GameObject cubePrefab,
        ICubePool cubePool)
    {
        _container = container;
        _cubePrefab = cubePrefab;
        _cubePool = cubePool;
    }

    public GameObject CreateCube(Transform parent, Color color)
    {
        var cube = _container.InstantiatePrefab(_cubePrefab, parent);
        cube.GetComponent<CubeView>().Initialize(color);
        return cube;
    }

    public GameObject CreateDraggedCube(Transform parent, GameObject original)
    {
        // Get cube from pool instead of creating new one
        var copy = _cubePool.Get(original, parent);
        
        // Copy any necessary properties from original
        var originalView = original.GetComponent<CubeView>();
        var copyView = copy.GetComponent<CubeView>();
        copyView.Initialize(originalView.Color, originalView.GetInstanceID());
        
        return copy;
        // var copy = _container.InstantiatePrefab(original, parent);
        // return copy;
    }
}
