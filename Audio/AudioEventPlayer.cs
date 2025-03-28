using _Project.Scripts.ScriptableObjects.AudioEvents;
using UnityEngine;

namespace _Project.Scripts.Audio {
    public class AudioEventPlayer : MonoBehaviour {
        [SerializeField] AudioEvent audioEvent;
        [SerializeField] bool playOnStart = false;
        [SerializeField] bool is3DAudio = false;

        void Start() {
            if (this.playOnStart && this.audioEvent != null) this.PlayAudio();
        }

        void PlayAudio() {
            if (this.audioEvent == null) return;

            if (this.is3DAudio)
                this.audioEvent.Play(this.transform.position);
            else
                this.audioEvent.Play();
        }

        public void StopAudio() {
            switch (this.audioEvent) {
                case SimpleAudioEvent simpleAudio:
                    simpleAudio.Stop();
                    break;
                case RandomAudioEvent randomAudio:
                    randomAudio.Stop();
                    break;
                case MusicAudioEvent musicAudio:
                    musicAudio.Stop();
                    break;
            }
        }
    }
}