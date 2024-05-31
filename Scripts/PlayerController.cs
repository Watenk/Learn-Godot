using Godot;
using System;
using System.ComponentModel;

public partial class PlayerController : Area2D, IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Category("PlayerSettings")]
	[Export]
	public float Speed { get; private set; }
	[Export]
	public int MaxHealth { get; private set; }
	
	[Category("BulletSettings")]
	[Export]
	public float BulletSpeed { get; private set; }
	[Export]
	public float BulletDistance { get; private set; }
	[Export]
	public float ShootDelay { get; private set; }
	
	[Export]
	public PackedScene Bullet;
	
	public int Health { get; private set; }

	private Vector2 screenSize;
	private Timer shootDelayTimer;
	
	public override void _Ready()
	{
		Health = MaxHealth;
		screenSize = GetViewportRect().Size;
		shootDelayTimer = new Timer(ShootDelay);
	}

	public override void _Process(double delta)
	{
		UpdateMovement((float)delta);
		shootDelayTimer.Tick((float)delta);
		
		if (Input.IsActionPressed("Shoot"))
		{
			if (shootDelayTimer.Time != 0) return;
			
			shootDelayTimer.Reset();
			ShootBulletInMouseDir();
		}
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		EmitSignal(SignalName.Hit);
		
		if (Health <= 0)
		{
			Health = 0;
			EmitSignal(SignalName.Death);
		}
	}
	
	private void OnBodyEntered(Node2D body)
	{
		TakeDamage(1);
		if (body is IDamageble)
		{
			((IDamageble)body).TakeDamage(1);
		}
	}
	
	private void UpdateMovement(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("Forward"))
		{
			velocity.Y += 1;
		}
		
		velocity = velocity.Normalized() * Speed;
		Position += velocity * delta;
	}
	
	private void ShootBulletInMouseDir()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 playerPos = GlobalPosition;
		Vector2 direction = (mousePosition - playerPos).Normalized();
		Vector2 spawnPos = playerPos + direction * BulletDistance;
		
		Bullet bullet = Bullet.Instantiate() as Bullet;
		bullet.Position = spawnPos;
		bullet.Shoot(direction, BulletSpeed);
		GetParent().AddChild(bullet);
	}
}