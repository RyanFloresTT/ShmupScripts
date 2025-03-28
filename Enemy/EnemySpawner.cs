using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using _Project.Scripts.EventBus;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Manages spawning of enemies in waves based on player interactions and game events.
/// </summary>
public class EnemySpawner : MonoBehaviour {
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] AnimationCurve enemySpawnCurve;
    [SerializeField] float spawnDistance = 10f;
    [SerializeField] List<Transform> spawnPoints;

    // TODO: Move this to a static class or something, it's on the enemy class too
    static readonly int Death = Animator.StringToHash("Death");

    /// <summary>
    /// Gets or sets the manager for enemy projectile pools.
    /// </summary>
    public GameObjectPoolManager EnemyProjectilePools { get; set; }

    GameObjectPoolManager enemySpawnPools;
    Dictionary<GameObject, int> spawnWeights;

    int totalAlive = 0;
    int currentEnemyLevel = 1;
    int currentWave;

    EventBinding<OnEnemyDeath> enemyDeathBinding;
    EventBinding<OnPlayerEnabled> playerEnabledBinding;
    EventBinding<OnPlayerChoiceMade> playerChoiceMadeBinding;

    Camera mainCamera;
    Transform player;
    bool playerIsUpgrading;

    /// <summary>
    /// Initializes enemy spawn pools and other necessary components.
    /// </summary>
    void Awake() {
        this.PopulateSpawnWeights();

        this.EnemyProjectilePools = new GameObjectPoolManager(this.transform);
        this.enemySpawnPools = new GameObjectPoolManager(this.transform);
        this.CreateEnemySpawnPools();

        this.mainCamera = Camera.main;
        this.playerIsUpgrading = true;
        this.currentWave = 0;
    }

    /// <summary>
    /// Creates enemy spawn pools for each enemy prefab.
    /// </summary>
    void CreateEnemySpawnPools() {
        foreach (GameObject enemy in this.enemyPrefabs)
            this.enemySpawnPools.CreatePool(enemy.name, enemy, (obj) => {
                this.InitializeEnemy(obj);
                this.PlaceOnNavMesh(obj);
            });
    }

    void PlaceOnNavMesh(GameObject enemyObj) {
        if (NavMesh.SamplePosition(enemyObj.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            enemyObj.transform.position = hit.position;
        else
            Debug.LogWarning("Failed to place enemy on NavMesh. Ensure the NavMesh is properly baked.");
    }

    /// <summary>
    /// Populates spawn weights for each enemy prefab based on their spawn weight.
    /// </summary>
    void PopulateSpawnWeights() {
        this.spawnWeights = new Dictionary<GameObject, int>();
        foreach (GameObject enemyPrefab in this.enemyPrefabs)
            if (enemyPrefab.TryGetComponent<Enemy>(out Enemy enemy))
                this.spawnWeights.Add(enemyPrefab, enemy.SpawnWeight);
    }

    /// <summary>
    /// Registers event handlers when the object is enabled.
    /// </summary>
    void OnEnable() {
        this.enemyDeathBinding =
            AsyncEventHandler<OnEnemyDeath>.RegisterAsync(async (deathEvent) =>
                await this.Handle_EnemyDeath(deathEvent));
        this.playerEnabledBinding = new EventBinding<OnPlayerEnabled>(this.Handle_PlayerEnabled);
        this.playerChoiceMadeBinding = new EventBinding<OnPlayerChoiceMade>(this.Handle_PlayerChoice);

        EventBus<OnEnemyDeath>.Register(this.enemyDeathBinding);
        EventBus<OnPlayerEnabled>.Register(this.playerEnabledBinding);
        EventBus<OnPlayerChoiceMade>.Register(this.playerChoiceMadeBinding);
    }

    /// <summary>
    /// Handles player enabled events to initialize the player position.
    /// </summary>
    /// <param name="enabled">The player enabled event.</param>
    void Handle_PlayerEnabled(OnPlayerEnabled enabled) {
        this.player = enabled.Player.transform;
        this.StartNextWave();
    }

    /// <summary>
    /// Handles player choice events to start the next wave.
    /// </summary>
    /// <param name="made">The player choice event.</param>
    void Handle_PlayerChoice(OnPlayerChoiceMade made) {
        this.playerIsUpgrading = false;
    }

    /// <summary>
    /// Deregisters event handlers when the object is disabled.
    /// </summary>
    void OnDisable() {
        EventBus<OnEnemyDeath>.Deregister(this.enemyDeathBinding);
        EventBus<OnPlayerEnabled>.Deregister(this.playerEnabledBinding);
        EventBus<OnPlayerChoiceMade>.Deregister(this.playerChoiceMadeBinding);
    }

    /// <summary>
    /// Handles enemy death events to return enemies to the spawn pool and start the next wave if no enemies are alive.
    /// </summary>
    /// <param name="death">The enemy death event.</param>
    async UniTask Handle_EnemyDeath(OnEnemyDeath death) {
        await death.Enemy.PlayAnimationAsync(Death);


        // Tween the enemy to push it below the floor
        await Tween.PositionY(death.Enemy.gameObject.transform,
            death.Enemy.gameObject.transform.position.y - 5,
            1.5f,
            Ease.Linear);


        // Reset enemy health and return to pool
        death.Enemy.Health.HasDied = false;
        this.enemySpawnPools.Return(death.Enemy.name.Replace("(Clone)", "").Trim(), death.Enemy.gameObject);

        this.totalAlive--;

        if (this.totalAlive == 0) this.StartNextWave();
    }

    /// <summary>
    /// Starts the next wave of enemies based on the current game state.
    /// </summary>
    void StartNextWave() {
        this.currentWave++;

        EventBus<OnNextWave>.Raise(new OnNextWave(this.currentWave));

        this.LevelUpEnemies();
        int spendingAmount = Mathf.FloorToInt(this.enemySpawnCurve.Evaluate(this.currentWave));

        var selectedEnemies = this.AllocateEnemies(spendingAmount);
        foreach (GameObject enemy in selectedEnemies) this.SpawnEnemy(enemy);
    }

    /// <summary>
    /// Allocates enemies to spawn based on the spending amount for the current wave.
    /// </summary>
    /// <param name="spendingAmount">The amount of resources available for enemy allocation.</param>
    /// <returns>A list of selected enemy prefabs to spawn.</returns>
    List<GameObject> AllocateEnemies(int spendingAmount) {
        List<GameObject> selectedEnemies = new();
        List<GameObject> enemyTypes = new(this.spawnWeights.Keys);
        int enemyCount = enemyTypes.Count;

        while (spendingAmount > 0) {
            GameObject selectedEnemy = enemyTypes[Random.Range(0, enemyCount)];
            int cost = this.spawnWeights[selectedEnemy];

            if (spendingAmount < cost) continue;

            selectedEnemies.Add(selectedEnemy);
            spendingAmount -= cost;
        }

        return selectedEnemies;
    }

    /// <summary>
    /// Spawns an enemy at a valid position on the NavMesh near the desired location.
    /// </summary>
    /// <param name="enemyPrefab">The enemy prefab to spawn.</param>
    void SpawnEnemy(GameObject enemyPrefab) {
        Vector3 spawnPoint = this.GetRandomSpawnPosition();

        Vector3 validSpawnPoint = this.GetValidNavMeshPosition(spawnPoint);

        GameObject enemyObj = this.enemySpawnPools.Get(enemyPrefab.name);
        if (enemyObj == null) return;
        enemyObj.transform.position = validSpawnPoint;

        NavMeshAgent agent = enemyObj.GetComponent<NavMeshAgent>();
        if (agent != null) {
            agent.enabled = true;
            agent.Warp(validSpawnPoint);
        }

        this.totalAlive++;
        Enemy currentEnemy = enemyObj.GetComponent<Enemy>();

        currentEnemy.ResetStats();
        currentEnemy.Health.Reset();
        currentEnemy.Target = this.player;
    }

    /// <summary>
    /// Finds a valid position on the NavMesh near the given position.
    /// </summary>
    /// <param name="position">The desired position.</param>
    /// <returns>A valid position on the NavMesh.</returns>
    Vector3 GetValidNavMeshPosition(Vector3 position) {
        NavMeshHit hit;
        float maxDistance = 10f; // Maximum distance to search for a valid position
        int areaMask = NavMesh.AllAreas; // Include all NavMesh areas

        return NavMesh.SamplePosition(position, out hit, maxDistance, areaMask)
            ? hit.position
            : position;
    }

    /// <summary>
    /// Gets a random spawn position from the spawn points list (if not empty), or around the player.
    /// </summary>
    /// <returns>A random spawn position.</returns>
    Vector3 GetRandomSpawnPosition() {
        if (this.spawnPoints is { Count: > 0 }) {
            // Use a random spawn point from the list
            Transform randomSpawnPoint = this.spawnPoints[Random.Range(0, this.spawnPoints.Count)];
            return randomSpawnPoint.position;
        }

        // Fall back to spawning around the player
        Vector2 randomCircle = Random.insideUnitCircle.normalized * this.spawnDistance;
        return new Vector3(randomCircle.x, 0f, randomCircle.y) + this.player.position;
    }

    /// <summary>
    /// Levels up enemies every 5 waves to increase their difficulty.
    /// </summary>
    void LevelUpEnemies() {
        if (this.currentWave % 5 == 0) {
            Debug.Log("The Enemies Grow Stronger...");

            this.currentEnemyLevel++;
            LevelUpData levelData = new() { Level = this.currentEnemyLevel };
            EventBus<OnEnemyLevelUp>.Raise(new OnEnemyLevelUp(levelData));

            foreach (GameObjectPool enemyObjPool in this.enemySpawnPools.GetAllPools())
            foreach (GameObject enemyObj in enemyObjPool.AllObjects)
                if (enemyObj.TryGetComponent<Enemy>(out Enemy enemy))
                    enemy.CatchUpLevels(this.currentEnemyLevel);
        }
    }

    /// <summary>
    /// Initializes an enemy object with a projectile pool manager.
    /// </summary>
    /// <param name="enemyObj">The enemy object to initialize.</param>
    void InitializeEnemy(GameObject enemyObj) {
        if (enemyObj.TryGetComponent(out Enemy enemyComponent))
            enemyComponent.SetProjectilePoolManager(this.EnemyProjectilePools);
    }
}