using UnityEngine;
using System.Collections;

public class GameHelpers  {
	
	public delegate void OnFoundMatches( HostData[] pGameList);
	
	static PlayerInfo mLocalPlayer;
	
	//use to get the local player info
	public static PlayerInfo GetLocalPlayerInfo()
	{
		if( mLocalPlayer == null)
			mLocalPlayer = new PlayerInfo( "lcl", "Local Player", false);
		
		return mLocalPlayer;
	}
	
	#region MATCHES RETRIEVAL
	
	static OnFoundMatches mResultMatches;
	static float mElapsedTime;
	static float mSearchTime;
	//use to retrive the available matches
	//@param 1: the game type to search
	//@param 2: the result to be called once the gamelist connected
	//@param 3: the search time
	public static void RetrieveAvailableMatches( string pGameType, OnFoundMatches pResult, float pSearchTime)
	{
		if( mResultMatches == null )
			GameController.mUpdateDelegate += SearchUpdate;
		
		mResultMatches = pResult;
		mElapsedTime = Time.time;
		mSearchTime = pSearchTime;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList( pGameType);
	}
	
	//use to search any avaiable game
	//@paramL the ucrrent time
	static void SearchUpdate( float pCurrentTime )
	{
		if( MasterServer.PollHostList().Length == 0 )
		{
			//Debug.Log(pCurrentTime - mElapsedTime);
			//check if done searching
			if( mSearchTime > -1 && pCurrentTime - mElapsedTime > mSearchTime )
			{
				GameController.mUpdateDelegate -= SearchUpdate;
				mResultMatches( null);
				mResultMatches = null;
			}
		}
		else
		{
			GameController.mUpdateDelegate -= SearchUpdate;
			mResultMatches( MasterServer.PollHostList());
			mResultMatches = null;
		}
	}
	
	#endregion
	
	
	#region PLAY ONLINE
	
	static JoiningStatusListener mPlayOnlineListener;
	
	//use to search a matched game and connect to it.
	//if cant found then create one
	//@param: listener while searching a game
	public static void AutoJoinGame( JoiningStatusListener pListener)
	{
		mPlayOnlineListener = pListener;
		RetrieveAvailableMatches( "HOCKEY_M", OnMatchesFound, 5);
		mPlayOnlineListener.OnStartSearch();
	}
	
	//cancel the current autojoin game
	//NOTE:only call this when AutoJoinGame was used
	public static void AutoJoinCancel()
	{	
		//remove game from the master server
		if( Network.isServer )
		{
			Network.Disconnect();
			MasterServer.UnregisterHost();
		}
		
		GameController.mUpdateDelegate -= SearchUpdate;
		GameController.mUpdateDelegate -= SearchConnectedPlayer;
		mPlayOnlineListener.OnRoomClose();
	}
		               
	//callback when matches found
	//@param: the gamelist found
	static void OnMatchesFound( HostData[] pGameList )
	{
		//if found game then join to any found matches
		//else if not found then create a game
		if( pGameList != null )
		{
			for( int i=0; i<pGameList.Length; i++)
			{
				//check if the game is not yet full
				if( pGameList[i].connectedPlayers < 2)
				{
					Network.Connect( pGameList[i]);
					mPlayOnlineListener.OnRoomJoined( pGameList[i]);
					GameController.mUpdateDelegate += SearchConnectedPlayer;
					return;
				}
			}
		}
		
		//if cant join to any game. then create one
		Network.InitializeServer( 2, 4192, !Network.HavePublicAddress());
		MasterServer.RegisterHost( "HOCKEY_M", Network.player.ipAddress);
		mPlayOnlineListener.OnRoomCreated();
		GameController.mUpdateDelegate += SearchConnectedPlayer;
	}
	
	//@host: search if there is a player connected
	static void SearchConnectedPlayer( float pCurrentTime)
	{
		if( Network.connections.Length >= 1)
		{
			if( Network.isServer )
				MasterServer.UnregisterHost();
			GameController.mUpdateDelegate -= SearchConnectedPlayer;
			mPlayOnlineListener.OnRoomReady();
		}
	}
	#endregion
}
