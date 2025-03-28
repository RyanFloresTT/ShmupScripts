using UnityEngine;

public abstract class SwingTrigger : MonoBehaviour, IVisitor {
    public int Damage { get; set; }
    public float Length { get; set; }

    void OnEnable() {
            transform.localScale = new Vector3 (0.1f, 0.1f, Length);
    }

    void OnTriggerEnter(Collider other) {
        other.GetComponent<IVisitable>()?.Accept(this);
    }

    public abstract void Visit<T>(T visitable) where T : Component, IVisitable;
}