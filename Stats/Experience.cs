using System;
using UnityEngine;

/// <summary>
/// Represents the experience system for an entity, allowing it to gain experience and level up.
/// </summary>
public class Experience {
    /// <summary>
    /// Occurs when the entity levels up.
    /// </summary>
    public Action<int> OnEntityLevelUp { get; set; }

    /// <summary>
    /// Gets the current level of the entity.
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Gets or sets the current experience points of the entity.
    /// </summary>
    public float CurrentXP { get; set; } = 0;

    /// <summary>
    /// Gets or sets the experience points required for the entity to level up.
    /// </summary>
    public float XpRequirement { get; set; } = 50;

    public Action<XPData> OnValueChanged { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Experience"/> class with default values.
    /// </summary>
    public Experience() {
        this.Level = 1;
        this.CurrentXP = 0;
        this.XpRequirement = 50;
    }

    /// <summary>
    /// Adds the specified amount of experience points to the entity.
    /// </summary>
    /// <param name="incomingXP">The amount of experience points to add.</param>
    public void GainExperince(float incomingXP) {
        this.CurrentXP += incomingXP;

        this.OnValueChanged?.Invoke(new XPData(this.CurrentXP, this.XpRequirement, this.Level));

        if (this.CurrentXP >= this.XpRequirement) this.LevelUp();
    }

    /// <summary>
    /// Levels up the entity, increasing its level and adjusting the experience requirement.
    /// </summary>
    public virtual void LevelUp() {
        this.CurrentXP -= this.XpRequirement;
        this.Level++;
        this.XpRequirement *= 1.25f;
        this.OnEntityLevelUp?.Invoke(this.Level);
    }
}

public class XPData {
    public float CurrentXP { get; set; }
    public float XPRequirement { get; set; }
    public int Level { get; set; }

    public XPData(float currentXP, float xpRequirement, int level) {
        this.CurrentXP = currentXP;
        this.XPRequirement = xpRequirement;
        this.Level = level;
    }
}