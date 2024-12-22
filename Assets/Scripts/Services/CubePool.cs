using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICubePool
{
    GameObject Get(GameObject original, Transform parent);
    void Return(GameObject cube);
    void Clear();
}

public class CubePool : ICubePool
{
    private readonly DiContainer _container;
    private readonly Dictionary<int, Stack<GameObject>> _pools = new();
    
    public CubePool(DiContainer container)
    {
        _container = container;
    }
    
    public GameObject Get(GameObject original, Transform parent)
    {
        int originalId = original.GetInstanceID();
        Debug.Log($"Trying to get cube for original ID: {originalId}. Pool exists: {_pools.ContainsKey(originalId)}");
        
        if (_pools.ContainsKey(originalId))
        {
            Debug.Log($"Pool count for ID {originalId}: {_pools[originalId].Count}");
        }
        
        // Initialize pool for this cube type if it doesn't exist
        if (!_pools.ContainsKey(originalId))
        {
            _pools[originalId] = new Stack<GameObject>();
        }
        
        GameObject cube;
        if (_pools[originalId].Count > 0)
        {
            Debug.Log("Reusing cube from pool");
            cube = _pools[originalId].Pop();
            cube.transform.SetParent(parent);
            cube.transform.localScale = Vector3.one;
            cube.SetActive(true);
        }
        else
        {
            Debug.Log("Creating new cube - pool was empty");
            cube = _container.InstantiatePrefab(original, parent);
            
            if (!cube.GetComponent<CubeDragController>())
            {
                cube.AddComponent<CubeDragController>();
                _container.InjectGameObject(cube);
            }
        }
        
        return cube;
    }
    
    public void Return(GameObject cube)
    {
        if (cube == null) return;
        
        var cubeView = cube.GetComponent<CubeView>();
        int originalId = cubeView.OriginalId;
        Debug.Log($"Returning cube to pool with original ID: {originalId}");
        
        cube.SetActive(false);
        cube.transform.SetParent(null);
        
        if (!_pools.ContainsKey(originalId))
        {
            _pools[originalId] = new Stack<GameObject>();
        }
        _pools[originalId].Push(cube);
        Debug.Log($"Pool count for ID {originalId} after return: {_pools[originalId].Count}");
    }
    
    public void Clear()
    {
        foreach (var pool in _pools.Values)
        {
            while (pool.Count > 0)
            {
                var cube = pool.Pop();
                Object.Destroy(cube);
            }
        }
        _pools.Clear();
    }
}
