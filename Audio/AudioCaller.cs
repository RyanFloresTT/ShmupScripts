using _Project.Scripts.Managers;
using UnityEngine;

namespace _Project.Scripts.Audio {
    /// <summary>
    /// Static utility class to easily play audio from anywhere in code
    /// </summary>
    public static class AudioCaller {
        // Play simple sound effects
        public static void PlaySFX(AudioClip clip) {
            if (clip != null) AudioManagerRuntime.PlaySFX(clip);
        }

        public static void PlaySFX(AudioClip clip, float volume, float pitch = 1f) {
            if (clip != null)
                AudioManagerRuntime.PlaySFX(clip, volume, pitch);
        }

        public static void PlaySFXAtPosition(AudioClip clip, Vector3 position) {
            if (clip != null)
                AudioManagerRuntime.PlaySFXAtPosition(clip, position);
        }

        public static void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume, float pitch = 1f) {
            if (clip != null)
                AudioManagerRuntime.PlaySFXAtPosition(clip, position, volume, pitch);
        }

        // Play audio events 
        public static void PlayAudioEvent(AudioEvent audioEvent) {
            if (audioEvent != null)
                audioEvent.Play();
        }

        public static void PlayAudioEventAtPosition(AudioEvent audioEvent, Vector3 position) {
            if (audioEvent != null)
                audioEvent.Play(position);
        }

        // Play background music
        public static void PlayMusic(AudioClip musicClip, float fadeTime = 0.5f) {
            if (musicClip != null)
                AudioManagerRuntime.PlayMusic(musicClip, fadeTime);
        }

        public static void StopMusic(float fadeTime = 0.5f) {
            AudioManagerRuntime.StopMusic(fadeTime);
        }
    }
}