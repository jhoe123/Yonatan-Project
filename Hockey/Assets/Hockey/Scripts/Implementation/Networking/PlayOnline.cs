using UnityEngine;
using System.Collections;

public class PlayOnline : GUIButton2, JoiningStatusListener {
	
	public GUILabel mStatusText;
	
	public override void OnRelease (Vector3 pCursorPosition, int pCursorIndex)
	{
		base.OnRelease (pCursorPosition, pCursorIndex);
		mEnabled = false;
		GameHelpers.AutoJoinGame( this);
	}
	
	#region MATCH MAKING
	
	public void OnStartSearch ()
	{
		mStatusText.mContent.text = "Searching game...";
	}
	
	public void OnRoomCreated ()
	{
		mStatusText.mContent.text = "Waiting for players!";
	}
	
	public void OnRoomJoined (HostData pData)
	{
		mStatusText.mContent.text = "Joined to game!";
	}
	
	public void OnRoomReady ()
	{
		mStatusText.mContent.text = "Initiating game";
		
		PlayerInfo owner;
		PlayerInfo opponent;
		Debug.Log( Network.connections.Length);
		
		if( Network.isServer )
		{
			owner = new PlayerInfo( Network.player.externalIP, "loca", true);
			owner.networkPlayer = Network.player;
			opponent = new PlayerInfo( Network.connections[0].externalIP, "remote", true);
			owner.networkPlayer = Network.connections[0];
		}else
		{
			owner = new PlayerInfo( Network.connections[0].externalIP, "remote", true);
			owner.networkPlayer = Network.connections[0];
			opponent = new PlayerInfo( Network.player.externalIP, "loca", true);
			opponent.networkPlayer = Network.player;
		}
		
		GameplayScene.StartGame( owner, opponent);
	}
	
	public void OnError ()
	{
		Debug.Log( "ERROR");
	}
	
	public void OnRoomClose ()
	{
	}
	#endregion
}
