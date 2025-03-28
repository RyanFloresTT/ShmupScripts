using System;
using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Managers {
    public enum GameScene {
        Loader,
        Splash,
        Robbys,
        Title,
        Ryans,
        Upgrade,
        HUD,
        ClassSelection,
        DefaultLevel
    }

    [CreateAssetMenu(fileName = "SceneController", menuName = "ScriptableObjects/Managers/SceneController")]
    public class SceneControllerSo : ScriptableObject {
        [SerializeField] int delayDurationMS = 200;

        GameScene ActiveScene { get; set; } = GameScene.Loader;

        public async UniTask ChangeScene(GameScene newScene) {
            await SceneTransition.Instance.ChangeSceneWithTransition(this.GameSceneToString(newScene),
                this.delayDurationMS);
        }

        public string GameSceneToString(GameScene scene) {
            return scene switch {
                GameScene.Loader => "Loader",
                GameScene.Splash => "Splash",
                GameScene.Robbys => "Robbys",
                GameScene.Title => "TitleMenu",
                GameScene.Ryans => "Ryans",
                GameScene.Upgrade => "Upgrade",
                GameScene.HUD => "HUD",
                GameScene.ClassSelection => "ClassSelection",
                GameScene.DefaultLevel => "DefaultLevel",
                _ => null
            };
        }
    }
}