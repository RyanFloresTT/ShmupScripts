using UnityEngine;

public class HealthPotion : Pickup {
    [SerializeField, Range(0.01f, 1f)] float healPercentage;

    protected override void ApplyPickupEffect(Player entity) {
        entity.Health.Heal(entity.Health.MaxHealth * healPercentage);
    }
}
