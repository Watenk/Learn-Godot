using Godot;

public class LoopingTimer : Timer
{
    public LoopingTimer(float startTime) : base(startTime) {}

    public override void Tick(float deltaTime)
	{
		Time -= deltaTime;
		
		if (Time <= 0)
		{
			Time = 0;
			InvokeOnTimer();
			Reset();
		}
	}
}