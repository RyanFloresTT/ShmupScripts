using Cysharp.Threading.Tasks;
using Saving;
using System;
using _Project.Scripts.EventBus;
using _Project.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents the player character in the game, inheriting from Character.
/// </summary>
public class Player : Character {
    [SerializeField] Transform debugTransform;
    [SerializeField] LayerMask targetHitLayer;
    static readonly int Death = Animator.StringToHash("Death");
    [Header("Player")] [SerializeField] SaveManager playerSaveManager;

    /// <summary>
    /// Gets the purse containing basic currency for the player.
    /// </summary>
    public Purse PlayerPurse { get; private set; }

    /// <summary>
    /// Gets the purse containing permanent currency for the player.
    /// </summary>
    public Purse PlayerPermPurse { get; private set; }

    /// <summary>
    /// Event invoked when the player levels up.
    /// </summary>
    public Action OnPlayerLevelUp { get; set; }

    EventBinding<OnProjectileTimeout> projectileTimeoutBinding;

    public UnityEvent onPlayerDeath;

    /// <summary>
    /// Initializes the player character on Awake.
    /// </summary>
    public override void Awake() {
        base.Awake();
        this.PlayerPurse = new Purse(0, 100);
        this.GetSavedData();
        this.SubscibeToStatEvents();
        this.SetUpPurseEvents();
    }

    void Start() {
        _ = this.MimicStartDelay();
    }

    void SubscibeToStatEvents() {
        this.XP.OnEntityLevelUp += this.Handle_LevelUp;
        this.XP.OnValueChanged += this.Handle_XPGained;
        this.Health.OnEntityHealthChanged += this.Handle_HealthChanged;
        this.Health.OnEntityDeath += this.Handle_Death;
        this.Resource.OnEntityResourceChanged += this.Handle_Player_ResourceChanged;
    }

    void Handle_XPGained(XPData data) {
        EventBus<OnPlayerExperienceGained>.Raise(new OnPlayerExperienceGained(data));
    }

    /// <summary>
    /// Invoked when the object becomes enabled and registers event handlers.
    /// </summary>
    void OnEnable() {
        this.projectileTimeoutBinding = new EventBinding<OnProjectileTimeout>(this.Handle_ProjectileTimeout);
        EventBus<OnProjectileTimeout>.Register(this.projectileTimeoutBinding);
    }

    /// <summary>
    /// Invoked when the object becomes disabled and deregisters event handlers.
    /// </summary>
    void OnDisable() {
        EventBus<OnProjectileTimeout>.Deregister(this.projectileTimeoutBinding);
    }

    /// <summary>
    /// Updates the player character during each frame.
    /// </summary>
    void Update() {
        this.UpdateMediators();
        this.UpdateTargetDirection();
    }

    void UpdateTargetDirection() {
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, this.targetHitLayer)) {
            this.debugTransform.position = hit.point;
            this.TargetDirection = hit.point;
        }
    }

    /// <summary>
    /// Sets up events to raise events when purse amounts change.
    /// </summary>
    void SetUpPurseEvents() {
        this.PlayerPurse.OnAmountChanged += x =>
            EventBus<OnPlayerGainedBasicCurrency>.Raise(new OnPlayerGainedBasicCurrency(this.PlayerPurse));
        this.PlayerPermPurse.OnAmountChanged += x =>
            EventBus<OnPlayerGainedPermCurrency>.Raise(new OnPlayerGainedPermCurrency(this.PlayerPermPurse));
    }

    /// <summary>
    /// Handles player resource changes and raises corresponding events.
    /// </summary>
    /// <param name="resource">The resource that changed.</param>
    void Handle_Player_ResourceChanged(Resource resource) {
        EventBus<OnPlayerResourceChanged>.Raise(new OnPlayerResourceChanged() { Resource = this.Resource });
    }

    /// <summary>
    /// Handles player death by deactivating the game object.
    /// </summary>
    protected override void Handle_Death() {
        base.Handle_Death();

        EventBus<OnPlayerDeath>.Raise(new OnPlayerDeath());
        this.onPlayerDeath.Invoke();

        if (this.animator == null)
            Debug.LogWarning("Animator is null.");

        this.animator.SetTrigger(Death);
    }

    /// <summary>
    /// Mimics a delay before starting the player, for testing purposes.
    /// </summary>
    async UniTaskVoid MimicStartDelay() {
        //await UniTask.Delay(1000);
        EventBus<OnPlayerEnabled>.Raise(new OnPlayerEnabled(this));
    }

    /// <summary>
    /// Sets up the projectile pool for the player.
    /// </summary>
    public void SetupProjectilePool() {
        GameObject poolContainer = new("Player Projectile Pool");
        this.ProjectilePools = new GameObjectPoolManager(poolContainer.transform);
        this.OnPoolsInitialized?.Invoke();
    }

    /// <summary>
    /// Handles projectile timeout events by returning the projectile to the pool.
    /// </summary>
    /// <param name="returnEvent">The projectile timeout event.</param>
    void Handle_ProjectileTimeout(OnProjectileTimeout returnEvent) {
        this.ProjectilePools.Return(returnEvent.Projectile.ProjectileName, returnEvent.Projectile.gameObject);
    }

    /// <summary>
    /// Handles health changes for the player and raises corresponding events.
    /// </summary>
    /// <param name="incomingChange">The amount of change in health.</param>
    void Handle_HealthChanged(int incomingChange) {
        EventBus<OnPlayerHealthChanged>.Raise(new OnPlayerHealthChanged() { Health = this.Health });
    }

    /// <summary>
    /// Handles leveling up of the player character.
    /// </summary>
    /// <param name="level">The new level of the player.</param>
    void Handle_LevelUp(int level) {
        this.Stats.LevelUp(level);
        this.Health.LevelUp(level);
        this.Resource.LevelUp(level);

        PlayerInfo playerData = new() {
            PlayerHealth = this.Health,
            PlayerResource = this.Resource,
            PlayerStats = this.Stats
        };
        EventBus<OnPlayerStatsUpdated>.Raise(new OnPlayerStatsUpdated(playerData));
        this.OnPlayerLevelUp?.Invoke();
    }

    /// <summary>
    /// Retrieves saved data for the player from the save manager.
    /// </summary>
    void GetSavedData() {
        Purse purse = this.playerSaveManager.SaveData.PlayerData.PermanentPurse;
        if (purse == null) return;
        this.PlayerPermPurse = new Purse(purse.Amount,
            purse.MaxAmount, this.playerSaveManager);
        Logger.Log($"Loading PlayerPermPurse \nAmount: {purse.Amount}",
            Logger.LogCategory.Saving);
    }

    public override Vector3 GetAreaDenialLocation(float radius) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return hit.point;
        return this.transform.position;
    }

    /// <summary>
    /// Makes a player choice, raising an event for handling.
    /// </summary>
    [ContextMenu("Make Choice")]
    void Make_PlayerChoice() {
        EventBus<OnPlayerChoiceMade>.Raise(new OnPlayerChoiceMade());
    }
}