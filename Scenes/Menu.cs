using Godot;
using System;

public partial class Menu : Control
{
	[Export]
	public Button HostButton;

	[Export]
	public Button JoinButton;

    [Export]
    public TextEdit TextEdit;

    private Lobby Lobby;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Lobby = GetNode<Lobby>("/root/Lobby");
        HostButton.Pressed += OnHostButtonPressed;
        JoinButton.Pressed += OnJoinButtonPressed;
	}

    private void OnJoinButtonPressed()
    {
        SetPlayerInfo();
        Lobby.JoinGame();
    }

    private void OnHostButtonPressed()
    {
        SetPlayerInfo();
        Lobby.CreateGame();
    }

    public void SetPlayerInfo()
    {
        Lobby.playerInfo = new Godot.Collections.Dictionary<string, string>()
        {
            {"name", TextEdit.Text}
        };
    }
}
