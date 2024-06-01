using System;
using System.Collections.Generic;
using Godot;

public partial class ObjectManager : Node
{
	[Export]
	public int EnemyPoolSize { get; private set; }
	[Export]
	public int BulletPoolSize { get; private set; }
	
	[Export]
	public PackedScene Enemy { get; private set; }
	[Export]
	public PackedScene Bullet { get; private set; }
	
	private Dictionary<Type, object> objectPools = new Dictionary<Type, object>();

	public override void _Ready()
	{
		AddPool<Enemy>(Enemy, EnemyPoolSize);
		AddPool<Bullet>(Bullet, BulletPoolSize);
	}
	
	public T GetObject<T>() 
	{
		return (T)GetPool<T>().GetPoolable();
	}
	
	public void ReturnObject<T>(T returnedObject)
	{
		GetPool<T>().ReturnPoolable((IPoolable)returnedObject);
	}
	
	private ObjectPool<T> GetPool<T>()
	{
		objectPools.TryGetValue(typeof(T), out object pool);
		if (pool == null) GD.PrintErr("Tried to get " + typeof(T).Name + " pool that doesn't exist");
		return (ObjectPool<T>)pool;
	}
	
	private void AddPool<T>(PackedScene scene, int size)
	{
		objectPools.Add(typeof(T), new ObjectPool<T>(scene, size, this));
	}
}