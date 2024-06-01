using Godot;

public interface IMovable
{
	[Export]
	public float Drag { get; }
	public Vector2 Velocity { get; }
	
	public void UpdateMovement(float delta);
}