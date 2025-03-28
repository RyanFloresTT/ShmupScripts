using _Project.Scripts.Player;
using AbilitySystem;
using PrimeTween;
using UnityEngine;

/// <summary>
/// Represents a melee swing ability that triggers a melee attack with a specific swing animation and damage.
/// </summary>
public class MeleeSwingAbility : Ability<MeleeSwingAbilityData> {
    readonly MeleeSwingAbilityData data;
    readonly GameObject swingTrigger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeleeSwingAbility"/> class.
    /// </summary>
    /// <param name="abilityData">The data defining the melee swing ability.</param>
    /// <param name="character">The character associated with this ability.</param>
    public MeleeSwingAbility(MeleeSwingAbilityData abilityData, Character character) : base(abilityData, character) {
        this.data = abilityData;
        this.character = character;

        this.swingTrigger = GameObject.Instantiate(this.data.SwingTrigger, character.transform);

        this.swingTrigger.SetActive(false);
    }

    /// <summary>
    /// Executes the melee swing ability, triggering a melee attack with animation and damage.
    /// </summary>
    public override void Execute() {
        if (!this.character.Resource.Use(this.AbilityData.ResourceCost)) return;

        this.character.Resource.Gain(this.AbilityData.ResourceGeneration);

        base.Execute();

        if (this.swingTrigger.activeSelf) return;

        SwingTrigger swing = this.swingTrigger.GetComponentInChildren<SwingTrigger>();
        swing.Length = this.data.SwingLength;
        swing.Damage = (int)(this.data.DamageScalar * this.character.Stats.Attack);

        Transform model = this.character.transform.Find("Model");

        Quaternion startRotation = Quaternion.Euler(0f, model.eulerAngles.y + 22.5f, 0f);
        this.swingTrigger.transform.rotation = startRotation;

        float halfLength = this.data.SwingLength / 2f;
        Vector3 spawnOffset = startRotation * Vector3.forward * halfLength;
        this.swingTrigger.transform.position = model.position + spawnOffset;

        this.swingTrigger.transform.localPosition = new Vector3(0f, 1f, 0f);

        this.swingTrigger.SetActive(true);

        Quaternion endRotation = Quaternion.Euler(0f, startRotation.eulerAngles.y - 45f, 0f);

        Tween.LocalRotationAtSpeed(this.swingTrigger.transform, endRotation, this.data.Speed, Ease.Linear)
            .OnComplete(() => this.swingTrigger.SetActive(false));
    }
}