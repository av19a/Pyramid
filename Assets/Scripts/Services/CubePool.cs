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
        // Get the instance ID of the original cube to use as a key
        int originalId = original.GetInstanceID();
        
        // Initialize pool for this cube type if it doesn't exist
        if (!_pools.ContainsKey(originalId))
        {
            _pools[originalId] = new Stack<GameObject>();
        }
        
        GameObject cube;
        if (_pools[originalId].Count > 0)
        {
            // Reuse existing cube from pool
            cube = _pools[originalId].Pop();
            cube.transform.SetParent(parent);
            cube.transform.localScale = Vector3.one;
            cube.SetActive(true);
        }
        else
        {
            // Create new cube if pool is empty
            cube = _container.InstantiatePrefab(original, parent);
            
            // Ensure the copy has the drag controller
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
        
        // Deactivate and reset the cube
        cube.SetActive(false);
        cube.transform.SetParent(null);
        
        // Store in the appropriate pool
        var originalId = cube.GetComponent<CubeView>().OriginalId;
        if (!_pools.ContainsKey(originalId))
        {
            _pools[originalId] = new Stack<GameObject>();
        }
        _pools[originalId].Push(cube);
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
