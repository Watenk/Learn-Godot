using Godot;
using System;

public partial class Bullet : Area2D, IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Export]
	public int MaxHealth { get; private set; }
	[Export]
	public float BulletSpeed { get; private set; }
	[Export]
	public float BulletLifetime { get; private set; }
	
	public int Health { get; private set; }
	
	private Vector2 direction;
	private Timer lifetimeTimer;

	public override void _Ready()
	{
		Health = MaxHealth;
	}

	public override void _Process(double delta)
	{
		lifetimeTimer.Tick((float)delta);
		Position += direction * (float)delta * BulletSpeed;
		BoundsWrapping();
	}
	
	public void Init(Vector2 direction)
	{
		this.direction = direction;
		lifetimeTimer = new Timer(BulletLifetime);
		lifetimeTimer.OnTimer += OnEndLife;
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		EmitSignal(SignalName.Hit);
		
		if (Health <= 0)
		{
			Health = 0;
			EmitSignal(SignalName.Death);
			lifetimeTimer.OnTimer -= OnEndLife;
			Hide();
			QueueFree();
		}
	}
	
	private void OnBodyEntered(Node body2D)
	{
		GD.Print("Bullet Collision");
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
	
	private void OnEndLife()
	{
		lifetimeTimer.OnTimer -= OnEndLife;
		EmitSignal(SignalName.Death);
		QueueFree();
	}
}