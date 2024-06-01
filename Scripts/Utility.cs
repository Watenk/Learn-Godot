using System;
using Godot;

public static class Utility
{
	private static readonly Random random = new Random();

	public static float RandomFloat(float min, float max)
	{
		return (float)(random.NextDouble() * (max - min) + min);
	}
	
	public static float RandomFloat(Vector2 bounds)
    {
        return (float)(random.NextDouble() * (bounds.Y - bounds.X) + bounds.X);
    }
}