using System.Collections.Generic;
using Godot;

public class ObjectPool<T> 
{
	private List<IPoolable> idlePoolables = new List<IPoolable>();
	private List<IPoolable> activePoolables = new List<IPoolable>();
	
	public ObjectPool(PackedScene scene, int size, Node parent) 
	{
		for (int i = 0; i < size; i++)
		{
			Node instance = scene.Instantiate();
			((IPoolable)instance).DeActivate();
			idlePoolables.Add((IPoolable)instance);
			parent.AddChild(instance);
		}
	}
	
	public IPoolable GetPoolable()
	{
		if (idlePoolables.Count == 0) GD.PrintErr("Tried to get " + typeof(T).Name + " but pool is empty");
		IPoolable poolable = idlePoolables[0];
		idlePoolables.RemoveAt(0);
		activePoolables.Add(poolable);
		poolable.Activate();
		return poolable;
	}
	
	public void ReturnPoolable(IPoolable returnedPoolable)
	{
		activePoolables.Remove(returnedPoolable);
		idlePoolables.Add(returnedPoolable);
		returnedPoolable.DeActivate();
	}
}