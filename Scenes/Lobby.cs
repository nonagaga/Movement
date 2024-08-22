using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using static Godot.MultiplayerApi;

public partial class Lobby : Node
{
	[Signal]
	public delegate void PlayerConnectedEventHandler(long peerId, Godot.Collections.Dictionary<string, string> playerInfo);

	[Signal]
	public delegate void PlayerDisconnectedEventHandler(long peerId);
	[Signal]
	public delegate void ServerDisconnectedEventHandler();

	const int PORT = 7000;
	const string DEFAULT_SERVER_IP = "127.0.0.1";
	const int MAX_CONNECTIONS = 2;

	//contains playerinfo for every player
	public Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>> players = new();

	//player info for local player
	public Godot.Collections.Dictionary<string, string> playerInfo = new Godot.Collections.Dictionary<string, string>()
	{
		{"name", "name" }
	};

	public int PlayersLoaded = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedOk;
        Multiplayer.ConnectionFailed += OnConnectedFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;

        PlayerConnected += Lobby_PlayerConnected;
	}

    private void Lobby_PlayerConnected(long peerId, Godot.Collections.Dictionary<string, string> playerInfo)
    {
        if(Multiplayer.IsServer())
        {
            GD.Print("Player: " + players[peerId]["name"] + " has connected!");
        }
    }

    public void JoinGame(string address = "")
    {
        if (address == "")
        {
            address = DEFAULT_SERVER_IP;
        }

        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address,PORT);
        if(error != Error.Ok)
        {
            GD.PrintErr(error);
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        GD.Print("Game successfully joined!");
    }

    public void CreateGame()
    {
        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(PORT, MAX_CONNECTIONS);
        if (error != Error.Ok)
        {
            GD.PrintErr(error);
            return;
        }
        Multiplayer.MultiplayerPeer = peer;

        players[1] = playerInfo;
        EmitSignal(nameof(PlayerConnected), 1,playerInfo);

        GD.Print("Game successfully created!");
    }

    public void RemoveMultiplayerPeer()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    [Rpc(CallLocal = true,TransferMode =MultiplayerPeer.TransferModeEnum.Reliable)]
    public void PlayerLoaded()
    {
        if (Multiplayer.IsServer())
        {
            PlayersLoaded++;
            if (PlayersLoaded == players.Count)
            {
                GetNode("/root.Game").Call("StartGame");
                PlayersLoaded = 0;
            }
        }
    }

    public void OnPeerConnected(long id)
    {
        
    }

    [Rpc(RpcMode.AnyPeer,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RegisterPlayer(Godot.Collections.Dictionary<string, string> newPlayerInfo)
    {
        var newPlayerId = Multiplayer.GetRemoteSenderId();
        players[newPlayerId] = newPlayerInfo;
        EmitSignal(nameof(PlayerConnected), newPlayerId, newPlayerInfo);
    }

    public void OnServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        players.Clear();
        EmitSignal(nameof(ServerDisconnected));
    }

    public void OnConnectedFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    public void OnConnectedOk()
    {
        var peerId = Multiplayer.GetUniqueId();
        players[peerId] = playerInfo;
        EmitSignal(nameof(PlayerConnected),peerId, playerInfo);

        Rpc("RegisterPlayer",playerInfo);
    }

    public void OnPeerDisconnected(long id)
    {
        players.Remove(id);
        EmitSignal(nameof(PlayerDisconnected), id);
    }


}
