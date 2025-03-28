using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Project.Scripts.Managers;
using _Project.Scripts.Player;
using UnityEngine;
using UnityServiceLocator;

namespace _Project.Scripts.AbilitySystem {
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Projectile : MonoBehaviour, IVisitor {
        /// <summary>
        /// Layers that the projectile should interact with.
        /// </summary>
        [SerializeField] LayerMask targetLayers;

        /// <summary>
        /// Gets the speed of the projectile.
        /// </summary>
        float Speed { get; set; }

        /// <summary>
        /// Gets the direction of the projectile.
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// Gets the damage dealt by the projectile.
        /// </summary>
        protected int Damage { get; private set; }

        /// <summary>
        /// Gets the name of the projectile.
        /// </summary>
        public string ProjectileName { get; private set; }

        /// <summary>
        /// Gets the cancellation token source used for returning the projectile to the pool.
        /// </summary>
        public CancellationTokenSource ReturnTokenSource { get; private set; }

        /// <summary>
        /// Gets or sets the projectile manager responsible for pooling.
        /// </summary>
        public GameObjectPoolManager ProjectileManager { get; set; }

        /// <summary>
        /// A list of modifiers that will be applied when this projectile hits a valid target.
        /// </summary>
        public List<StatModifierData> Modifiers { get; set; }

        [SerializeField] GameObject ExplosionPrefab;

        AudioClip explosionSFX;

        /// <summary>
        /// The rigidbody component attached to the projectile.
        /// </summary>
        Rigidbody rb;


        bool isPiercing;

        /// <summary>
        /// Initializes the projectile with the specified direction, speed, damage, and name.
        /// </summary>
        /// <param name="direction">The direction of the projectile.</param>
        /// <param name="speed">The speed of the projectile.</param>
        /// <param name="damage">The damage dealt by the projectile.</param>
        /// <param name="projectileName">The name of the projectile.</param>
        public void Initialize(Vector3 direction, float speed, int damage, string projectileName, bool isPiercing,
            AudioClip explosionSFX,
            List<StatModifierData> modifiers = null) {
            this.Direction = direction;
            this.Speed = speed;
            this.Damage = damage;
            this.isPiercing = isPiercing;
            this.ProjectileName = projectileName;
            this.ReturnTokenSource = new CancellationTokenSource();
            this.Modifiers = modifiers;
            this.explosionSFX = explosionSFX;

            this.GetComponent<SphereCollider>().isTrigger = true;
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            this.rb = this.GetComponent<Rigidbody>();
            this.ReturnTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Called when the object becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            if (this.ReturnTokenSource == null) return;
            this.ReturnTokenSource.Cancel();
            this.ReturnTokenSource.Dispose();
        }

        /// <summary>
        /// Called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            if (this.rb != null) this.rb.linearVelocity = this.Direction.normalized * this.Speed;
        }

        /// <summary>
        /// Called when the projectile enters a trigger collider.
        /// </summary>
        /// <param name="other">The collider that the projectile entered.</param>
        void OnTriggerEnter(Collider other) {
            if ((this.targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

            if (other.TryGetComponent(out IHaveHealth healthObject))
                healthObject.Health.TakeDamage(this.Damage);

            IVisitable visitable = other.GetComponent<IVisitable>();
            visitable?.Accept(this);

            if (this.isPiercing) return;
            if (this.ExplosionPrefab != null)
                Instantiate(this.ExplosionPrefab, this.transform.position, Quaternion.identity);

            if (this.explosionSFX != null)
                AudioManagerRuntime.PlaySFXAtPosition(this.explosionSFX, this.transform.position);

            this.ProjectileManager.Return(this.ProjectileName, this.gameObject);
        }

        /// <summary>
        /// Visits the specified visitable object.
        /// </summary>
        /// <typeparam name="T">The type of the visitable object.</typeparam>
        /// <param name="visitable">The visitable object.</param>
        public virtual void Visit<T>(T visitable) where T : Component, IVisitable {
            if (!visitable.TryGetComponent(out Character entity)) return;
            if (this.Modifiers.Count <= 0 || this.Modifiers == null) return;
            foreach (StatModifier statModifier in this.Modifiers.Select(modifier => ServiceLocator.For(this)
                         .Get<IStatModifierFactory>()
                         .Create(modifier.OperatorType, modifier.Type, modifier.Value, modifier.Duration, modifier.VFX,
                             entity)))
                entity.Stats.Mediator.AddModifier(statModifier);
        }
    }
}