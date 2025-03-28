using System;
using AbilitySystem;
using KBCore.Refs;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Enemy;
using _Project.Scripts.EventBus;
using _Project.Scripts.Items;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.InputSystem;
using static _Project.Scripts.Enemy.Selector;
using Random = UnityEngine.Random;

public class Enemy : Character {
    static readonly int IsMoving = Animator.StringToHash("IsMoving");
    static readonly int Death = Animator.StringToHash("Death");
    [SerializeField] [Self] NavMeshAgent agent;
    public Transform Target { get; set; }

    [SerializeField] LootTable lootTable;
    [SerializeField] int spawnWeight;
    [SerializeField] [Self] EnemyAbilitySystem abilitySystem;
    public int SpawnWeight => this.spawnWeight;
    public bool ShouldMove { get; set; }
    public bool IsSpawning { get; set; }

    BTNode behaviorTree;

    float deathAnimationTime;

    public override void Awake() {
        base.Awake();
        this.SetupAbilities();
        this.SetupBehaviorTree();
        this.XP.OnEntityLevelUp += this.Handle_LevelUp;
    }

    void SetupAbilities() {
        var abilityDatas = this.ClassData.EnemyAbilities.Select(pair => pair.AbilityData).ToList();

        this.abilitySystem.SetAbilityList(abilityDatas);
    }

    void Start() {
        this.lootTable.InitalizeLootTable();
        this.Health.OnEntityDeath += this.Handle_Death;
        this.Health.OnEntityHealthChanged += this.Handle_HealthChange;

        foreach (AnimationClip clip in this.animator.runtimeAnimatorController.animationClips)
            if (clip.name == "Standing React Death Right")
                this.deathAnimationTime = clip.length;
    }

    void SetupBehaviorTree() {
        Sequence root = new();

        // Create nodes for each ability condition pair
        for (int i = 0; i < this.ClassData.EnemyAbilities.Count; i++) {
            AbilityConditionPairSO pair = this.ClassData.EnemyAbilities[i];
            AbilityNode abilityNode = new(pair.Condition, i, this.abilitySystem);
            root.AddChild(abilityNode);
        }

        // Add fallback actions
        root.AddChild(new ActionNode(this.MoveCloserToTarget));

        this.behaviorTree = root;
    }

    public void SetProjectilePoolManager(GameObjectPoolManager poolManager) {
        this.ProjectilePools = poolManager;
        this.OnPoolsInitialized?.Invoke();
    }

    void Handle_HealthChange(int change) { }

    void Update() {
        if (this.Target == null) {
            Debug.LogError("Target is null.");
            return;
        }

        if (this.Health.HasDied) return;

        this.UpdateMediators();
        this.UpdateAnimations();
        this.agent.speed = this.Stats.MoveSpeed;

        // Execute behavior tree logic
        this.behaviorTree.Execute(this);

        // Rotate towards the target
        this.RotateTowardsTarget();

        // Set destination only if ShouldMove is true
        if (this.ShouldMove)
            this.agent.destination = this.Target.position;
        else
            this.agent.ResetPath();

        // Update default target direction, which is always going to be forward facing
        this.TargetDirection = this.transform.forward;
    }

    public void CatchUpLevels(int level) {
        while (this.XP.Level < level) this.XP.LevelUp();
    }

    void Handle_LevelUp(int level) {
        this.Stats.LevelUp(level);
        this.Health.LevelUp(level);
        this.Resource.LevelUp(level);
    }

    protected override void Handle_Death() {
        this.lootTable.SpawnDrops(this.transform);
        this.Stats.ClearModifiers();
        base.Handle_Death();
        EventBus<OnEnemyDeath>.Raise(new OnEnemyDeath(this));
    }

    public void ResetStats() {
        this.Health.CurrentHealth = this.Health.MaxHealth;
    }

    public override Vector3 GetAreaDenialLocation(float radius) {
        Vector3 playerPosition = this.Target.position;
        float angle = Random.Range(0f, 360f);
        Vector3 offset = new(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius);
        return playerPosition + offset;
    }

    void MoveCloserToTarget(Enemy enemy) {
        this.agent.destination = this.Target.position;
    }

    void RotateTowardsTarget() {
        Vector3 direction = (this.Target.position - this.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        this.transform.rotation =
            Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * this.agent.angularSpeed);
    }

    void UpdateAnimations() {
        bool isMoving = this.agent.velocity.magnitude > 0.1f;
        this.animator.SetBool(IsMoving, isMoving);
    }

    void OnDrawGizmos() {
        if (this.agent == null || !this.agent.hasPath) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < this.agent.path.corners.Length - 1; i++)
            Gizmos.DrawLine(this.agent.path.corners[i], this.agent.path.corners[i + 1]);
    }

    public async UniTask PlayAnimationAsync(int animationId) {
        if (this.animator == null) {
            Debug.LogError("Animator component not found!");
            return;
        }

        this.animator.SetTrigger(animationId);

        await UniTask.Delay(TimeSpan.FromSeconds(this.deathAnimationTime));
    }
}