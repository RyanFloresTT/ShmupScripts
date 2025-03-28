using _Project.Scripts.Items;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IHaveHealth, IVisitable {
    [SerializeField] LootTable lootTable;
    [SerializeField] int MaxHealth = 1;
    public Health Health { get; set; }

    void Start() {
        this.Health = new Health(this.MaxHealth, new StatsMediator(), new AnimationCurve());
        this.Health.OnEntityDeath += this.Handle_Death;
        this.lootTable.InitalizeLootTable();
    }

    void Handle_Death() {
        this.lootTable.SpawnDrops(this.transform);
        Destroy(this.gameObject);
    }

    public void Accept(IVisitor visitor) {
        visitor.Visit(this);
    }
}