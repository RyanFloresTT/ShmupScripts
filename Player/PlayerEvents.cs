using AbilitySystem;

public struct OnPlayerLevelUp : IEvent {
    public LevelUpData LevelUpData { get; set; }

    public OnPlayerLevelUp(LevelUpData data) {
        this.LevelUpData = data;
    }
}

public struct OnPlayerExperienceGained : IEvent {
    public XPData Data { get; set; }


    public OnPlayerExperienceGained(XPData data) {
        this.Data = data;
    }
}

public struct LevelUpData {
    public int Level { get; set; }
}

public struct OnPlayerStatsUpdated : IEvent {
    public PlayerInfo PlayerData { get; set; }

    public OnPlayerStatsUpdated(PlayerInfo data) {
        this.PlayerData = data;
    }
}

public struct OnPlayerAbilityUnlocked : IEvent {
    public Ability Ability;
}

public struct OnPlayerAbilityUse : IEvent {
    public Ability<AbilityData> Ability;
}

public struct OnPlayerHealthChanged : IEvent {
    public Health Health;
}

public struct OnPlayerResourceChanged : IEvent {
    public Resource Resource;

    public OnPlayerResourceChanged(Resource resource) {
        this.Resource = resource;
    }
}

public struct OnPlayerGainedBasicCurrency : IEvent {
    public Purse Purse;

    public OnPlayerGainedBasicCurrency(Purse purse) {
        this.Purse = purse;
    }
}

public struct OnPlayerGainedPermCurrency : IEvent {
    public Purse Purse;

    public OnPlayerGainedPermCurrency(Purse purse) {
        this.Purse = purse;
    }
}

public struct OnPlayerEnabled : IEvent {
    public Player Player;

    public OnPlayerEnabled(Player player) {
        this.Player = player;
    }
}

public struct OnPlayerChoiceMade : IEvent { }

public struct OnPlayerChoiceNeeded : IEvent { }

public struct OnPlayerSendingAbilities : IEvent { }

public struct OnPlayerDeath : IEvent { }