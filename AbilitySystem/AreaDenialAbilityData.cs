using UnityEngine;

[CreateAssetMenu(fileName = "Area Denial Ability Data",
    menuName = "ScriptableObjects/Abilities/Area Denial Ability Data")]
public class AreaDenialAbilityData : DamageAbilityData {
    [Header("Area Denial")] public GameObject AreaDenialGameObject;
    public float Duration;
    public int TickRateSeconds;
    public float Radius;
    [Header("Animation")] public AnimationClip AbilityAnimation;
}