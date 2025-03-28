using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem {
    [CreateAssetMenu(fileName = "Ability Data", menuName = "ScriptableObjects/Abilities/Ability Data")]
    public class AbilityData : ScriptableObject {
        [Header("Ability Data:")] public string AbilityName;
        public Sprite Icon;
        public int UnlockLevel;

        [Header("Cooldown:")] [Range(0, 60)] public float CooldownSecs;

        [Header("Resource:")] public int ResourceCost;
        public int ResourceGeneration;

        [Header("SFX: ")] public AudioClip CastSFX;
        [Header("Animation: ")] public AnimationClip AbilityAnimation;
    }
}