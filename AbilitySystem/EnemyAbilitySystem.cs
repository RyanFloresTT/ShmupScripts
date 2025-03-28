using System.Collections.Generic;
using UnityEngine;
using KBCore.Refs;
using System;

namespace AbilitySystem {
    /// <summary>
    /// Manages the enemy ability system.
    /// </summary>
    public class EnemyAbilitySystem : AbilitySystem {
        /// <summary>
        /// The enemy associated with this ability system.
        /// </summary>
        [SerializeField] [Self] Enemy enemy;

        /// <summary>
        /// Timer for each ability.
        /// </summary>
        readonly Dictionary<Ability, CountdownTimer> abilityTimers = new();

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable() {
            this.RegisterAbilities();
            base.OnEnable();
        }

        /// <summary>
        /// Registers the abilities available in the system.
        /// </summary>
        void RegisterAbilities() {
            AbilityFactory.Register<BuffAbilityData, BuffAbility>();
            AbilityFactory.Register<ProjectileAbilityData, ProjectileAbility>();
            AbilityFactory.Register<MeleeSwingAbilityData, MeleeSwingAbility>();
            AbilityFactory.Register<AreaDenialAbilityData, AreaDenialAbility>();
        }

        /// <summary>
        /// Called when an ability is performed.
        /// </summary>
        /// <param name="abilityIndex">The index of the performed ability.</param>
        public override void OnAbilityPerformed(int abilityIndex) {
            if (abilityIndex < 0 || abilityIndex >= this._abilities.Count) return; // Invalid index, do nothing

            Ability ability = this._abilities[abilityIndex];

            if (!ability.Unlocked) return; // Ability is not unlocked, do nothing

            if (!ability.IsOnCooldown()) this.abilityQueue.Enqueue(ability);
        }

        /// <summary>
        /// Handles the ability unlock event.
        /// </summary>
        /// <param name="unlockEvent">The ability unlock event data.</param>
        void HandleAbilityUnlocked(OnPlayerAbilityUnlocked unlockEvent) {
            this.UnlockAbility(unlockEvent.Ability);
        }

        /// <summary>
        /// Unlocks the specified ability.
        /// </summary>
        /// <param name="ability">The ability to unlock.</param>
        void UnlockAbility(Ability ability) {
            int abilityIndex = this._abilities.FindIndex((a) => a == ability);

            if (abilityIndex == -1) return; // Ability not found

            ability.ResetTimer(ability.AbilityData.CooldownSecs);
        }

        /// <summary>
        /// Populates the abilities list based on the ability data.
        /// </summary>
        protected override void PopulateAbilities() {
            foreach (AbilityData abilityData in this._abilitiesData)
                try {
                    Ability ability = AbilityFactory.Create(abilityData, this.enemy);
                    if (ability != null) this._abilities.Add(ability);
                }
                catch (Exception ex) {
                    Logger.Log($"Failed to create ability: {ex.Message}", Logger.LogCategory.AbilitySystem);
                }
        }

        /// <summary>
        /// Sets the ability data list.
        /// </summary>
        /// <param name="abilitiesData">The list of ability data.</param>
        public void SetAbilityList(List<AbilityData> abilitiesData) {
            this._abilitiesData = abilitiesData;
        }
    }
}