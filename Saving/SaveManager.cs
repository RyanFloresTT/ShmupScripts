using System.IO;
using System.Reflection;
using UnityEngine;

namespace Saving {
    #region Data Structure

    /// <summary>
    /// Enumeration defining various fields that can be saved.
    /// </summary>
    public enum SaveField {
        ScreenMode,
        Resolution,
        MusicVolume,
        SFXVolume,
        PermanentPurse,
        WaveScore
    }

    /// <summary>
    /// Serializable class representing save data for the game.
    /// </summary>
    [System.Serializable]
    public class SaveData {
        /// <summary>
        /// Serializable class representing resolution settings.
        /// </summary>
        [System.Serializable]
        public class SerializableResolution {
            public int Width;
            public int Height;
            public double RefreshRate;

            public SerializableResolution() { }

            public SerializableResolution(Resolution resolution) {
                this.Width = resolution.width;
                this.Height = resolution.height;
                this.RefreshRate = resolution.refreshRateRatio.value;
            }
        }

        /// <summary>
        /// Serializable class representing game settings.
        /// </summary>
        [System.Serializable]
        public class Settings {
            public FullScreenMode ScreenMode;
            public SerializableResolution Resolution;
            public float MusicVolume = 10f;
            public float SFXVolume = 10f;
        }

        /// <summary>
        /// Serializable class representing player information.
        /// </summary>
        [System.Serializable]
        public class PlayerInfo {
            public Purse PermanentPurse = new(0, 100);
        }

        public Settings GameSettings = new();
        public PlayerInfo PlayerData = new();
        public int WaveScore = 0;
    }

    #endregion

    /// <summary>
    /// ScriptableObject for managing game save operations.
    /// </summary>
    [CreateAssetMenu(fileName = "SaveManager", menuName = "ScriptableObjects/Managers/SaveManager")]
    public class SaveManager : ScriptableObject {
        static SaveManager instance;
        SaveData saveData;
        string saveFilePath;

        /// <summary>
        /// Singleton instance of the SaveManager.
        /// </summary>
        public static SaveManager Instance {
            get {
                if (instance == null) instance = Resources.Load<SaveManager>("SaveManager");
                return instance;
            }
        }

        /// <summary>
        /// Exposes the in-memory save data.
        /// </summary>
        public SaveData SaveData {
            get {
                if (this.saveData == null) this.LoadGameData(); // Ensure data is loaded
                return this.saveData;
            }
        }

        /// <summary>
        /// Initializes the SaveManager and loads the save data.
        /// </summary>
        void OnEnable() {
            instance = this; // Ensure Singleton assignment
            this.saveFilePath = Path.Combine(Application.persistentDataPath, "game_data.json");

            // Load data at startup if it hasn't already been loaded
            this.LoadGameData();
        }

        /// <summary>
        /// Loads the game data from disk into memory.
        /// </summary>
        void LoadGameData() {
            if (File.Exists(this.saveFilePath)) {
                string json = File.ReadAllText(this.saveFilePath);
                this.saveData = JsonUtility.FromJson<SaveData>(json);
                Logger.Log("Game data loaded successfully", Logger.LogCategory.Saving);
            }
            else {
                // If no save file exists, create a new one from scratch
                this.saveData = this.CreateNewSaveData();
                this.SaveGameData(); // Persist the default save data to disk
            }
        }

        /// <summary>
        /// Creates a new instance of SaveData with default values.
        /// </summary>
        SaveData CreateNewSaveData() {
            SaveData newSave = new() {
                GameSettings = {
                    ScreenMode = Screen.fullScreenMode,
                    Resolution = new SaveData.SerializableResolution(Screen.currentResolution),
                    MusicVolume = 10f,
                    SFXVolume = 10f
                },
                PlayerData = {
                    PermanentPurse = new Purse(0, 100)
                }
            };
            Logger.Log("New save data created", Logger.LogCategory.Saving);
            return newSave;
        }

        /// <summary>
        /// Saves the in-memory save data to disk.
        /// </summary>
        public void SaveGameData() {
            if (this.saveData == null) return;

            string json = JsonUtility.ToJson(this.saveData, true);
            File.WriteAllText(this.saveFilePath, json);
            Logger.Log("Game data saved to disk", Logger.LogCategory.Saving);
        }

        /// <summary>
        /// Updates a specific field in the SaveData directly.
        /// </summary>
        /// <param name="field">The field to update.</param>
        /// <param name="value">The new value to set.</param>
        public void UpdateSingleField(SaveField field, object value) {
            if (this.saveData == null) this.LoadGameData(); // Fallback in case data is not yet loaded

            switch (field) {
                case SaveField.PermanentPurse:
                    if (value is Purse permPurse)
                        this.saveData.PlayerData.PermanentPurse = permPurse;
                    else
                        Logger.Log($"Invalid type for {field}. Expected Purse.", Logger.LogCategory.Saving);
                    break;
                case SaveField.WaveScore:
                    if (value is int waveScore)
                        this.saveData.WaveScore = waveScore;
                    else
                        Logger.Log($"Invalid type for {field}. Expected int.", Logger.LogCategory.Saving);
                    break;
                case SaveField.MusicVolume:
                    if (value is float musicVolume)
                        this.saveData.GameSettings.MusicVolume = musicVolume;
                    else
                        Logger.Log($"Invalid type for {field}. Expected float.", Logger.LogCategory.Saving);
                    break;
                case SaveField.SFXVolume:
                    if (value is float sfxVolume)
                        this.saveData.GameSettings.SFXVolume = sfxVolume;
                    else
                        Logger.Log($"Invalid type for {field}. Expected float.", Logger.LogCategory.Saving);
                    break;
                default:
                    Logger.Log($"{field} is not supported for updates.", Logger.LogCategory.Saving);
                    break;
            }
        }

        /// <summary>
        /// Deletes the save file if necessary (for debugging or resets).
        /// </summary>
        public void DeleteSaveFile() {
            if (File.Exists(this.saveFilePath)) {
                File.Delete(this.saveFilePath);
                Logger.Log("Save file deleted", Logger.LogCategory.Saving);
            }
        }
    }
}