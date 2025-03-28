using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.ScriptableObjects.Classes {
    [CreateAssetMenu(fileName = "New Class", menuName = "AbilitySystem/Class Data")]
    public class ClassData : ScriptableObject {
        [Header("Basic Information")] public string ClassName;
        public Sprite ClassIcon;
        [TextArea(3, 10)] public string Description;

        [Header("Class Stats")] public
            BaseStats ClassBaseStats;

        [Header("Settings")] public bool IsEnemyClass;
        public bool IsLocked;

        [Header("Abilities")] public List<AbilityData> Abilities = new();

        [Header("Enemy Abilities")] public List<AbilityConditionPairSO> EnemyAbilities = new();

        [Header("GameObjects")] public GameObject Prefab;
        public GameObject ClassSelectionModel;
    }
}