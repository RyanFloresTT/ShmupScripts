using UnityEngine;

public abstract class Pickup : MonoBehaviour, IVisitor {
    protected abstract void ApplyPickupEffect(Player entity);

    public void Visit<T>(T visitable) where T : Component, IVisitable {
        if (visitable is Player entity) {
            ApplyPickupEffect(entity);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other) {
        other.GetComponent<IVisitable>()?.Accept(this);

        Logger.Log($"{other.name} tried to pick up {name}", Logger.LogCategory.Pickups);
    }
}