using _Project.Scripts.AbilitySystem;

/// <summary>
/// Event triggered when a projectile times out.
/// </summary>
public struct OnProjectileTimeout : IEvent {
    /// <summary>
    /// The projectile that timed out.
    /// </summary>
    public Projectile Projectile;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnProjectileTimeout"/> struct.
    /// </summary>
    /// <param name="projectile">The projectile that timed out.</param>
    public OnProjectileTimeout(Projectile projectile) {
        Projectile = projectile;
    }
}
