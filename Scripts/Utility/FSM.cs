using System.Collections;
using System.Collections.Generic;

/// <summary> A Finite State Machine that keeps track of state and includes a blackboard </summary>
/// <typeparam name="T"> BlackboardType </typeparam>
public class Fsm<T> : IUpdateable
{
	public BaseState<T> CurrentState { get; private set; }
	
	private Dictionary<System.Type, BaseState<T>> states = new Dictionary<System.Type,BaseState<T>>();
	
	public Fsm(T blackboard, params BaseState<T>[] newStates)
	{
		foreach (BaseState<T> baseState in newStates)
		{
			this.states.Add(baseState.GetType(), baseState);
			baseState.Init(this, blackboard);
		}
	}
	
	public void SwitchState(System.Type state)
	{
		CurrentState?.Exit();
		states.TryGetValue(state, out BaseState<T> baseState);
		CurrentState = baseState;
		CurrentState.Enter();
	}

	public void Update(float delta)
	{
		CurrentState?.Update(delta);
	}
	
	public BaseState<T> GetState<U>()
	{
		states.TryGetValue(typeof(U), out BaseState<T> state);
		return state;
	}
}
