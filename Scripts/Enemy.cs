using Godot;
using System;
using System.ComponentModel;

public partial class Enemy : Area2D, IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Category("EnemySettings")]
	[Export]
	public int MaxHealth { get; private set; }
	
	[Category("BulletSettings")]
	[Export]
	public float BulletSpawnDistance { get; private set; }
	[Export]
	public Vector2 ShootDelay { get; private set; }
	[Export]
	public Vector2 ShotOffset { get; private set; }
	
	[Export]
	public PackedScene Bullet { get; private set; }
	[Export]
	public PlayerResource PlayerResource { get; private set; }
	
	public int Health { get; private set; }

	private LoopingTimer shootTimer;
	
	public override void _Ready()
	{
		shootTimer = new LoopingTimer(Utility.RandomFloat(ShootDelay));
		shootTimer.OnTimer += OnShootTimerDone;
	}

	public override void _Process(double delta)
	{
		shootTimer.Tick((float)delta);
		UpdateMovement();
	}
	
	private void OnShootTimerDone()
	{
		ShootBulletInPlayerDir();
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		EmitSignal(SignalName.Hit);
		
		if (Health <= 0)
		{
			Health = 0;
			EmitSignal(SignalName.Death);
			shootTimer.OnTimer -= OnShootTimerDone;
			Hide();
			QueueFree();
		}
	}
	
	private void OnBodyEntered(Node body2D)
	{
		GD.Print("Enemy Collision");
		TakeDamage(1);
	}
	
	private void UpdateMovement()
	{
		
	}
	
	private void ShootBulletInPlayerDir()
	{
		Vector2 playerPos = PlayerResource.PlayerPos;
		Vector2 direction = (playerPos - GlobalPosition).Normalized();
		Vector2 spawnPos = GlobalPosition + direction * BulletSpawnDistance;
		
		Bullet bullet = Bullet.Instantiate() as Bullet;
		bullet.Position = spawnPos;
		Vector2 offsetDirection = new Vector2(direction.X + Utility.RandomFloat(ShotOffset.X, ShotOffset.Y), direction.Y + Utility.RandomFloat(ShotOffset.X, ShotOffset.Y));
		bullet.Init(offsetDirection.Normalized());
		GetParent().AddChild(bullet);
	}
}
