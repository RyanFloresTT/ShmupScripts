using System.Collections.Generic;
using _Project.Scripts.EventBus;
using UnityEngine;

namespace _Project.Scripts.Items {
    [CreateAssetMenu(fileName = "Loot Tables/New Loot Table", menuName = "Loot Table")]
    public class LootTable : ScriptableObject {
        [SerializeField] List<LootItem> lootItems;
        [SerializeField] float dropRadius = 1.0f;

        public void InitalizeLootTable() {
            foreach (LootItem lootItem in this.lootItems)
                EventBus<OnItemRegister>.Raise(new OnItemRegister() { Item = lootItem.Prefab });
        }

        public void SpawnDrops(Transform dropLocation) {
            foreach (LootItem item in this.lootItems)
                for (int i = 0; i < item.MinDrops; i++) {
                    float randomValue = Random.value;

                    if (randomValue <= item.DropChance) {
                        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(0f, this.dropRadius);
                        Vector3 spawnPosition = dropLocation.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
                        EventBus<OnItemDropped>.Raise(new OnItemDropped()
                            { Item = item.Prefab, Position = spawnPosition });
                    }
                }
        }
    }
}