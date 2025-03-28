using System;
using _Project.Scripts.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.UI {
    public class SplashUIManager : MonoBehaviour {
        [SerializeField] int displayDurationMS = 2000;
        [SerializeField] GameScene nextScene;
        [SerializeField] SceneControllerSo sceneController;

        async void Start() {
            try {
                // Show splash
                await UniTask.Delay(TimeSpan.FromMilliseconds(this.displayDurationMS));

                // Change scene
                await this.sceneController.ChangeScene(this.nextScene);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}