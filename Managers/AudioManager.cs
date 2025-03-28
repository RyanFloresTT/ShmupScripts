using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Managers {
    [CreateAssetMenu(fileName = "AudioManager", menuName = "Managers/Audio Manager")]
    public class AudioManager : ScriptableObject {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        [SerializeField] AudioMixerGroup sfxMixerGroup;

        [Header("Volume Settings")] [Range(0f, 1f)]
        public float masterVolume = 1f;

        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        AudioSource musicSource;
        AudioSource musicSourceFadeTarget;
        List<AudioSource> activeAudioSources = new();
        GameObject audioSourcesContainer;

        const int MAX_AUDIO_SOURCES = 16; // Adjust based on your game's needs
        Queue<AudioSource> audioSourcePool = new();

        Dictionary<AudioClip, float> lastPlayedTime = new();
        const float MIN_TIME_BETWEEN_SAME_CLIPS = 0.05f;

        // Initialize the audio manager
        public void Initialize() {
            if (this.audioSourcesContainer == null) {
                this.audioSourcesContainer = new GameObject("Audio Sources");
                DontDestroyOnLoad(this.audioSourcesContainer);

                // Create music sources
                this.musicSource = this.CreateAudioSource("Music Source");
                this.musicSource.loop = true;
                this.musicSource.outputAudioMixerGroup = this.musicMixerGroup;

                this.musicSourceFadeTarget = this.CreateAudioSource("Music Source Fade Target");
                this.musicSourceFadeTarget.loop = true;
                this.musicSourceFadeTarget.outputAudioMixerGroup = this.musicMixerGroup;
                this.musicSourceFadeTarget.volume = 0f;

                // Initialize audio source pool
                for (int i = 0; i < MAX_AUDIO_SOURCES; i++) {
                    AudioSource source = this.CreateAudioSource($"SFX Source {i}");
                    source.outputAudioMixerGroup = this.sfxMixerGroup;
                    source.gameObject.SetActive(false);
                    this.audioSourcePool.Enqueue(source);
                }
            }

            this.ApplyVolumeSettings();
        }

        AudioSource CreateAudioSource(string name) {
            GameObject sourceObj = new(name);
            sourceObj.transform.SetParent(this.audioSourcesContainer.transform);
            return sourceObj.AddComponent<AudioSource>();
        }

        // Apply volume settings to mixer groups
        public void ApplyVolumeSettings() {
            // Apply master volume (example if using a mixer)
            // audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);

            // Apply volumes directly to sources for simplicity
            if (this.musicSource != null)
                this.musicSource.volume = this.musicVolume;

            if (this.musicSourceFadeTarget != null)
                this.musicSourceFadeTarget.volume = 0; // This is controlled by fading
        }

        #region Music Methods

        // Play background music with optional crossfade
        public void PlayMusic(AudioClip musicClip, float fadeTime = 0.5f) {
            if (musicClip == null) return;

            // If this is already the active clip, don't restart
            if (this.musicSource.clip == musicClip && this.musicSource.isPlaying)
                return;

            if (fadeTime <= 0) {
                // Direct play
                this.musicSource.clip = musicClip;
                this.musicSource.volume = this.musicVolume;
                this.musicSource.Play();
            }
            else
                // Start coroutine for crossfade
                this.audioSourcesContainer.MonoBehaviour()
                    .StartCoroutine(this.CrossfadeMusicCoroutine(musicClip, fadeTime));
        }

        IEnumerator CrossfadeMusicCoroutine(AudioClip musicClip, float fadeTime) {
            // Setup the new track on the fade target
            this.musicSourceFadeTarget.clip = musicClip;
            this.musicSourceFadeTarget.Play();

            // Crossfade between sources
            float startTime = Time.time;
            float endTime = startTime + fadeTime;

            while (Time.time < endTime) {
                float t = (Time.time - startTime) / fadeTime;
                this.musicSource.volume = this.musicVolume * (1f - t);
                this.musicSourceFadeTarget.volume = this.musicVolume * t;
                yield return null;
            }

            // Ensure the transition is complete
            this.musicSource.volume = 0f;
            this.musicSourceFadeTarget.volume = this.musicVolume;

            // Swap sources for next crossfade
            AudioSource temp = this.musicSource;
            this.musicSource = this.musicSourceFadeTarget;
            this.musicSourceFadeTarget = temp;

            // Reset fade target for next use
            this.musicSourceFadeTarget.Stop();
            this.musicSourceFadeTarget.volume = 0f;
        }

        public void StopMusic(float fadeTime = 0.5f) {
            if (fadeTime <= 0)
                this.musicSource.Stop();
            else
                this.audioSourcesContainer.MonoBehaviour().StartCoroutine(this.FadeOutMusicCoroutine(fadeTime));
        }

        IEnumerator FadeOutMusicCoroutine(float fadeTime) {
            float startVolume = this.musicSource.volume;
            float startTime = Time.time;
            float endTime = startTime + fadeTime;

            while (Time.time < endTime) {
                float t = (Time.time - startTime) / fadeTime;
                this.musicSource.volume = startVolume * (1f - t);
                yield return null;
            }

            this.musicSource.Stop();
            this.musicSource.volume = this.musicVolume; // Reset for next play
        }

        #endregion

        #region SFX Methods

        // Play a sound effect once
        public AudioSource PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f) {
            if (clip == null) return null;

            // Prevent rapid duplicate sound effects
            if (this.lastPlayedTime.ContainsKey(clip))
                if (Time.time - this.lastPlayedTime[clip] < MIN_TIME_BETWEEN_SAME_CLIPS)
                    return null;

            this.lastPlayedTime[clip] = Time.time;

            AudioSource source = this.GetAudioSourceFromPool();
            if (source == null) return null;

            source.clip = clip;
            source.volume = volume * this.sfxVolume;
            source.pitch = pitch;
            source.loop = false;
            source.Play();

            // Return to pool when done
            this.audioSourcesContainer.MonoBehaviour()
                .StartCoroutine(this.ReturnToPoolWhenFinished(source, clip.length / pitch));

            return source;
        }

        // Play a sound effect at a specific position in 3D space
        public AudioSource PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f,
            float spatialBlend = 1f, float maxDistance = 20f) {
            if (clip == null) return null;

            AudioSource source = this.GetAudioSourceFromPool();
            if (source == null) return null;

            source.transform.position = position;
            source.clip = clip;
            source.volume = volume * this.sfxVolume;
            source.pitch = pitch;
            source.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = maxDistance;
            source.loop = false;
            source.Play();

            // Return to pool when done
            this.audioSourcesContainer.MonoBehaviour()
                .StartCoroutine(this.ReturnToPoolWhenFinished(source, clip.length / pitch));

            return source;
        }

        // Play a looping sound effect (returns the source so you can stop it later)
        public AudioSource PlayLoopingSFX(AudioClip clip, float volume = 1f, float pitch = 1f) {
            if (clip == null) return null;

            AudioSource source = this.GetAudioSourceFromPool();
            if (source == null) return null;

            source.clip = clip;
            source.volume = volume * this.sfxVolume;
            source.pitch = pitch;
            source.loop = true;
            source.Play();

            return source;
        }

        // Stop a playing sound effect
        public void StopSFX(AudioSource source) {
            if (source != null && this.activeAudioSources.Contains(source)) {
                source.Stop();
                this.ReturnSourceToPool(source);
            }
        }

        AudioSource GetAudioSourceFromPool() {
            if (this.audioSourcePool.Count == 0) {
                // No available sources, try to reclaim the oldest one
                if (this.activeAudioSources.Count > 0) {
                    AudioSource oldest = this.activeAudioSources[0];
                    this.activeAudioSources.RemoveAt(0);
                    oldest.Stop();
                    return oldest;
                }

                return null;
            }

            AudioSource source = this.audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            this.activeAudioSources.Add(source);
            return source;
        }

        void ReturnSourceToPool(AudioSource source) {
            if (source == null) return;

            source.clip = null;
            source.gameObject.SetActive(false);
            this.activeAudioSources.Remove(source);
            this.audioSourcePool.Enqueue(source);
        }

        IEnumerator ReturnToPoolWhenFinished(AudioSource source, float delay) {
            yield return new WaitForSeconds(delay);

            if (source != null && !source.loop) this.ReturnSourceToPool(source);
        }

        #endregion
    }

// Extension method to access MonoBehaviour on GameObject
    public static class AudioExtensions {
        public static MonoBehaviour MonoBehaviour(this GameObject gameObject) {
            MonoBehaviour mb = gameObject.GetComponent<MonoBehaviour>();
            if (mb == null) mb = gameObject.AddComponent<AudioMonoBehaviourHelper>();
            return mb;
        }
    }

// Helper class for coroutines
    public class AudioMonoBehaviourHelper : MonoBehaviour { }
}