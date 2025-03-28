using UnityEngine;

public enum OperatorType { Add, Multiply }
[CreateAssetMenu(fileName = "Stat Modifier Data", menuName = "Stats/Stat Modifer Data")]
public class StatModifierData : ScriptableObject
{
    public StatType Type = StatType.Attack;
    public OperatorType OperatorType = OperatorType.Add;
    public float Value = 10;
    public float Duration = 5f;
    public GameObject VFX;
}
