using _Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewRandomAudioEvent", menuName = "Audio/Random Audio Event")]
public class RandomAudioEvent : AudioEvent {
    [SerializeField] AudioClip[] clips;

    [Header("Volume")] [SerializeField] [Range(0f, 1f)]
    float volume = 1f;

    [SerializeField] [Range(0f, 0.5f)] float volumeVariation = 0f;

    [Header("Pitch")] [SerializeField] [Range(0.1f, 3f)]
    float pitch = 1f;

    [SerializeField] [Range(0f, 0.5f)] float pitchVariation = 0f;

    [Header("Spatial Settings")] [SerializeField] [Range(0f, 1f)]
    float spatialBlend = 0f;

    [SerializeField] float maxDistance = 20f;

    [Header("Selection Settings")] [SerializeField]
    bool avoidRepeat = true;

    int lastPlayedIndex = -1;
    AudioSource lastSource = null;

    public override void Play() {
        if (this.clips == null || this.clips.Length == 0) return;

        AudioClip selectedClip = this.GetRandomClip();
        if (selectedClip == null) return;

        float finalVolume = this.volume + Random.Range(-this.volumeVariation, this.volumeVariation);
        float finalPitch = this.pitch + Random.Range(-this.pitchVariation, this.pitchVariation);

        this.lastSource = AudioManagerRuntime.PlaySFX(selectedClip, finalVolume, finalPitch);
    }

    public override void Play(Vector3 position) {
        if (this.clips == null || this.clips.Length == 0) return;

        AudioClip selectedClip = this.GetRandomClip();
        if (selectedClip == null) return;

        float finalVolume = this.volume + Random.Range(-this.volumeVariation, this.volumeVariation);
        float finalPitch = this.pitch + Random.Range(-this.pitchVariation, this.pitchVariation);

        this.lastSource = AudioManagerRuntime.PlaySFXAtPosition(selectedClip, position, finalVolume, finalPitch,
            this.spatialBlend, this.maxDistance);
    }

    AudioClip GetRandomClip() {
        if (this.clips.Length == 1)
            return this.clips[0];

        int index;
        if (this.avoidRepeat && this.clips.Length > 1) {
            // Avoid playing the same clip twice in a row
            index = Random.Range(0, this.clips.Length - 1);
            if (index >= this.lastPlayedIndex)
                index++;
        }
        else
            index = Random.Range(0, this.clips.Length);

        this.lastPlayedIndex = index;
        return this.clips[index];
    }

    public void Stop() {
        if (this.lastSource != null) {
            AudioManagerRuntime.StopSFX(this.lastSource);
            this.lastSource = null;
        }
    }
}