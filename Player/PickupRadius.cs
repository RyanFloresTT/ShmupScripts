using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickupRadius : MonoBehaviour {
    [SerializeField] float radius;
    [SerializeField, Self] SphereCollider pickupCollider;
    const float Y_OFFSET = 1f;


    void Awake() {
        pickupCollider.radius = radius;
        pickupCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<MagnetizedItem>(out var pickup)) {
            pickup.StartMovingToPlayer(transform);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        var spherePos = new Vector3(transform.position.x, transform.position.y - Y_OFFSET, transform.position.z);
        Gizmos.DrawWireSphere(spherePos, radius);
    }
}
