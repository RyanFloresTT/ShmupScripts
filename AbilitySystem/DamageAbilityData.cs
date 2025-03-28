using AbilitySystem;
using UnityEngine;

public class DamageAbilityData : AbilityData {
    [Header("Damage:")]
    [Range(0.1f, 10)] public float DamageScalar;
    [Range(0,1)] public float ProcChance;
}
