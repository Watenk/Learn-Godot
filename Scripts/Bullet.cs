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
	public int Health { get; private set; }
	
	private Vector2 direction;
	private float speed;

	public override void _Ready()
	{
		Health = MaxHealth;
	}

	public override void _Process(double delta)
	{
		Position += direction * (float)delta * speed;
	}
	
	public void Shoot(Vector2 direction, float speed)
	{
		this.direction = direction;
		this.speed = speed;
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		EmitSignal(SignalName.Hit);
		
		if (Health <= 0)
		{
			Health = 0;
			EmitSignal(SignalName.Death);
			QueueFree();
		}
	}
	
	private void OnBodyEntered(Node body)
	{
		TakeDamage(1);
		if (body is IDamageble)
		{
			((IDamageble)body).TakeDamage(1);
		}
	}
}