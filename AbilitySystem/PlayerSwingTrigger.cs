using UnityEngine;

public class PlayerSwingTrigger : SwingTrigger, IVisitor {
    void OnTriggerEnter(Collider other) {
        other.GetComponent<IVisitable>()?.Accept(this);
    }

    public override void Visit<T>(T visitable) {
        if (visitable is IHaveHealth entity) {
            if (visitable.GetComponent<Player>() != null) { return; }
            entity.Health.TakeDamage(Damage);
        }
    }
}