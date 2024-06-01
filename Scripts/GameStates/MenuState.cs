using Godot;
using System;

public class MenuState : BaseState<GameManager>
{
	private Button playButton;
	private Node mainMenu;

	public override void Enter()
	{
		mainMenu = bb.LoadScene(bb.MainMenu);
		
		playButton = mainMenu.GetNode<Button>("PlayButton");
		if (playButton == null) GD.PrintErr("Couldn't find PlayButton in mainMenu");
		playButton.Pressed += OnPlayButton;
	}

	public override void Exit()
	{
		playButton.Pressed -= OnPlayButton;
		mainMenu.QueueFree();
		playButton = null;
		mainMenu = null;
	}

	private void OnPlayButton()
	{
		owner.SwitchState(typeof(LevelState));
	}
}