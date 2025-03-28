using UnityEngine;

namespace AbilitySystem {
    [CreateAssetMenu(fileName = "Buff Ability Data", menuName = "ScriptableObjects/Abilities/Buff Ability Data")]
    public class BuffAbilityData : AbilityData {
        [field: SerializeField] public float BuffAmount { get; set; }
        [field: SerializeField] public OperatorType OperatorType { get; set; }
        [field: SerializeField] public StatType StatType { get; set; }
        [field: SerializeField] public float DurationSecs { get; set; }
    }
}