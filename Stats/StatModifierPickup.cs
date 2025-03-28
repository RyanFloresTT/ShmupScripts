using UnityEngine;
using UnityServiceLocator;
public class StatModifierPickup : Pickup {

    [SerializeField] StatModifierData statData;

    protected override void ApplyPickupEffect(Player entity) {
        StatModifier modifier = ServiceLocator.For(this).Get<IStatModifierFactory>().Create(statData.OperatorType, statData.Type, statData.Value, statData.Duration, statData.VFX, entity);
        entity.Stats.Mediator.AddModifier(modifier);
    }
}