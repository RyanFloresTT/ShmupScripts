using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages multiple GameObject pools for efficient object reuse.
/// </summary>
[Serializable]
public class GameObjectPoolManager {
    readonly Dictionary<string, GameObjectPool> pools = new();
    readonly Transform container;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameObjectPoolManager"/> class.
    /// </summary>
    /// <param name="container">The parent transform under which pooled objects will be organized.</param>
    public GameObjectPoolManager(Transform container) {
        this.container = container;
    }

    /// <summary>
    /// Creates or retrieves a pool for a specified key with the given prefab.
    /// </summary>
    /// <param name="key">The unique identifier for the pool.</param>
    /// <param name="prefab">The prefab GameObject to be pooled.</param>
    /// <param name="initializationMethod">Optional method to initialize pooled GameObjects.</param>
    public void CreatePool(string key, GameObject prefab, Action<GameObject> initializationMethod = null) {
        if (!this.pools.ContainsKey(key)) {
            GameObjectPool pool = new(prefab, this.container, initializationMethod);
            this.pools.Add(key, pool);
        }
    }

    /// <summary>
    /// Retrieves an object from the pool associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the pool.</param>
    /// <param name="enableOnGet">Determines if the retrieved object should be enabled.</param>
    /// <returns>The GameObject retrieved from the pool, or null if the pool doesn't exist.</returns>
    public GameObject Get(string key, bool enableOnGet = true) {
        if (this.pools.ContainsKey(key)) return this.pools[key].Get(enableOnGet);
        Debug.LogError($"Pool with key {key} not found.");
        return null;
    }

    public List<GameObjectPool> GetAllPools() {
        return this.pools.Values.ToList();
    }

    /// <summary>
    /// Returns a GameObject to its corresponding pool for reuse.
    /// </summary>
    /// <param name="key">The unique identifier for the pool.</param>
    /// <param name="gameObject">The GameObject to be returned to the pool.</param>
    /// <param name="disableOnReturn">Determines if the returned object should be disabled.</param>
    /// <param name="resetPositionOnReturn">Determines if the object's position should be reset upon return.</param>
    public void Return(string key, GameObject gameObject, bool disableOnReturn = true,
        bool resetPositionOnReturn = true) {
        if (this.pools.ContainsKey(key))
            this.pools[key].Return(gameObject, disableOnReturn, resetPositionOnReturn);
        else
            Debug.LogError($"Pool with key {key} not found.");
    }

    /// <summary>
    /// Reinitializes the pool associated with the specified key using a new GameObject.
    /// </summary>
    /// <param name="key">The unique identifier for the pool.</param>
    /// <param name="gameObject">The new GameObject to reinitialize the pool.</param>
    public void ReInitializeWithNewGameObject(string key, GameObject gameObject) {
        if (this.pools.ContainsKey(key))
            this.pools[key].ReInitializeWithNewGameObject(gameObject);
        else
            Debug.LogError($"Pool with key {key} not found.");
    }
}