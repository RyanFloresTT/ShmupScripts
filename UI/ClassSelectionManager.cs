using _Project.Scripts.EventBus;
using _Project.Scripts.ScriptableObjects.Classes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.UI {
    public class ClassSelectionManager : MonoBehaviour {
        public static ClassSelectionManager Instance;

        public ClassData SelectedClass { get; set; }
        GameObject spawnedCharacter;

        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                SceneManager.sceneLoaded += this.OnSceneLoaded;
            }
            else
                Destroy(this.gameObject);
        }

        void OnDestroy() {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            switch (scene.name) {
                case "HUD":
                    return;
                case "DefaultLevel" when this.SelectedClass != null: {
                    if (this.spawnedCharacter == null)
                        UniTask.Delay(100).GetAwaiter().OnCompleted(
                            this.SpawnCharacter);
                    else
                        Debug.Log("Character already spawned. Skipping duplicate spawn.");

                    break;
                }

                case "ClassSelection":
                    this.SelectedClass = null;
                    this.ResetSpawnedCharacter();
                    EventBus<OnClassSelection>.Raise(new OnClassSelection { Class = null });
                    break;
                default:
                    Debug.Log($"No spawn required for scene: {scene.name}");
                    break;
            }
        }

        void SpawnCharacter() {
            if (this.SelectedClass == null || this.SelectedClass.Prefab == null) {
                Debug.LogError("SelectedClass is null or does not have a valid characterPrefab.");
                return;
            }

            // Spawn the character if none exists
            Vector3 spawnPosition = new(-4, 8, -40);
            Quaternion spawnRotation = Quaternion.identity;
            this.spawnedCharacter = Instantiate(this.SelectedClass.Prefab, spawnPosition, spawnRotation);
        }

        void ResetSpawnedCharacter() {
            if (this.spawnedCharacter == null) return;
            Destroy(this.spawnedCharacter);
            this.spawnedCharacter = null;
        }
    }
}

public struct OnClassSelection : IEvent {
    public ClassData Class;
}