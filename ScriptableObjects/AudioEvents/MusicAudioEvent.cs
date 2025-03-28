using _Project.Scripts.Managers;
using UnityEngine;

namespace _Project.Scripts.ScriptableObjects.AudioEvents {
    [CreateAssetMenu(fileName = "NewMusicAudioEvent", menuName = "Audio/Music Audio Event")]
    public class MusicAudioEvent : AudioEvent {
        [SerializeField] AudioClip musicClip;
        [SerializeField] float fadeTime = 1.0f;

        public override void Play() {
            if (this.musicClip == null) return;
            AudioManagerRuntime.PlayMusic(this.musicClip, this.fadeTime);
        }

        public override void Play(Vector3 position) {
            this.Play();
        }

        public void Stop() {
            AudioManagerRuntime.StopMusic(this.fadeTime);
        }
    }
}