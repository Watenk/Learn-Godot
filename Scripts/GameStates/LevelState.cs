using System.Collections.Generic;
using Godot;

public class LevelState : BaseState<GameManager>
{
	private Node level;
	
	private PlayerController player;
	
	private List<Enemy> enemies = new List<Enemy>();
	private Timer spawnEnemyTimer;
	private Label scoreLabel;
	private Label livesLabel;
	
	public override void Enter()
	{
		level = bb.LoadScene(bb.Level);
		scoreLabel = level.GetNode<Label>("ScoreLabel");
		if (scoreLabel == null) GD.PrintErr("Couldn't find ScoreLabel in level");
		livesLabel = level.GetNode<Label>("LivesLabel");
		if (livesLabel == null) GD.PrintErr("Couldn't find LivesLabel in level");
		player = level.GetNode<PlayerController>("Player");
		if (player == null) GD.PrintErr("Couldn't find Player in level");
		player.Death += OnPlayerDeath;
		player.Hit += OnPlayerDamaged;
		
		bb.score = 1;
		
		spawnEnemyTimer = new Timer(GetNewSpawnRate());
		spawnEnemyTimer.OnTimer += OnSpawn;
		
		OnSpawn();
	}

	public override void Update(float delta)
	{
		spawnEnemyTimer.Tick(delta);
	}

	public override void Exit()
	{
		foreach (Enemy enemy in enemies)
		{
			enemy.DeActivate();
		}
		spawnEnemyTimer.OnTimer -= OnSpawn;
		player.Death -= OnPlayerDeath;
		player.Hit -= OnPlayerDamaged;
		level.QueueFree();
	}
	
	public float GetNewSpawnRate()
	{
		float newSpawnRate = bb.GameSettingsResource.BaseEnemySpawnRate - (bb.GameSettingsResource.SpawnRateScoreMultiplier * bb.score) + 1 + Utility.RandomFloat(-bb.GameSettingsResource.SpawnRateDeviation, bb.GameSettingsResource.SpawnRateDeviation);
		if (newSpawnRate < 1) newSpawnRate = 1;
		return newSpawnRate;
	}
	
	private void OnSpawn()
	{
		spawnEnemyTimer.SetStartTime(GetNewSpawnRate());
		spawnEnemyTimer.Reset();
		
		Enemy enemy = bb.ObjectManager.GetObject<Enemy>();
		enemy.PlayerResource = player.PlayerResource;
		enemy.Position = GetEnemyPos();
		enemy.Death += OnEnemyDeath;
		enemies.Add(enemy);	
	}
	
	private void OnPlayerDeath()
	{
		owner.SwitchState(typeof(GameOverState));
	}
	
	private void OnPlayerDamaged()
	{
		livesLabel.Text = "Lives: " + player.Health;
	}
	
	private void OnEnemyDeath(Enemy enemy)
	{
		bb.score++;
		scoreLabel.Text = "Score: " + bb.score;
		enemy.Death -= OnEnemyDeath;
		enemy.Position = Vector2.Zero;
		bb.ObjectManager.ReturnObject(enemy);
		enemies.Remove(enemy);
	}
	
	private Vector2 GetEnemyPos()
	{
		Vector2 screenSize = bb.GetViewportRect().Size;

		int padding = 50;

		float x = 0;
		float y = 0;

		if (Utility.RandomFloat(0.0f, 1.0f) > 0.5f) 
		{
			// Up / Down
			if (Utility.RandomFloat(0.0f, 1.0f) > 0.5f)
			{
				y = -padding; 
			}
			else
			{
				y = screenSize.Y + padding;
			}
			
			x = Utility.RandomFloat(0.0f, screenSize.X);
		}
		else 
		{
			// Right / Left
			if (Utility.RandomFloat(0.0f, 1.0f) > 0.5f)
			{
				x = -padding; 
			}
			else
			{
				x = screenSize.X + padding;
			}
			
			y = Utility.RandomFloat(0.0f, screenSize.Y);
		}

		return new Vector2(x, y);
	}
}