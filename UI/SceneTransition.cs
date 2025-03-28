using Cysharp.Threading.Tasks;
using KBCore.Refs;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.UI {
    public class SceneTransition : MonoBehaviour {
        [SerializeField] [Self] CanvasGroup canvasGroup;

        public static SceneTransition Instance { get; private set; }

        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) Destroy(this.gameObject);
        }

        async UniTask FadeInAsync(float durationMs) {
            this.canvasGroup.blocksRaycasts = true;
            await Tween.Alpha(this.canvasGroup, 1, durationMs / 1000f, Ease.Linear).ToUniTask();
        }

        async UniTask FadeOutAsync(float durationMs) {
            this.canvasGroup.blocksRaycasts = false;
            await Tween.Alpha(this.canvasGroup, 0, durationMs / 1000f, Ease.Linear).ToUniTask();
        }


        public async UniTask ChangeSceneWithTransition(string sceneName, float transitionDurationMs = 200f) {
            await this.FadeInAsync(transitionDurationMs);

            await SceneManager.LoadSceneAsync(sceneName);

            await this.FadeOutAsync(transitionDurationMs);
        }
    }
}