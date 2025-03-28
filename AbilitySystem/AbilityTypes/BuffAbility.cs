using _Project.Scripts.Player;
using UnityEngine;
using UnityServiceLocator;

namespace AbilitySystem {
    /// <summary>
    /// Represents a buff ability that applies a stat modifier to a character.
    /// </summary>
    public class BuffAbility : Ability<BuffAbilityData> {
        readonly BuffAbilityData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuffAbility"/> class.
        /// </summary>
        /// <param name="abilityData">The data defining the buff ability.</param>
        /// <param name="character">The character to which the buff ability is applied.</param>
        public BuffAbility(BuffAbilityData abilityData, Character character) : base(abilityData, character) {
            this.data = abilityData;
            this.character = character;
        }

        /// <summary>
        /// Executes the buff ability by applying a stat modifier to the character's stats.
        /// </summary>
        public override void Execute() {
            if (!this.character.Resource.Use(this.AbilityData.ResourceCost)) return;

            this.character.Resource.Gain(this.AbilityData.ResourceGeneration);

            base.Execute();
            StatModifier modifier = ServiceLocator.For(this.character).Get<IStatModifierFactory>()
                .Create(this.data.OperatorType, this.data.StatType, this.data.BuffAmount, this.data.DurationSecs);
            this.character.Stats.Mediator.AddModifier(modifier);
        }
    }
}