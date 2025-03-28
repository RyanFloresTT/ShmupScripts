using _Project.Scripts.EventBus;
using Saving;
using TMPro;
using UnityEngine;

namespace _Project.Prefabs.Scoring {
    public class ScoreManager : MonoBehaviour {
        [SerializeField] SaveManager saveManager;
        [SerializeField] TextMeshProUGUI scoreText;

        EventBinding<OnNextWave> onNextWaveBinding;

        int maxWave;

        void Awake() {
            this.onNextWaveBinding = new EventBinding<OnNextWave>(this.Handle_NextWave);
            this.maxWave = this.saveManager.SaveData.WaveScore;
            this.scoreText.text = "0";
        }

        void OnEnable() {
            EventBus<OnNextWave>.Register(this.onNextWaveBinding);
        }

        void OnDisable() {
            EventBus<OnNextWave>.Deregister(this.onNextWaveBinding);
        }

        void Handle_NextWave(OnNextWave obj) {
            this.scoreText.text = obj.Wave.ToString();

            if (obj.Wave <= this.maxWave) return;
            this.saveManager.UpdateSingleField(SaveField.WaveScore, obj.Wave);
            this.saveManager.SaveGameData();
        }
    }
}