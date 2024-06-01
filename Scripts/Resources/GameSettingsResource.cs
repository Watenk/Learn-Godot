using Godot;

public partial class GameSettingsResource : Resource
{
	[Export]
	public float BaseEnemySpawnRate { get; private set; }
	[Export]
	public float SpawnRateScoreMultiplier { get; private set; }
	[Export]
	public float SpawnRateDeviation { get; private set; }
}