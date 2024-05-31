using Godot;
using System;

public partial class Bullet : RigidBody2D, IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Export]
	public int MaxHealth { get; private set; }
	[Export]
	public int Health { get; private set; }

	public override void _Ready()
	{
		Health = MaxHealth;
	}

	public override void _Process(double delta)
	{
		
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

}
