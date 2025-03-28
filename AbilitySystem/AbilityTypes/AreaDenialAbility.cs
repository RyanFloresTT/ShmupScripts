using _Project.Scripts.Managers;
using _Project.Scripts.Player;
using UnityEngine;

namespace AbilitySystem {
    /// <summary>
    /// Represents a buff ability that applies a stat modifier to a character.
    /// </summary>
    public class AreaDenialAbility : Ability<AreaDenialAbilityData> {
        readonly AreaDenialAbilityData data;
        readonly int abilityLayerIndex;
        bool isActive = false; // Flag to track if this ability is active

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDenialAbility"/> class.
        /// </summary>
        /// <param name="abilityData">The data defining the area denial ability.</param>
        /// <param name="character">The character responsible for providing the target location</param>
        public AreaDenialAbility(AreaDenialAbilityData abilityData, Character character) :
            base(abilityData, character) {
            this.data = abilityData;
            this.character = character;

            this.abilityLayerIndex = character.animator.GetLayerIndex("Ability Layer");

            // Subscribe to the animation ready event
            character.OnCharacterReadyAbilityAnimation += this.SummonAoD;
        }

        void SummonAoD() {
            // Only cast if this ability is active
            if (!this.isActive) return;

            // Reset the flag after casting
            this.isActive = false;

            Vector3 targetPosition = this.character.GetAreaDenialLocation(this.data.Radius);
            targetPosition.y = 0;
            GameObject vfxInstance =
                Object.Instantiate(this.data.AreaDenialGameObject, targetPosition, Quaternion.identity);
            if (vfxInstance.TryGetComponent<AreaDenial>(out AreaDenial areaDenial)) {
                areaDenial.Damage = (int)(this.data.DamageScalar * this.character.Stats.Attack);
                areaDenial.Radius = this.data.Radius;
                areaDenial.TickRateSeconds = this.data.TickRateSeconds;
            }

            if (this.data.CastSFX != null) AudioManagerRuntime.PlaySFXAtPosition(this.data.CastSFX, targetPosition);

            Object.Destroy(vfxInstance, this.data.Duration);
        }

        /// <summary>
        /// Executes the buff ability by applying a stat modifier to the character's stats.
        /// </summary>
        public override void Execute() {
            if (!this.character.Resource.Use(this.AbilityData.ResourceCost)) return;

            this.character.Resource.Gain(this.AbilityData.ResourceGeneration);

            this.isActive = true;

            // Get attack speed and calculate animation speed multiplier
            float attackSpeed = this.character.Stats.AttackSpeed; // Use the AttackSpeed property from Stats
            float animationSpeedMultiplier = 1f / attackSpeed;
            this.character.animator.SetFloat("AttackSpeedMultiplier", animationSpeedMultiplier);

            // Play animation
            string stateName = this.data.AbilityAnimation.name;
            this.character.animator.Play(stateName, this.abilityLayerIndex);
            base.Execute();
        }
    }
}