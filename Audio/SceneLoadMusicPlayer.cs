using _Project.Scripts.ScriptableObjects.AudioEvents;
using UnityEngine;

namespace _Project.Scripts.Audio {
    public class SceneLoadMusicPlayer : MonoBehaviour {
        [SerializeField] MusicAudioEvent musicEvent;

        void OnEnable() {
            this.musicEvent.Play();
        }
    }
}