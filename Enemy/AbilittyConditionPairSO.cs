using AbilitySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityConditionPair", menuName = "AbilitySystem/AbilityConditionPair")]
public class AbilityConditionPairSO : ScriptableObject {
    public AbilityData AbilityData;
    public ConditionSO Condition;
}