using System.Collections.Generic;
using System.Linq;

// -----------------------------------------------------------------------
// This script is based on code from git-amend's YouTube channel.
// Original source: [Learn to Build an Advanced Event Bus | Unity Architecture] (https://www.youtube.com/watch?v=4_DTAnigmaQ)
// -----------------------------------------------------------------------


public interface IStatModifierApplicationOrder {
    float Apply(IEnumerable<StatModifier> statModifiers, float baseValue);
}

public class NormalStatModifierOrder : IStatModifierApplicationOrder {
    public float Apply(IEnumerable<StatModifier> statModifiers, float baseValue) {
        var allModifiers = statModifiers.ToList();

        foreach (var modifier in allModifiers.Where(modifier => modifier.Strategy is AddOperation)) {
            baseValue = modifier.Strategy.Calculate(baseValue);
        }
        
        foreach (var modifier in allModifiers.Where(modifier => modifier.Strategy is MultiplyOperation)) {
            baseValue = modifier.Strategy.Calculate(baseValue);
        }
        
        return baseValue;
    }
}