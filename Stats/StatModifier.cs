using System;
using _Project.Scripts.Player;
using UnityEngine;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


/// <summary>
/// Represents a modifier that applies a strategy to modify a specific type of stat value.
/// </summary>
public class StatModifier : IDisposable {
    /// <summary>
    /// The type of stat that this modifier affects.
    /// </summary>
    public StatType Type { get; }

    /// <summary>
    /// The strategy used to modify the stat value.
    /// </summary>
    public IOperationStrategy Strategy { get; }

    /// <summary>
    /// The icon associated with this modifier.
    /// </summary>
    public readonly Sprite icon;

    /// <summary>
    /// Gets or sets whether this modifier is marked for removal.
    /// </summary>
    public bool MarkedForRemoval { get; set; }

    /// <summary>
    /// Event triggered when this modifier is disposed.
    /// </summary>
    public event Action<StatModifier> OnDispose = delegate { };

    public GameObject BuffVFXInstance { get; }

    readonly CountdownTimer timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatModifier"/> class with a specified type, strategy, and duration.
    /// </summary>
    /// <param name="type">The type of stat this modifier affects.</param>
    /// <param name="strategy">The strategy used to modify the stat value.</param>
    /// <param name="duration">The duration for which this modifier applies, in seconds.</param>
    public StatModifier(StatType type, IOperationStrategy strategy, float duration, GameObject buffVFXPrefab = null,
        Character character = null) {
        this.Type = type;
        this.Strategy = strategy;
        if (duration > 0) {
            if (buffVFXPrefab != null && character != null)
                this.BuffVFXInstance = GameObject.Instantiate(buffVFXPrefab, character.transform);

            this.timer = new CountdownTimer(duration);
            this.timer.OnTimerStop += () => this.MarkedForRemoval = true;
            this.timer.Start();
        }
    }

    /// <summary>
    /// Updates the modifier's internal timer.
    /// </summary>
    /// <param name="deltaTime">The time passed since the last update.</param>
    public void Update(float deltaTime) {
        this.timer?.Tick(deltaTime);
        if (this.BuffVFXInstance != null && this.MarkedForRemoval) GameObject.Destroy(this.BuffVFXInstance);
    }

    /// <summary>
    /// Handles a query to modify a stat value based on this modifier.
    /// </summary>
    /// <param name="sender">The object sending the query.</param>
    /// <param name="query">The query containing the stat to modify.</param>
    public void Handle(object sender, Query query) {
        if (query.StatType == this.Type) query.Value = this.Strategy.Calculate(query.Value);
    }

    /// <summary>
    /// Disposes of this modifier and invokes the OnDispose event.
    /// </summary>
    public void Dispose() {
        this.OnDispose.Invoke(this);
    }
}