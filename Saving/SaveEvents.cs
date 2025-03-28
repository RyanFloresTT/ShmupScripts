namespace Saving {
    public struct OnInitialSettingsRetrieval : IEvent {
        public SaveData GameData;

        public OnInitialSettingsRetrieval(SaveData gameData) {
            GameData = gameData;
        }
    }
}