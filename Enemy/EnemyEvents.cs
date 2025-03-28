public struct OnEnemyLevelUp : IEvent {
    public LevelUpData Data { get; private set; }

    public OnEnemyLevelUp(LevelUpData data) {
        this.Data = data;
    }
}

public struct OnEnemySpawned : IEvent { }

public struct OnEnemyDeath : IEvent {
    public Enemy Enemy { get; private set; }

    public OnEnemyDeath(Enemy enemy) {
        this.Enemy = enemy;
    }
}

public struct OnNextWave : IEvent {
    public int Wave { get; private set; }

    public OnNextWave(int wave) {
        this.Wave = wave;
    }
}