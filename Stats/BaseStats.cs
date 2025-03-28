using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : ScriptableObject {
    public int Attack = 10;
    public int Defense = 20;
    public int MoveSpeed = 30;
    public float AttackSpeed = 1f;
}