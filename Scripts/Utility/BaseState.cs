using System.Collections;
using System.Collections.Generic;

/// <summary> BaseState for FSM use. With a blackboard </summary>
/// <typeparam name="T"> BlackboardType </typeparam>
public abstract class BaseState<T> : IUpdateable
{
	protected Fsm<T> owner;
	protected T bb;
	
	public virtual void Init(Fsm<T> owner, T blackboard)
	{
		this.owner = owner;
		this.bb = blackboard;
	}
	
	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Update(float delta) {}
}
