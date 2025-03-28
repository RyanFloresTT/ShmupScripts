using _Project.Scripts.EventBus;
using UnityEngine;

public class UpgradeStationTrigger : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        EventBus<OnPlayerEnteredStore>.Raise(new OnPlayerEnteredStore(true));
    }

    void OnTriggerExit(Collider other) {
        EventBus<OnPlayerEnteredStore>.Raise(new OnPlayerEnteredStore(false));
    }
}