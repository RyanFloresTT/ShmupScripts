using System;
using _Project.Scripts.Player;

namespace AbilitySystem {
    /// <summary>
    /// Represents a base class for abilities.
    /// </summary>
    public abstract class Ability {
        /// <summary>
        /// Executes the ability.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Gets the cooldown progress of the ability.
        /// </summary>
        /// <returns>The cooldown progress as a float.</returns>
        public abstract float GetCooldownProgress();

        /// <summary>
        /// Checks if the ability is on cooldown.
        /// </summary>
        /// <returns>True if the ability is on cooldown, otherwise false.</returns>
        public abstract bool IsOnCooldown();

        /// <summary>
        /// Updates the cooldown timer.
        /// </summary>
        /// <param name="deltaTime">The time to update the cooldown timer with.</param>
        public abstract void UpdateCooldownTimer(float deltaTime);

        /// <summary>
        /// Initializes the cooldown timer.
        /// </summary>
        /// <param name="cooldown">The cooldown time to set.</param>
        public abstract void InitCooldownTimer(float cooldown);

        /// <summary>
        /// Resets the cooldown timer.
        /// </summary>
        /// <param name="newTime">The new cooldown time to set.</param>
        public abstract void ResetTimer(float newTime);

        /// <summary>
        /// Indicates whether the ability is unlocked.
        /// </summary>
        public bool Unlocked { get; protected set; }

        /// <summary>
        /// The data associated with the ability.
        /// </summary>
        public AbilityData AbilityData { get; protected set; }

        /// <summary>
        /// The cooldown timer for the ability.
        /// </summary>
        protected CountdownTimer cooldownTimer;

        /// <summary>
        /// The global cooldown constant.
        /// </summary>
        protected const float GLOBAL_COOLDOWN = 0.5f;

        /// <summary>
        /// Event triggered when the ability is unlocked.
        /// </summary>
        public Action<Ability> OnAbilityUnlocked { get; set; }
    }

    /// <summary>
    /// Represents a specific implementation of an ability.
    /// </summary>
    /// <typeparam name="T">The type of ability data.</typeparam>
    public class Ability<T> : Ability where T : AbilityData {
        /// <summary>
        /// The character associated with the ability.
        /// </summary>
        protected Character character;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ability{T}"/> class.
        /// </summary>
        /// <param name="abilityData">The data associated with the ability.</param>
        /// <param name="character">The character associated with the ability.</param>
        protected Ability(T abilityData, Character character) {
            this.AbilityData = abilityData;
            this.character = character;

            this.InitCooldownTimer(abilityData.CooldownSecs);

            character.XP.OnEntityLevelUp += this.HandleLevelUp;

            if (this.AbilityData.UnlockLevel < 2) this.Unlocked = true;
        }

        /// <summary>
        /// Deconstructor of the <see cref="Ability{T}"/> class.
        /// </summary>
        ~Ability() {
            this.character.XP.OnEntityLevelUp -= this.HandleLevelUp;
        }

        /// <summary>
        /// Handles the level up event.
        /// </summary>
        /// <param name="level">The new level of the character.</param>
        protected void HandleLevelUp(int level) {
            if (!this.Unlocked && level >= this.AbilityData.UnlockLevel) {
                this.Unlocked = true;
                this.OnAbilityUnlocked?.Invoke(this);
            }
        }

        /// <summary>
        /// Executes the ability.
        /// </summary>
        public override void Execute() {
            if (this.IsOnCooldown() || this.AbilityData.CooldownSecs == 0) return;

            this.StartCooldown(this.AbilityData.CooldownSecs);
        }

        /// <summary>
        /// Starts the cooldown for the ability.
        /// </summary>
        /// <param name="cooldownSecs">The cooldown duration.</param>
        protected void StartCooldown(float cooldownSecs = GLOBAL_COOLDOWN) {
            this.ResetTimer(cooldownSecs);
            this.cooldownTimer.Start();
        }

        /// <summary>
        /// Checks if the ability is on cooldown.
        /// </summary>
        /// <returns>True if the ability is on cooldown, otherwise false.</returns>
        public override bool IsOnCooldown() {
            return this.cooldownTimer.IsRunning;
        }

        /// <summary>
        /// Updates the cooldown timer.
        /// </summary>
        /// <param name="deltaTime">The time to update the cooldown timer with.</param>
        public override void UpdateCooldownTimer(float deltaTime) {
            this.cooldownTimer.Tick(deltaTime);
        }

        /// <summary>
        /// Gets the cooldown progress of the ability.
        /// </summary>
        /// <returns>The cooldown progress as a float.</returns>
        public override float GetCooldownProgress() {
            return this.cooldownTimer.Progress;
        }

        /// <summary>
        /// Initializes the cooldown timer.
        /// </summary>
        /// <param name="cooldown">The cooldown time to set.</param>
        public override void InitCooldownTimer(float cooldown) {
            this.cooldownTimer = new CountdownTimer(cooldown);
        }

        /// <summary>
        /// Resets the cooldown timer.
        /// </summary>
        /// <param name="newTime">The new cooldown time to set.</param>
        public override void ResetTimer(float newTime) {
            this.cooldownTimer.Reset(newTime);
        }
    }
}