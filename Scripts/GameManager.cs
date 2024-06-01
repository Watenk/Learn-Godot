using System.ComponentModel;
using Godot;

public partial class GameManager : Node2D
{
	[Category("References")]
	[Export]
	public ObjectManager ObjectManager { get; private set; }
	[Export]
	public GameSettingsResource GameSettingsResource { get; private set; }
	
	[Category("Scenes")]
	[Export]
	public PackedScene MainMenu { get; private set; }
	[Export]
	public PackedScene Level { get; private set; }
	[Export]
	public PackedScene GameOver { get; private set; }
	
	public int score = 1;
	
	private Fsm<GameManager> gameState;

	public override void _Ready()
	{
		gameState = new Fsm<GameManager>(this,
			new MenuState(),
			new LevelState(),
			new GameOverState()
		);
		
		gameState.SwitchState(typeof(MenuState));
	}

	public override void _Process(double delta)
	{
		gameState.Update((float)delta);
	}
	
	public Node LoadScene(PackedScene packedScene)
	{
		Node instance = packedScene.Instantiate();
		CallDeferred(nameof(AddChildDeferred), instance);
		return instance;
	}
	
	public void AddChildDeferred(Node childNode)
	{
		GetParent().AddChild(childNode);
	}
}