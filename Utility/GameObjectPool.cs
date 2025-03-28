using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Represents a pool of GameObject instances for efficient object reuse.
/// </summary>
[Serializable]
public class GameObjectPool {
    GameObject prefab;

    readonly int initialPoolAmount = 10;
    readonly int bufferAmount = 5;
    readonly Transform container;
    readonly Stack<GameObject> pool = new();

    /// <summary>
    /// Gets all objects currently in the pool.
    /// </summary>
    public List<GameObject> AllObjects => this.pool.ToList();

    /// <summary>
    /// Initializes a new instance of the GameObjectPool class.
    /// </summary>
    /// <param name="prefab">Prefab of the GameObject to pool.</param>
    /// <param name="container">Transform container for pooled GameObjects.</param>
    /// <param name="initializationMethod">Optional method to initialize pooled GameObjects.</param>
    public GameObjectPool(GameObject prefab, Transform container, Action<GameObject> initializationMethod = null) {
        this.prefab = prefab;
        this.container = container;
        this.InitializePool(this.initialPoolAmount, initializationMethod);
    }

    /// <summary>
    /// Retrieves a GameObject from the pool, optionally enabling it on retrieval.
    /// </summary>
    /// <param name="enableOnGet">Determines if the retrieved GameObject should be enabled.</param>
    /// <returns>The retrieved GameObject from the pool.</returns>
    public GameObject Get(bool enableOnGet = true) {
        if (this.pool.Count < this.bufferAmount) this.InitializePool(this.initialPoolAmount);
        GameObject gameObject = this.pool.Pop();
        if (enableOnGet) gameObject.SetActive(true);
        return gameObject;
    }

    /// <summary>
    /// Returns a GameObject to the pool, optionally disabling it and resetting its position.
    /// </summary>
    /// <param name="gameObject">The GameObject to return to the pool.</param>
    /// <param name="disableOnReturn">Determines if the GameObject should be disabled on return.</param>
    /// <param name="resetPositionOnReturn">Determines if the GameObject's position should be reset on return.</param>
    public void Return(GameObject gameObject, bool disableOnReturn = true, bool resetPositionOnReturn = true) {
        if (disableOnReturn) gameObject.SetActive(false);
        if (resetPositionOnReturn) gameObject.transform.position = this.container.position;
        this.pool.Push(gameObject);
    }

    void InitializePool(int initializeAmount, Action<GameObject> initializationMethod = null) {
        for (int i = 0; i < initializeAmount; i++) {
            GameObject gameObject = Object.Instantiate(this.prefab, this.container);
            initializationMethod?.Invoke(gameObject);
            this.Return(gameObject);
        }
    }

    /// <summary>
    /// Re-initializes the pool with a new GameObject prefab and optional initialization method.
    /// </summary>
    /// <param name="gameObject">New prefab of the GameObject to use for the pool.</param>
    /// <param name="initializationMethod">Optional method to initialize pooled GameObjects.</param>
    public void ReInitializeWithNewGameObject(GameObject gameObject, Action<GameObject> initializationMethod = null) {
        this.pool.Clear();
        this.prefab = gameObject;
        this.InitializePool(this.initialPoolAmount, initializationMethod);
    }
}