using UnityEngine;

[CreateAssetMenu(fileName = "Melee Ability Data", menuName = "ScriptableObjects/Abilities/Melee Ability Data")]
public class MeleeSwingAbilityData : DamageAbilityData {
    [Header("Melee:")]
    public float Speed;
    public int SwingLength;
    public GameObject SwingTrigger;
}
