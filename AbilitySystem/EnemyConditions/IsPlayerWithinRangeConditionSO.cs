using UnityEngine;

public enum ComparisonOperator {
    GreaterThan,
    LessThan,
    EqualTo,
    GreaterThanOrEqualTo,
    LessThanOrEqualTo
}

[CreateAssetMenu(fileName = "NewCondition", menuName = "AbilitySystem/Condition/IsPlayerInRange")]
public class IsPlayerInRangeConditionSO : ConditionSO {
    public float range;
    public ComparisonOperator comparisonOperator;

    public override bool CheckCondition(Enemy enemy) {
        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);
        bool withinRange = false;

        switch (comparisonOperator) {
            case ComparisonOperator.GreaterThan:
                withinRange = distance > range;
                break;
            case ComparisonOperator.LessThan:
                withinRange = distance < range;
                break;
            case ComparisonOperator.EqualTo:
                withinRange = Mathf.Approximately(distance, range);
                break;
            case ComparisonOperator.GreaterThanOrEqualTo:
                withinRange = distance >= range;
                break;
            case ComparisonOperator.LessThanOrEqualTo:
                withinRange = distance <= range;
                break;
        }
        enemy.ShouldMove = !withinRange;
        return withinRange;
    }
}