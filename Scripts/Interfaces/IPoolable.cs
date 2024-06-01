using Godot;

public interface IPoolable
{
	public bool Disabled { get; }
	
	public void Activate();
	public void DeActivate();
}