using Godot;
using System;
using System.ComponentModel;

public partial class PlayerController : Area2D, IDamageble, IMovable
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Category("PlayerSettings")]
	[Export]
	public float Drag { get; private set; }
	[Export]
	public float Speed { get; private set; }
	[Export]
	public int MaxHealth { get; private set; }
	
	[Category("BulletSettings")]
	[Export]
	public float BulletSpawnDistance { get; private set; }
	[Export]
	public float ShootDelay { get; private set; }
	
	[Export]
	public PackedScene Bullet { get; private set; }
	[Export]
	public PlayerResource PlayerResource { get; private set; }
	
	public int Health { get; private set; }
	public Vector2 Velocity { get; private set; }

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
		shootDelayTimer.Tick((float)delta);
		UpdateMovement((float)delta);
		BoundsWrapping();
		
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
		GD.Print("Player Collision");
		TakeDamage(1);
	}
	
	private void BoundsWrapping()
	{
		Rect2 viewportRect = GetViewportRect();

		if (Position.X > viewportRect.Size.X)
		{
			Position = new Vector2(0, Position.Y);
		}
		
		else if (Position.X < 0)
		{
			Position = new Vector2(viewportRect.Size.X, Position.Y);
		}

		if (Position.Y> viewportRect.Size.Y)
		{
			Position = new Vector2(Position.X, 0);
		}
		
		else if (Position.Y < 0)
		{
			Position = new Vector2(Position.X, viewportRect.Size.Y);
		}
	}
	
	private void ShootBulletInMouseDir()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = (mousePosition - GlobalPosition).Normalized();
		Vector2 spawnPos = GlobalPosition + direction * BulletSpawnDistance;
		
		Bullet bullet = Bullet.Instantiate() as Bullet;
		bullet.Position = spawnPos;
		bullet.Init(direction);
		GetParent().AddChild(bullet);
	}

	public void UpdateMovement(float delta)
	{
		// Rotation
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = (mousePosition - GlobalPosition).Normalized();
		float angle = Mathf.Atan2(direction.Y, direction.X);
		Rotation = angle + Mathf.Pi / 2;

		// Input
		if (Input.IsActionPressed("Forward"))
		{
			Velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).Normalized() * Speed * delta;
		}
		
		// Drag
		if (Velocity.Length() > 0)
		{
			Vector2 dragVelocity = Velocity;
			if (Velocity.X > 0) dragVelocity.X -= Drag;
			if (Velocity.X < 0) dragVelocity.X += Drag;
			if (Velocity.Y > 0) dragVelocity.Y -= Drag;
			if (Velocity.Y < 0) dragVelocity.Y += Drag;
			Velocity = dragVelocity;
		} 
		
		Position += Velocity * delta;
		
		PlayerResource.PlayerPos = Position;
	}
}