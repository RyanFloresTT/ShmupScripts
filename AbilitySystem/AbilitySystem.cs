using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem {
    /// <summary>
    /// Represents the base class for the ability system.
    /// </summary>
    public abstract class AbilitySystem : MonoBehaviour {
        /// <summary>
        /// The list of ability data.
        /// </summary>
        [SerializeField] protected List<AbilityData> _abilitiesData;
        /// <summary>
        /// The list of abilities.
        /// </summary>
        protected List<Ability> _abilities = new();

        /// <summary>
        /// The queue of abilities to be executed.
        /// </summary>
        protected readonly Queue<Ability> abilityQueue = new();

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable() {
            PopulateAbilities();
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update() {
            foreach (var ability in _abilities) {
                ability.UpdateCooldownTimer(Time.deltaTime);
            }

            if (abilityQueue.Count > 0) {
                Ability nextAbility = abilityQueue.Peek();
                if (nextAbility != null && nextAbility.Unlocked && !nextAbility.IsOnCooldown()) {
                    abilityQueue.Dequeue().Execute();
                }
            }
        }

        /// <summary>
        /// Called when an ability is performed.
        /// </summary>
        /// <param name="abilityIndex">The index of the performed ability.</param>
        public abstract void OnAbilityPerformed(int abilityIndex);

        /// <summary>
        /// Populates the abilities list based on the ability data.
        /// </summary>
        protected abstract void PopulateAbilities();
    }
}