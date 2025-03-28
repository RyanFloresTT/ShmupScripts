using KBCore.Refs;
using UnityEngine;

namespace _Project.Scripts.Player {
    public class AnimationManager : MonoBehaviour {
        [SerializeField] GameObject levelUpVFX;
        [SerializeField] [Self] Character character;
        readonly int timeoutDurationSec = 2;

        void OnEnable() {
            this.character.OnStatsInitialized += () => {
                this.character.XP.OnEntityLevelUp += this.Handle_EntityLevelUp;
            };
        }

        void OnDisable() {
            this.character.OnStatsInitialized -= () => {
                this.character.XP.OnEntityLevelUp -= this.Handle_EntityLevelUp;
            };
        }

        /// <summary>
        /// Handles animation(s) when the entity levels up.
        /// </summary>
        void Handle_EntityLevelUp(int level) {
            GameObject levelUpVFXInstance = Instantiate(this.levelUpVFX, this.character.transform);
            Destroy(levelUpVFXInstance, this.timeoutDurationSec);
        }
    }
}