using Godot;
using System;

public partial class PlayerController : Area2D, IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Export]
	public float Speed { get; private set; }
	[Export]
	public int MaxHealth { get; private set; }
	[Export]
	public int Health { get; private set; }

	private Vector2 screenSize;
	
	public override void _Ready()
	{
		Health = MaxHealth;
		screenSize = GetViewportRect().Size;
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("Forward"))
		{
			velocity.Y += 1;
		}
		
		velocity = velocity.Normalized() * Speed;
		Position += velocity * (float)delta;
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
}
