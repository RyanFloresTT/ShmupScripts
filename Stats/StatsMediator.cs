using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


/// <summary>
/// Mediates stat modifications by applying stat modifiers to queries.
/// </summary>
public class StatsMediator {
    readonly List<StatModifier> listModifiers = new();
    readonly Dictionary<StatType, IEnumerable<StatModifier>> modifiersCache = new();
    readonly IStatModifierApplicationOrder order = new NormalStatModifierOrder();

    /// <summary>
    /// Event triggered when any stat is modified.
    /// </summary>
    public event Action<StatType> OnStatModified = delegate { };

    /// <summary>
    /// Applies the appropriate stat modifiers to the specified query.
    /// </summary>
    /// <param name="sender">The object sending the query.</param>
    /// <param name="query">The query containing the stat type and initial value.</param>
    public void PerformQuery(object sender, Query query) {
        if (!this.modifiersCache.ContainsKey(query.StatType))
            this.modifiersCache[query.StatType] =
                this.listModifiers.Where(modifier => modifier.Type == query.StatType).ToList();

        query.Value = this.order.Apply(this.modifiersCache[query.StatType], query.Value);
        this.OnStatModified.Invoke(query.StatType); // Trigger the event when the stat is modified
    }

    /// <summary>
    /// Invalidates the cache for the specified stat type, forcing it to be rebuilt on the next query.
    /// </summary>
    /// <param name="statType">The stat type whose cache should be invalidated.</param>
    void InvalidateCache(StatType statType) {
        this.modifiersCache.Remove(statType);
        this.OnStatModified.Invoke(statType);
    }

    /// <summary>
    /// Adds a new stat modifier to the mediator.
    /// </summary>
    /// <param name="modifier">The stat modifier to add.</param>
    public void AddModifier(StatModifier modifier) {
        this.listModifiers.Add(modifier);
        this.InvalidateCache(modifier.Type);
        modifier.MarkedForRemoval = false;

        modifier.OnDispose += _ => this.InvalidateCache(modifier.Type);
        modifier.OnDispose += _ => this.listModifiers.Remove(modifier);

        this.OnStatModified.Invoke(modifier.Type);
    }

    /// <summary>
    /// Updates all active stat modifiers, removing those marked for removal.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void Update(float deltaTime) {
        foreach (StatModifier modifier in this.listModifiers) modifier.Update(deltaTime);

        foreach (StatModifier modifier in this.listModifiers.Where(modifier => modifier.MarkedForRemoval).ToList())
            modifier.Dispose();
    }

    /// <summary>
    /// Will clear all modifiers by running the timer on them instantly.
    /// </summary>
    public void ClearModifiers() {
        foreach (StatModifier modifier in this.listModifiers) modifier.Update(float.MaxValue);
    }
}

/// <summary>
/// Represents a query to retrieve or modify a specific stat.
/// </summary>
public class Query {
    /// <summary>
    /// The type of stat being queried.
    /// </summary>
    public readonly StatType StatType;

    /// <summary>
    /// The current value associated with the stat query.
    /// </summary>
    public float Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Query"/> class with the specified stat type and initial value.
    /// </summary>
    /// <param name="statType">The type of stat to query.</param>
    /// <param name="value">The initial value associated with the query.</param>
    public Query(StatType statType, float value) {
        this.StatType = statType;
        this.Value = value;
    }
}