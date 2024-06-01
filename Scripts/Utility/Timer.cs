using Godot;

public class Timer
{
	public event TimerEventHandler OnTimer;
	public delegate void TimerEventHandler();
	
	public float Time { get; protected set; }
	public float StartTime { get; protected set; }
	
	private bool running = true;
	
	public Timer(float startTime)
	{
		StartTime = startTime;
		Time = StartTime;
	}
	
	public virtual void Tick(float deltaTime)
	{
		if (!running) return;
		
		Time -= deltaTime;
		
		if (Time <= 0)
		{
			Time = 0;
			running = false;
			InvokeOnTimer();
		}
	}
	
	public void InvokeOnTimer()
	{
		OnTimer?.Invoke();
	}
	
	public void Reset()
	{
		Time = StartTime;
		running = true;
	}
	
	public void SetStartTime(float startTime)
	{
		StartTime = startTime;
	}
}