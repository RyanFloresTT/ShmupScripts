using _Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------


public class HUDLoader : MonoBehaviour {
    [SerializeField] SceneControllerSo sceneController;
    [SerializeField] GameScene _hudScene = GameScene.HUD;

    void Awake() {
        string hudSceneName = this.sceneController.GameSceneToString(this._hudScene);
        SceneManager.LoadSceneAsync(hudSceneName, LoadSceneMode.Additive);
    }
}