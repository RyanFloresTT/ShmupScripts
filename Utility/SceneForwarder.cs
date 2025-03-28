using System;
using _Project.Scripts.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Utility {
    public class SceneForwarder : MonoBehaviour {
        [SerializeField] SceneControllerSo sceneController;
        [SerializeField] GameScene targetScene;
        [SerializeField] bool autoStart;

        async void Start() {
            try {
                if (!this.autoStart) return;
                await this.sceneController.ChangeScene(this.targetScene);
            }
            catch (Exception e) {
                Debug.LogError($"Something went wrong when trying to change scene: {e.Message}");
            }
        }

        public async UniTaskVoid ForwardSceneAsync() {
            await this.sceneController.ChangeScene(this.targetScene);
        }

        public void ForwardScene() {
            _ = this.sceneController.ChangeScene(this.targetScene);
        }
    }
}