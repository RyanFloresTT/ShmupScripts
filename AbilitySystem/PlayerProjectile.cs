using _Project.Scripts.AbilitySystem;

public class PlayerProjectile : Projectile {
    public override void Visit<T>(T visitable) {
        base.Visit(visitable);
        if (visitable is IHaveHealth entity) {
            if (visitable.GetComponent<Player>() != null) { return; }
            entity.Health.TakeDamage(Damage);
        }
    }
}
