using UnityEngine;

public struct OnXpDrop : IEvent {
    public Transform DropLocation { get ; private set; }
    public OnXpDrop (Transform dropSpot) {
        DropLocation = dropSpot;
    }
}