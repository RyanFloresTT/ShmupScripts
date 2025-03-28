using UnityEngine;

public abstract class ConditionSO : ScriptableObject {
    public abstract bool CheckCondition(Enemy enemy);
}

[CreateAssetMenu(fileName = "NewCondition", menuName = "AbilitySystem/Condition/IsFriendlyInRange")]
public class IsFriendlyInRangeConditionSO : ConditionSO {
    public float range;

    public override bool CheckCondition(Enemy enemy) {
        // Logic to check if there is a friendly unit within range
        return false; // Placeholder
    }
}
