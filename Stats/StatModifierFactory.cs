using System;
using _Project.Scripts.Player;
using UnityEngine;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


public interface IStatModifierFactory {
    StatModifier Create(OperatorType operatorType, StatType statType, float value, float duration,
        GameObject vfx = null, Character character = null);
}

public class StatModifierFactory : IStatModifierFactory {
    public StatModifier Create(OperatorType operatorType, StatType statType, float value, float duration,
        GameObject vfx = null, Character character = null) {
        IOperationStrategy strategy = operatorType switch {
            OperatorType.Add => new AddOperation(value),
            OperatorType.Multiply => new MultiplyOperation(value),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new StatModifier(statType, strategy, duration, vfx, character);
    }
}