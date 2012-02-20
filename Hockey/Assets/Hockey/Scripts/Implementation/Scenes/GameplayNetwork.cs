using UnityEngine;
using System.Collections;

/*
 * GameplayNetwork.cs
 * Jhoemar Pagao (c) 2012
 * jhoemar.pagao@gmail.com
 * 
 * Implementation for a network gameplay.
 * */
public class GameplayNetwork : GameplayScene {
	
	NetworkView mView;
	
	//true if the current player is host while false if not
	static bool mIsHost;
	public static bool isHost
	{ get{return mIsHost;} }
	
	protected override void Awake ()
	{
		mCurrent = this;
		mView = networkView;
		mOwnerPlayer = mPlayer1;
		mOpponentPlayer = mPlayer2;
		
		PlayerController controller = (PlayerController)FindObjectOfType( typeof( PlayerController));
		
		//init the host and opponent player
		if(  Network.isServer )
		{
			GameHelpers.UpdateMatchInfo( gameInfo);
			mIsHost = true;
			mCurrentPlayer = ownerPlayer;
			controller.Initialize( mOwnerPlayer);
		}
		else
		{
			mIsHost = false;
			mCurrentPlayer = mOpponentPlayer;
			controller.Initialize( mOpponentPlayer);
		}
		
		base.Awake ();
		
		if( !mIsHost)
			mView.RPC( "OnOpponentLoaded", RPCMode.OthersBuffered );
	}
	
	#region METHOD CALLBACKS
	
	//callback when a player goal
	//@param: the player who goaled
	public override void OnPlayerGoalStart (PlayerIngame pGoalee)
	{
		base.OnPlayerGoalStart (pGoalee);
		if( mIsHost )
			mView.RPC( "OnPlayerGoalStart_N", RPCMode.Others, pGoalee.info.networkPlayer);
	}
	
	//callback when game ends
	//@param: the winner of the game
	public override void OnGameEnd (PlayerIngame pWinner)
	{
		base.OnGameEnd (pWinner);
		if( mIsHost )
			mView.RPC( "OnGameEnd_N", RPCMode.Others, pWinner.info.networkPlayer );
	}
	
	#endregion
	
	#region RPC CALLBACKS
	
	[RPC]
	//@RPC Host: callback when the client was successfully loaded
	//@param: the player who successfully loaded
	void OnOpponentLoaded()
	{
		CancelInvoke( "CheckIfOpponentLoaded");
		OnGameStart();
	}
	
	[RPC]
	//@RPC ALL: callback when the game starts
	public override void OnGameStart ()
	{
		base.OnGameStart ();
		if( mIsHost )
			mView.RPC( "OnGameStart", RPCMode.Others);
	}
	
	[RPC]
	//@RPC ALL: callback when the intro start
	public override void OnIntroStart ()
	{
		base.OnIntroStart ();
		if( mIsHost )
			mView.RPC( "OnIntroStart", RPCMode.Others );
	}
	
	[RPC]
	//@RPC ALL: callback when the intro end
	public override void OnIntroEnd ()
	{
		base.OnIntroEnd ();
		if( mIsHost )
			mView.RPC( "OnIntroStart", RPCMode.Others );
	}
	
	[RPC]
	//@RPC Client: callback when a player goaled
	//@param: the player who goaled
	void OnPlayerGoalStart_N( NetworkPlayer pGoalee)
	{
		if( pGoalee == mOwnerInfo.networkPlayer )
			OnPlayerGoalStart( ownerPlayer);
		else
			OnPlayerGoalStart( opponentPlayer);
	}
	
	[RPC]
	//@RPC ALL: callback when the goal end
	protected override void OnPlayerGoalEnd ()
	{
		base.OnPlayerGoalEnd ();
		if( mIsHost )
			mView.RPC( "OnPlayerGoalEnd", RPCMode.Others);
	}
	
	[RPC]
	//@RPC CLient: callback when recieved a game end
	//@param: the player who winned
	void OnGameEnd_N( NetworkPlayer pPlayer)
	{
		if( pPlayer == mOwnerInfo.networkPlayer )
			OnGameEnd( mOwnerPlayer);
		else
			OnGameEnd( mOpponentPlayer);
	}
	
	//@server. callback when the player disconnedted
	void OnPlayerDisconnected( NetworkPlayer pPlayer )
	{
		OnGameEnd( currentPlayer);
		GameHelpers.UpdateMatchInfo( gameInfo);
	}
	
	//@client. callback when disconnected
	void OnDisconnectedFromServer( NetworkDisconnection pDisconnectionInfo)
	{
	}

	#endregion
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		mCurrent = this;
	}
}
