using UnityEngine;

namespace _Project.Scripts.Managers {
    public class AudioManagerRuntime : MonoBehaviour {
        [SerializeField] AudioManager audioManager;

        static AudioManagerRuntime instance;

        void Awake() {
            // Singleton pattern
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            if (this.audioManager != null)
                this.audioManager.Initialize();
            else
                Debug.LogError("AudioManager not assigned to AudioManagerRuntime!");
        }

        // Static access methods that forward to our ScriptableObject

        public static void PlayMusic(AudioClip clip, float fadeTime = 0.5f) {
            if (!IsValid()) return;
            instance.audioManager.PlayMusic(clip, fadeTime);
        }

        public static void StopMusic(float fadeTime = 0.5f) {
            if (IsValid())
                instance.audioManager.StopMusic(fadeTime);
        }

        public static AudioSource PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f) {
            return IsValid() ? instance.audioManager.PlaySFX(clip, volume, pitch) : null;
        }

        public static AudioSource PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f,
            float pitch = 1f, float spatialBlend = 1f, float maxDistance = 20f) {
            if (IsValid())
                return instance.audioManager.PlaySFXAtPosition(clip, position, volume, pitch, spatialBlend,
                    maxDistance);
            return null;
        }

        public static AudioSource PlayLoopingSFX(AudioClip clip, float volume = 1f, float pitch = 1f) {
            return IsValid() ? instance.audioManager.PlayLoopingSFX(clip, volume, pitch) : null;
        }

        public static void StopSFX(AudioSource source) {
            if (IsValid() && source != null)
                instance.audioManager.StopSFX(source);
        }

        // Volume control
        public static void SetMasterVolume(float volume) {
            if (!IsValid()) return;
            instance.audioManager.masterVolume = Mathf.Clamp01(volume);
            instance.audioManager.ApplyVolumeSettings();
        }

        public static void SetMusicVolume(float volume) {
            if (!IsValid()) return;
            instance.audioManager.musicVolume = Mathf.Clamp01(volume);
            instance.audioManager.ApplyVolumeSettings();
        }

        public static void SetSFXVolume(float volume) {
            if (!IsValid()) return;
            instance.audioManager.sfxVolume = Mathf.Clamp01(volume);
            instance.audioManager.ApplyVolumeSettings();
        }

        static bool IsValid() {
            return instance != null && instance.audioManager != null;
        }
    }
}