using _Project.Scripts.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSimpleAudioEvent", menuName = "Audio/Simple Audio Event")]
public class SimpleAudioEvent : AudioEvent {
    [SerializeField] AudioClip clip;

    [Header("Volume")] [SerializeField] [Range(0f, 1f)]
    float volume = 1f;

    [SerializeField] [Range(0f, 0.5f)] float volumeVariation = 0f;

    [Header("Pitch")] [SerializeField] [Range(0.1f, 3f)]
    float pitch = 1f;

    [SerializeField] [Range(0f, 0.5f)] float pitchVariation = 0f;

    [Header("Spatial Settings")] [SerializeField] [Range(0f, 1f)]
    float spatialBlend = 0f;

    [SerializeField] float maxDistance = 20f;

    AudioSource lastSource = null;

    public override void Play() {
        if (this.clip == null) return;

        float finalVolume = this.volume + Random.Range(-this.volumeVariation, this.volumeVariation);
        float finalPitch = this.pitch + Random.Range(-this.pitchVariation, this.pitchVariation);

        this.lastSource = AudioManagerRuntime.PlaySFX(this.clip, finalVolume, finalPitch);
    }

    public override void Play(Vector3 position) {
        if (this.clip == null) return;

        float finalVolume = this.volume + Random.Range(-this.volumeVariation, this.volumeVariation);
        float finalPitch = this.pitch + Random.Range(-this.pitchVariation, this.pitchVariation);

        this.lastSource = AudioManagerRuntime.PlaySFXAtPosition(this.clip, position, finalVolume, finalPitch,
            this.spatialBlend, this.maxDistance);
    }

    public void Stop() {
        if (this.lastSource != null) {
            AudioManagerRuntime.StopSFX(this.lastSource);
            this.lastSource = null;
        }
    }
}