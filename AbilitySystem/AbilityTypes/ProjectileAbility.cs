using System;
using System.Threading;
using _Project.Scripts.AbilitySystem;
using _Project.Scripts.Audio;
using _Project.Scripts.Player;
using AbilitySystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileAbility : Ability<ProjectileAbilityData> {
    readonly ProjectileAbilityData data;
    readonly int abilityLayerIndex;
    bool isActive = false; // Flag to track if this ability is active

    public ProjectileAbility(ProjectileAbilityData data, Character character)
        : base(data, character) {
        this.data = data;
        this.character = character;
        character.OnPoolsInitialized += this.Handle_InitalizePool;
        this.abilityLayerIndex = character.animator.GetLayerIndex("Ability Layer");

        // Subscribe to the animation ready event
        character.OnCharacterReadyAbilityAnimation += this.SpawnProjectile;
    }

    void Handle_InitalizePool() {
        this.character.ProjectilePools.CreatePool(this.data.name, this.data.Projectile);
    }

    public override void Execute() {
        if (!this.character.Resource.Use(this.AbilityData.ResourceCost)) return;
        this.character.Resource.Gain(this.AbilityData.ResourceGeneration);

        base.Execute();

        // Set the flag to indicate this ability is active
        this.isActive = true;

        // Get attack speed and calculate animation speed multiplier
        float attackSpeed = this.character.Stats.AttackSpeed; // Use the AttackSpeed property from Stats
        float animationSpeedMultiplier = 1f / attackSpeed;
        this.character.animator.SetFloat("AttackSpeedMultiplier", animationSpeedMultiplier);

        // Play animation
        string stateName = this.data.AbilityAnimation.name;
        this.character.animator.Play(stateName, this.abilityLayerIndex);
    }

    void SpawnProjectile() {
        // Only spawn the projectile if this ability is active
        if (!this.isActive) return;

        // Reset the flag after spawning the projectile
        this.isActive = false;

        // Calculate spawn position and direction
        Vector3 spawnPosition = this.character.rightHand.position;
        Vector3 targetDirection = this.character.TargetDirection;

        if (this.character is Player) targetDirection -= spawnPosition;

        // Spawn the projectile
        GameObject projectileInstance = this.character.ProjectilePools.Get(this.data.name);
        if (projectileInstance == null) return;
        projectileInstance.transform.position = spawnPosition;
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.ProjectileManager = this.character.ProjectilePools;

        if (projectile == null) return;
        projectile.Initialize(targetDirection, this.data.Speed,
            (int)(this.data.DamageScalar * this.character.Stats.Attack), this.data.name, this.data.isPiercing,
            this.data.ExplosionSFX,
            this.data.Modifiers);
        _ = this.ReturnProjectileToPool(projectile, this.data.SecondTimeout,
            projectile.ReturnTokenSource.Token);

        // play sfx
        AudioCaller.PlaySFXAtPosition(this.data.CastSFX, spawnPosition);
    }

    async UniTaskVoid ReturnProjectileToPool(Projectile projectile, int secondDelay,
        CancellationToken cancellationToken) {
        try {
            await UniTask.Delay(secondDelay * 1000, false, PlayerLoopTiming.Update, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
                projectile.ProjectileManager.Return(this.data.name, projectile.gameObject);
        }
        catch (OperationCanceledException) {
            // Object gets destroyed from contact before expiry time
        }
    }

    void ResetAnimationLayer() {
        this.character.animator.SetLayerWeight(this.abilityLayerIndex, 0);
    }

    public void Dispose() {
        this.character.OnCharacterReadyAbilityAnimation -= this.SpawnProjectile;
    }
}