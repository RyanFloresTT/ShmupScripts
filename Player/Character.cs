using System;
using _Project.Scripts.ScriptableObjects.Classes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Player {
    /// <summary>
    /// Base class representing a character in the game with health, stats, and resources.
    /// </summary>
    public abstract class Character : MonoBehaviour, IHaveHealth, IVisitable {
        static readonly int MoveSpeedMultiplier = Animator.StringToHash("MoveSpeedMultiplier");
        [SerializeField] protected ResourceData resourceData;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected AnimationCurve levelCurve;
        public Transform rightHand;
        [Required] public Animator animator;
        public AnimationClip hitAnimation;
        public ClassData ClassData;

        /// <summary>
        /// Experience points of the character.
        /// </summary>
        public Experience XP { get; set; }

        /// <summary>
        /// Stats of the character.
        /// </summary>
        public Stats Stats { get; set; }

        /// <summary>
        /// Health of the character.
        /// </summary>
        public Health Health { get; set; }

        /// <summary>
        /// Resource (e.g., mana) of the character.
        /// </summary>
        public Resource Resource { get; set; }

        /// <summary>
        /// Action invoked when character pools are initialized.
        /// </summary>
        public Action OnPoolsInitialized { get; set; }

        /// <summary>
        /// Action invoked after the character's stats are initialized.
        /// </summary>
        public Action OnStatsInitialized { get; set; }

        /// <summary>
        /// Manager for projectile pools associated with the character.
        /// </summary>
        public GameObjectPoolManager ProjectilePools { get; protected set; }

        /// <summary>
        /// Event invoked when the character's ability animation reaches the point where something should happen, i.e. a projectile to be spawned at a peak.
        /// </summary>
        public event Action OnCharacterReadyAbilityAnimation;

        /// <summary>
        /// The direction of the Character's current Target.
        /// </summary>
        public Vector3 TargetDirection { get; set; }

        int damageLayerIndex;

        /// <summary>
        /// Updates the mediators for health, stats, and resource.
        /// </summary>
        protected void UpdateMediators() {
            this.Health.Mediator.Update(Time.deltaTime);
            this.Stats.Mediator.Update(Time.deltaTime);
            this.Resource.Mediator.Update(Time.deltaTime);
            this.Resource.UpdateRegenTimer(Time.deltaTime);
        }

        /// <summary>
        /// Initializes the character's stats during Awake phase.
        /// </summary>
        public virtual void Awake() {
            this.SetStats();
        }

        /// <summary>
        /// Registers the death handler on Start.
        /// </summary>
        void Start() {
            this.Health.OnEntityDeath += this.Handle_Death;
            this.Stats.Mediator.OnStatModified += this.Handle_StatModified;
            this.Health.OnEntityHealthChanged += this.OnEntityHealthChanged;
            this.damageLayerIndex = this.animator.GetLayerIndex("Damage Layer");
        }

        void OnEntityHealthChanged(int changedHealth) {
            if (changedHealth < 0) // taking damage
                this.animator.Play(this.hitAnimation.name, this.damageLayerIndex);
        }

        void Handle_StatModified(StatType statType) {
            Debug.Log(statType);
            switch (statType) {
                case StatType.AttackSpeed:
                    float speedMultiplier = this.Stats.MoveSpeed / this.ClassData.ClassBaseStats.MoveSpeed;
                    this.animator.SetFloat(MoveSpeedMultiplier, speedMultiplier);
                    break;
                case StatType.Attack:
                    break;
                case StatType.MoveSpeed:
                    Debug.Log("Moving Speed");
                    break;
                case StatType.Defense:
                case StatType.ResourceAmount:
                case StatType.ResourceRate:
                case StatType.ResourceMax:
                case StatType.Health:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
            }
        }

        /// <summary>
        /// Sets the initial stats of the character.
        /// </summary>
        protected void SetStats() {
            this.Health = new Health(this.maxHealth, new StatsMediator(), this.levelCurve);
            this.Stats = new Stats(new StatsMediator(), this.ClassData.ClassBaseStats, this.levelCurve);
            this.Resource = new Resource(new StatsMediator(), this.resourceData, this.levelCurve);
            this.XP = new Experience();
            this.Resource?.SetUpTimer();
            this.OnStatsInitialized?.Invoke();
        }

        /// <summary>
        /// Accepts a visitor for visitation.
        /// </summary>
        /// <param name="visitor">The visitor to accept.</param>
        public void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        /// <summary>
        /// Provides the location for Area Denial abilities.
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 GetAreaDenialLocation(float radius);

        /// <summary>
        /// Abstract method to handle character death.
        /// </summary>
        protected virtual void Handle_Death() {
        }

        /// <summary>
        /// Invokes the <see cref="OnCharacterReadyAbilityAnimation"/> event to signal that the ability animation is ready.
        /// </summary>
        public void AbilityReady() {
            this.OnCharacterReadyAbilityAnimation?.Invoke();
        }
    }
}