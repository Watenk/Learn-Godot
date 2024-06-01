using Godot;
using System;
using System.ComponentModel;

public partial class Enemy : Area2D, IDamageble, IPoolable
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void DeathEventHandler(Enemy enemy);
	
	[Category("EnemySettings")]
	[Export]
	public int MaxHealth { get; private set; }
	[Export]
	public Vector2 UpDownTimeBounds { get; private set; }
	[Export]
	public Vector2 SpeedBounds { get; private set; }
	
	[Category("BulletSettings")]
	[Export]
	public float BulletSpawnDistance { get; private set; }
	[Export]
	public Vector2 ShootDelay { get; private set; }
	[Export]
	public Vector2 ShotOffset { get; private set; }
	
	[Export]
	public PackedScene Bullet { get; private set; }
	
	public PlayerResource PlayerResource;
	
	public int Health { get; private set; }
	public bool Disabled { get; private set; }

	private LoopingTimer shootTimer;
	private LoopingTimer upDownTimer;
	private LoopingTimer randomizeUpDownTimerTimer;
	private int upOrDown;
	private int leftOrRight;
	private float speed;
	
	public override void _Ready()
	{
		if (Utility.RandomFloat(0.0f, 1.0f) >= 0.5f)
		{
			leftOrRight = 1;
			upOrDown = 1;
		}
		else
		{
			leftOrRight = -1;
			upOrDown = -1;
		}
		speed = Utility.RandomFloat(SpeedBounds);
		
		shootTimer = new LoopingTimer(Utility.RandomFloat(ShootDelay));
		upDownTimer = new LoopingTimer(Utility.RandomFloat(UpDownTimeBounds));
		randomizeUpDownTimerTimer = new LoopingTimer(Utility.RandomFloat(0.5f, 3.0f));
		
		shootTimer.OnTimer += OnShootTimerDone;
		upDownTimer.OnTimer += OnUpDownTimer;
		randomizeUpDownTimerTimer.OnTimer += OnRandomizeUpDownTimerTimer;
	}
	
	public override void _Process(double delta)
	{
		if (Disabled) return;
		
		shootTimer.Tick((float)delta);
		upDownTimer.Tick((float)delta);
		randomizeUpDownTimerTimer.Tick((float)delta);
		UpdateMovement((float)delta);
		BoundsWrapping();
	}
	
	public void Activate()
	{
		Show();
		Disabled = false;
		SetProcess(true);
		SetPhysicsProcess(true);
		SetProcessInput(true);
	}

	public void DeActivate()
	{
		Hide();
		Disabled = true;
		SetProcess(false);
		SetPhysicsProcess(false);
		SetProcessInput(false);
	}
	
	private void OnShootTimerDone()
	{
		ShootBulletInPlayerDir();
	}
	
	private void OnUpDownTimer()
	{
		upOrDown *= -1;
	}
	
	private void OnRandomizeUpDownTimerTimer()
	{
		upDownTimer.SetStartTime(Utility.RandomFloat(0.5f, 3.0f));
	}

	public void TakeDamage(int amount)
	{
		Health -= amount;
		
		EmitSignal(SignalName.Hit);
		
		if (Health <= 0)
		{
			Health = 0;
			EmitSignal(SignalName.Death, this);
		}
	}
	
	private void OnBodyEntered(Node body2D)
	{
		TakeDamage(1);
	}
	
	private void UpdateMovement(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		velocity.X = leftOrRight * speed * delta;
		velocity.Y = upOrDown * speed * delta;
		Position += velocity;
	}
	
	private void ShootBulletInPlayerDir()
	{
		Vector2 playerPos = PlayerResource.Player.Position;
		Vector2 direction = (playerPos - GlobalPosition).Normalized();
		Vector2 spawnPos = GlobalPosition + direction * BulletSpawnDistance;
		
		Bullet bullet = Bullet.Instantiate() as Bullet;
		bullet.Position = spawnPos;
		Vector2 offsetDirection = new Vector2(direction.X + Utility.RandomFloat(ShotOffset.X, ShotOffset.Y), direction.Y + Utility.RandomFloat(ShotOffset.X, ShotOffset.Y));
		bullet.Init(offsetDirection.Normalized());
		GetParent().AddChild(bullet);
	}
	
	private void BoundsWrapping()
	{
		Rect2 viewportRect = GetViewportRect();

		if (Position.X > viewportRect.Size.X)
		{
			Position = new Vector2(0, Position.Y);
		}
		
		else if (Position.X < 0)
		{
			Position = new Vector2(viewportRect.Size.X, Position.Y);
		}

		if (Position.Y> viewportRect.Size.Y)
		{
			Position = new Vector2(Position.X, 0);
		}
		
		else if (Position.Y < 0)
		{
			Position = new Vector2(Position.X, viewportRect.Size.Y);
		}
	}
}
