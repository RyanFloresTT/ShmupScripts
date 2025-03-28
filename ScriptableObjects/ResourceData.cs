using UnityEngine;

[CreateAssetMenu(menuName = "Resource/New Resource Data")]
public class ResourceData : ScriptableObject {
    public string ResourceName;
    public int StartingAmount;
    public int MaxAmount;
    public bool RegenerationEnabled;
    public float RegenAmount;
    public float RegenRate;
}
