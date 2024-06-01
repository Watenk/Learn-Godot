using Godot;

public interface IDamageble
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler();
	
	[Export]
	public int MaxHealth { get; }
	[Export]
	public int Health { get; }
	
	public void TakeDamage(int amount);
}