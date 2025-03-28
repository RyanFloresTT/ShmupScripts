using UnityEngine;

/// <summary>
/// Represents the statistical attributes of an entity.
/// </summary>
public class Stats {
    readonly StatsMediator mediator;
    readonly BaseStats baseStats;
    readonly AnimationCurve levelingCurve;

    /// <summary>
    /// Gets the mediator responsible for handling stat queries and modifiers.
    /// </summary>
    public StatsMediator Mediator => this.mediator;

    /// <summary>
    /// Gets the current attack value, modified by any active modifiers.
    /// </summary>
    public float Attack {
        get {
            Query q = new(StatType.Attack, this.baseStats.Attack);
            this.mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    /// <summary>
    /// Gets the current movement speed value, modified by any active modifiers.
    /// </summary>
    public float MoveSpeed {
        get {
            Query q = new(StatType.MoveSpeed, this.baseStats.MoveSpeed);
            this.mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    /// <summary>
    /// Gets the current movement speed value, modified by any active modifiers.
    /// </summary>
    public float AttackSpeed {
        get {
            Query q = new(StatType.AttackSpeed, this.baseStats.AttackSpeed);
            this.mediator.PerformQuery(this, q);
            return q.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stats"/> class with the specified mediator, base stats, and leveling curve.
    /// </summary>
    /// <param name="mediator">The mediator for handling stat queries and modifiers.</param>
    /// <param name="baseStats">The base statistical values.</param>
    /// <param name="levelCurve">The curve used to determine stat increases per level.</param>
    public Stats(StatsMediator mediator, BaseStats baseStats, AnimationCurve levelCurve) {
        this.mediator = mediator;
        this.baseStats = baseStats;
        this.levelingCurve = levelCurve;
    }

    /// <summary>
    /// Increases the stats based on the current level using the leveling curve.
    /// </summary>
    /// <param name="level">The current level of the entity.</param>
    public void LevelUp(int level) {
        float levelUpModifier = this.levelingCurve.Evaluate(level);
        this.Mediator.AddModifier(new StatModifier(StatType.Attack, new AddOperation(levelUpModifier), 0));
        this.Mediator.AddModifier(new StatModifier(StatType.MoveSpeed, new AddOperation(levelUpModifier), 0));
    }

    /// <summary>
    /// Clears Modifers on the Mediator by running the timer out on each instantly.
    /// </summary>
    public void ClearModifiers() {
        this.mediator.ClearModifiers();
    }
}

/// <summary>
/// Enumeration of different types of stats.
/// </summary>
public enum StatType {
    Attack,
    Defense,
    MoveSpeed,
    ResourceAmount,
    ResourceRate,
    ResourceMax,
    Health,
    AttackSpeed
}