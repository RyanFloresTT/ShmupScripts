using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Ability Data",
    menuName = "ScriptableObjects/Abilities/Projectile Ability Data")]
public class ProjectileAbilityData : DamageAbilityData {
    [Header("Projectile:")] public float Speed;
    public GameObject Projectile;
    public int SecondTimeout;
    public List<StatModifierData> Modifiers;
    public bool isPiercing;
    [Header("SFX: ")] public AudioClip ExplosionSFX;
}