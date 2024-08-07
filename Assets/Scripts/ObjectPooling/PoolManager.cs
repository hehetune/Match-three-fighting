using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class PoolManager
{
    private static readonly Dictionary<string, ObjectPool> _pools;

    static PoolManager()
    {
        _pools = new();
        SceneManager.sceneUnloaded += ClearPool;
        // EventManager.AddListener("OnSceneChanging", ClearPool);
        // EventManager.AddListener("OnSceneChanged", ClearPool);
    }

    public static bool Get<T>(Prefab prefab, out T instance) where T : IPoolObject
    {
        try
        {
            if (!_pools.TryGetValue(prefab.UniquePrefabID, out var pool))
            {
                pool = new ObjectPool(prefab.gameObject);
                _pools[prefab.UniquePrefabID] = pool;
            }
            instance = (T)pool.Get();
            return true;
        }
        catch
        {
            string info = prefab == null ? "Null prefab" : $"prefab with id : {prefab.UniquePrefabID}";
            Debug.LogError($"Error when try to get {typeof(T)} from pool by {info}");
            instance = default;
            return false;
        }
    }

    public static void ClearPool()
    {
        foreach (var pool in _pools.Values)
        {
            pool.ClearPool();
        }
        _pools.Clear();
    }

    private static void ClearPool(Scene _)
    {
        ClearPool();
    }
}