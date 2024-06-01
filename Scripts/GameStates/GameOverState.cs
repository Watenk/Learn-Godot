using Godot;

public class GameOverState : BaseState<GameManager>
{
	private Node gameOver;
	private Label scoreLabel;
	private Timer timer;
	
	public override void Enter()
	{
		gameOver = bb.LoadScene(bb.GameOver);
		
		scoreLabel = gameOver.GetNode<Label>("ScoreLabel");
		if (scoreLabel == null) GD.PrintErr("Couldn't find ScoreLabel in gameOver");
		
		timer = new Timer(10.0f);
		timer.OnTimer += OnTimer;
		
		scoreLabel.Text = "Score: " + bb.score;
	}

	public override void Update(float delta)
	{
		timer.Tick(delta);
	}

	public override void Exit()
	{
		timer.OnTimer -= OnTimer;
		gameOver.QueueFree();
		gameOver = null;
		scoreLabel = null;
		timer = null;
	}

	private void OnTimer()
	{
		owner.SwitchState(typeof(MenuState));
	}
}