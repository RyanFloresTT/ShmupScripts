using System;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Represents the health of an entity with methods to heal, take damage, and handle death events.
/// </summary>
public class Health {
    /// <summary>
    /// Event triggered when the entity's health reaches zero.
    /// </summary>
    public Action OnEntityDeath { get; set; }

    /// <summary>
    /// Event triggered when the entity's health changes.
    /// </summary>
    public Action<int> OnEntityHealthChanged;

    /// <summary>
    /// Maximum health of the entity, influenced by stats modifiers.
    /// </summary>
    public float MaxHealth {
        get {
            Query q = new(StatType.Health, this.maxHealth);
            this.mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    /// <summary>
    /// A flag to see if a character has already died.
    /// </summary>
    internal bool HasDied { get; set; }

    /// <summary>
    /// Current health of the entity.
    /// </summary>
    public float CurrentHealth { get; set; }

    /// <summary>
    /// Mediator responsible for applying stat modifiers to health.
    /// </summary>
    public StatsMediator Mediator => this.mediator;

    readonly float maxHealth;
    readonly StatsMediator mediator;
    readonly AnimationCurve levelUpCurve;

    /// <summary>
    /// A simple flag to let us know if the character has already died, so (for example) we don't trigger the death event repeatedly.
    /// </summary>
    bool hasDied;

    /// <summary>
    /// Constructor for the Health class.
    /// </summary>
    /// <param name="maxHealth">Initial maximum health value.</param>
    /// <param name="mediator">Stats mediator for handling stat modifications.</param>
    /// <param name="levelUpCurve">Animation curve for scaling health with level.</param>
    public Health(float maxHealth, StatsMediator mediator, AnimationCurve levelUpCurve) {
        this.maxHealth = maxHealth;
        this.mediator = mediator;
        this.levelUpCurve = levelUpCurve;
        this.CurrentHealth = this.MaxHealth;
    }

    /// <summary>
    /// Heals the entity by the specified amount.
    /// </summary>
    /// <param name="incomingHeal">Amount of health to heal.</param>
    public void Heal(float incomingHeal) {
        this.CurrentHealth = this.CurrentHealth + incomingHeal > this.MaxHealth
            ? this.MaxHealth
            : this.CurrentHealth + incomingHeal;
        this.OnEntityHealthChanged?.Invoke((int)incomingHeal);
    }

    /// <summary>
    /// Inflicts damage on the entity.
    /// </summary>
    /// <param name="incomingDamage">Amount of damage to inflict.</param>
    public void TakeDamage(float incomingDamage) {
        if (this.HasDied) return;

        this.CurrentHealth -= incomingDamage;

        this.CurrentHealth = Mathf.Clamp(this.CurrentHealth, 0, this.MaxHealth);

        Debug.Log($"Health: {this.CurrentHealth} / {this.MaxHealth}");

        // Notify of health change
        this.OnEntityHealthChanged?.Invoke((int)incomingDamage);

        if (!(this.CurrentHealth <= 0) || this.HasDied) return;
        this.HasDied = true;
        this.OnEntityDeath?.Invoke();
    }

    /// <summary>
    /// Adjusts the entity's health when leveling up.
    /// </summary>
    /// <param name="level">Current level of the entity.</param>
    public void LevelUp(int level) {
        float healthRatio = this.CurrentHealth / this.MaxHealth;

        float levelUpModifier = this.levelUpCurve.Evaluate(level);
        this.Mediator.AddModifier(new StatModifier(StatType.Health, new AddOperation(levelUpModifier), 0));

        this.CurrentHealth = this.MaxHealth * healthRatio;
        this.OnEntityHealthChanged?.Invoke(0);
    }

    /// <summary>
    /// Resets the health component of the entity to its maximum value
    /// and clears the death state flag.
    /// </summary>
    public void Reset() {
        this.hasDied = false;
        this.CurrentHealth = this.MaxHealth;
    }
}