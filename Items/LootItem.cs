using UnityEngine;

[System.Serializable]
public class LootItem {
    public GameObject Prefab;
    public int MinDrops;
    public int MaxDrops;
    [Range(0f, 1f)] public float DropChance;
}