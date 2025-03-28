using _Project.Scripts.AbilitySystem;

public class EnemyProjectile : Projectile {
    public override void Visit<T>(T visitable) {
        base.Visit(visitable);
        if (visitable is Player player) {
            player.Health.TakeDamage(Damage);
        }
    }
}
