using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents a resource management system for an entity, such as mana or energy.
/// </summary>
public class Resource {
    readonly ResourceData data;
    readonly StatsMediator mediator;
    readonly AnimationCurve levelUpCurve;
    readonly CountdownTimer regenTimer;

    /// <summary>
    /// Event triggered when the resource amount changes.
    /// </summary>
    public Action<Resource> OnEntityResourceChanged;

    /// <summary>
    /// The mediator for handling stat queries and modifiers.
    /// </summary>
    public StatsMediator Mediator => mediator;

    /// <summary>
    /// The current amount of the resource.
    /// </summary>
    public float CurrentAmount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Resource"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling stat queries and modifiers.</param>
    /// <param name="baseData">The base data containing initial resource parameters.</param>
    /// <param name="levelUpCurve">The curve defining how resource parameters scale with level.</param>
    public Resource(StatsMediator mediator, ResourceData baseData, AnimationCurve levelUpCurve) {
        data = baseData;
        CurrentAmount = data.StartingAmount;
        regenTimer = new CountdownTimer(1);
        this.mediator = mediator;
        this.levelUpCurve = levelUpCurve;
    }

    /// <summary>
    /// Sets up the regeneration timer for the resource.
    /// </summary>
    public void SetUpTimer() {
        regenTimer.Start();
        regenTimer.OnTimerStop += () => {
            Gain(RegenAmount);
            regenTimer.Start();
        };
    }

    /// <summary>
    /// The amount of resource regenerated per tick.
    /// </summary>
    public int RegenAmount {
        get {
            var query = new Query(StatType.ResourceAmount, data.RegenAmount);
            mediator.PerformQuery(this, query);
            return (int)query.Value;
        }
    }

    /// <summary>
    /// The rate at which the resource regenerates.
    /// </summary>
    public float RegenRate {
        get {
            var query = new Query(StatType.ResourceRate, data.RegenRate);
            mediator.PerformQuery(this, query);
            return query.Value;
        }
    }

    /// <summary>
    /// The maximum amount of the resource that can be held.
    /// </summary>
    public float MaxResource {
        get {
            var query = new Query(StatType.ResourceMax, data.MaxAmount);
            mediator.PerformQuery(this, query);
            return query.Value;
        }
    }

    /// <summary>
    /// Uses a specified amount of the resource.
    /// </summary>
    /// <param name="useAmount">The amount of resource to use.</param>
    /// <returns>True if the resource was successfully used; otherwise, false.</returns>
    public bool Use(int useAmount) {
        if (useAmount <= CurrentAmount) {
            CurrentAmount -= useAmount;
            OnEntityResourceChanged?.Invoke(this);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Updates the regeneration timer based on the elapsed time.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void UpdateRegenTimer(float deltaTime) {
        regenTimer.Tick(deltaTime);
    }

    /// <summary>
    /// Increases the current amount of the resource by the specified amount.
    /// </summary>
    /// <param name="gainAmount">The amount to increase the resource by.</param>
    /// <returns>True if the resource amount was successfully increased; otherwise, false.</returns>
    public bool Gain(int gainAmount) {
        int sumAmount = (int)CurrentAmount + gainAmount;
        CurrentAmount = Mathf.Clamp(sumAmount, 0, (int)MaxResource);
        OnEntityResourceChanged?.Invoke(this);
        return true;
    }

    /// <summary>
    /// Applies level-up effects to the resource based on the given level.
    /// </summary>
    /// <param name="level">The level at which to apply the level-up effects.</param>
    public void LevelUp(int level) {
        var levelUpModifier = levelUpCurve.Evaluate(level);
        mediator.AddModifier(new StatModifier(StatType.ResourceRate, new AddOperation(levelUpModifier), 0));
        mediator.AddModifier(new StatModifier(StatType.ResourceAmount, new AddOperation(levelUpModifier), 0));
    }
}