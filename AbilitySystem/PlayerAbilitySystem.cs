using System;
using System.Collections.Generic;
using _Project.Input;
using _Project.Scripts.EventBus;
using _Project.Scripts.UI;
using AbilitySystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.AbilitySystem {
    /// <summary>
    /// Manages the player's ability system.
    /// </summary>
    public class PlayerAbilitySystem : global::AbilitySystem.AbilitySystem {
        /// <summary>
        /// The player associated with this ability system.
        /// </summary>
        global::Player player;

        /// <summary>
        /// The UI document used for the ability view.
        /// </summary>
        protected UIDocument view;

        /// <summary>
        /// Manages the ability view in the UI.
        /// </summary>
        AbilityViewManager viewManager;

        /// <summary>
        /// Timer for each ability.
        /// </summary>
        readonly Dictionary<Ability, CountdownTimer> abilityTimers = new();

        /// <summary>
        /// Event that fires off when the ActionBar UI Component is enabled
        /// </summary>
        EventBinding<OnActionBarUISet> actionBarSetBinding;

        InputManager inputManager;

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected void Awake() {
            this.actionBarSetBinding = new EventBinding<OnActionBarUISet>(this.Handle_ActionBarSet);
            EventBus<OnActionBarUISet>.Register(this.actionBarSetBinding);
        }

        void Start() {
            this.inputManager = InputManager.Instance;
            this.EnableInputActions();
        }


        void Handle_ActionBarSet(OnActionBarUISet set) {
            if (this == null) {
                Logger.Log("PlayerAbilitySystem is null or destroyed. Ignoring ActionBarUISet event.",
                    Logger.LogCategory.AbilitySystem);
                return;
            }

            this.player = this.GetComponent<global::Player>();
            this.RegisterAbilities();
            this.LoadAbilities();
            this.PopulateAbilities();

            this.player.SetupProjectilePool();

            this.view = set.ActionBar;
            this.viewManager = new AbilityViewManager(this.view);
            this.viewManager.AddAbilitiesToView(this._abilities);
            this.RegisterUnlockEvents();
            this.AddAbilityClickHandlers();
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
        /// Registers the unlock events for each ability.
        /// </summary>
        void RegisterUnlockEvents() {
            foreach (Ability ability in this._abilities) ability.OnAbilityUnlocked += this.Handle_AbilityUnlocked;
        }


        void LoadAbilities() {
            this._abilitiesData.Clear();

            var abilityDatas = this.player.ClassData.Abilities;

            foreach (AbilityData abilityData in abilityDatas)
                if (abilityData != null)
                    this._abilitiesData.Add(abilityData);
        }

        /// <summary>
        /// Called every frame to update the ability system.
        /// </summary>
        protected override void Update() {
            base.Update();
            if (this.view == null) return;

            this.viewManager.UpdateSlotsBusy(this._abilities);
        }

        /// <summary>
        /// Enables the input actions for the player.
        /// </summary>
        void EnableInputActions() {
            this.inputManager.OnPlayer_Performed_Ability1 += this.OnAbilityPerformed;
            this.inputManager.OnPlayer_Performed_Ability2 += this.OnAbilityPerformed;
            this.inputManager.OnPlayer_Performed_Ability3 += this.OnAbilityPerformed;
            this.inputManager.OnPlayer_Performed_Ability4 += this.OnAbilityPerformed;
            this.inputManager.OnPlayer_Performed_Ability5 += this.OnAbilityPerformed;
        }

        /// <summary>
        /// Called when the object becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            EventBus<OnActionBarUISet>.Deregister(this.actionBarSetBinding);
        }

        /// <summary>
        /// Adds click handlers to the UI ability slots.
        /// </summary>
        void AddAbilityClickHandlers() {
            var slotEls = this.viewManager.GetSlotEls();
            for (int i = 0; i < slotEls.Count; i++) {
                int position = i;
                slotEls[position].RegisterCallback<ClickEvent>(evt => { this.OnAbilityPerformed(position); });
            }
        }

        /// <summary>
        /// Called when an ability is performed.
        /// </summary>
        /// <param name="abilityIndex">The index of the performed ability.</param>
        public override void OnAbilityPerformed(int abilityIndex) {
            if (abilityIndex < 0 || abilityIndex >= this._abilities.Count) return;

            Ability ability = this._abilities[abilityIndex];

            if (!ability.Unlocked || ability.IsOnCooldown()) return;

            this.abilityQueue.Enqueue(ability);
        }

        /// <summary>
        /// Handles the ability unlocked event.
        /// </summary>
        /// <param name="unlockEvent">The ability unlock event data.</param>
        void HandleAbilityUnlocked(OnPlayerAbilityUnlocked unlockEvent) {
            this.UnlockAbility(unlockEvent.Ability);
        }

        /// <summary>
        /// Handles the ability unlocked event.
        /// </summary>
        /// <param name="ability">The unlocked ability.</param>
        void Handle_AbilityUnlocked(Ability ability) {
            this.UnlockAbility(ability);
        }

        /// <summary>
        /// Unlocks the specified ability.
        /// </summary>
        /// <param name="ability">The ability to unlock.</param>
        void UnlockAbility(Ability ability) {
            int abilityIndex = this._abilities.FindIndex((a) => a == ability);
            if (abilityIndex == -1) return;

            this.viewManager.UnlockAbility(abilityIndex);
            ability.ResetTimer(ability.AbilityData.CooldownSecs);
        }

        /// <summary>
        /// Populates the abilities list based on the ability data.
        /// </summary>
        protected override void PopulateAbilities() {
            foreach (AbilityData abilityData in this._abilitiesData)
                try {
                    Ability ability = AbilityFactory.Create(abilityData, this.player);
                    if (ability != null) this._abilities.Add(ability);
                }
                catch (Exception ex) {
                    Logger.Log($"Failed to create ability: {ex.Message}", Logger.LogCategory.AbilitySystem);
                }
        }
    }
}