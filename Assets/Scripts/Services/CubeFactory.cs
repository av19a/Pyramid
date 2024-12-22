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

    public CubeFactory(
        DiContainer container,
        [Inject(Id = "CubePrefab")] GameObject cubePrefab)
    {
        _container = container;
        _cubePrefab = cubePrefab;
    }

    public GameObject CreateCube(Transform parent, Color color)
    {
        var cube = _container.InstantiatePrefab(_cubePrefab, parent);
        cube.GetComponent<CubeView>().Initialize(color);
        return cube;
    }

    public GameObject CreateDraggedCube(Transform parent, GameObject original)
    {
        var copy = _container.InstantiatePrefab(original, parent);
        var copyCanvasGroup = copy.GetComponent<CanvasGroup>();
        copyCanvasGroup.blocksRaycasts = false;
        return copy;
    }
}
