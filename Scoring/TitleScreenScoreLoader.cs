using Saving;
using TMPro;
using UnityEngine;

public class TitleScreenScoreLoader : MonoBehaviour  {
    [SerializeField] SaveManager saveManager;
    [SerializeField] TextMeshProUGUI scoreText;
    void Start() {
        if (saveManager.SaveData == null) return;
        scoreText.text = saveManager.SaveData.WaveScore.ToString();
    }
}
