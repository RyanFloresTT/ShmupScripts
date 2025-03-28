using _Project.Scripts.EventBus;
using UnityEngine;

namespace _Project.Scripts.UI {
    public class HUDDeathListener : MonoBehaviour {
        EventBinding<OnPlayerDeath> onPlayerDeath;

        void Start() {
            this.onPlayerDeath = new EventBinding<OnPlayerDeath>(this.Handle_PlayerDeath);
            EventBus<OnPlayerDeath>.Register(this.onPlayerDeath);
        }

        void OnDisable() {
            EventBus<OnPlayerDeath>.Deregister(this.onPlayerDeath);
        }

        void Handle_PlayerDeath() {
            this.gameObject.SetActive(false);
        }
    }
}