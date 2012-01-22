using UnityEngine;
using System.Collections;

public class GameplayScene : Scene {
	
	public static GameplayScene mCurrent;
	
	[SerializeField]
	protected PlayerIngame mPlayer1;
	
	[SerializeField]
	protected PlayerIngame mPlayer2;
	
	static protected PlayerInfo mOwnerInfo;
	static protected PlayerInfo mOpponentInfo;
	
	#region PROPERTIES
	
	//the player owner/who host this game
	protected static PlayerIngame mOwnerPlayer;
	public static PlayerIngame ownerPlayer
	{ get{return mOwnerPlayer;}}
	
	//the player opponent
	protected static PlayerIngame mOpponentPlayer;
	public static PlayerIngame opponentPlayer
	{ get{return mOpponentPlayer;}}
	
	//the current game information ongoing
	protected static GameInfo mGameinfo;
	public static GameInfo gameInfo
	{ get{ return mGameinfo;}}
	
	//the hockey table
	static HockeyTable mTable;
	public static HockeyTable table
	{ get{ return mTable;}}
	
	#endregion
	
	#region GAME LOADING
	
	//start the game for two remote player
	//@param 1: the host of the game
	//@param 2: the opponent of the host
	public static void StartGame( PlayerInfo pOwner, PlayerInfo pOpponent)
	{
		mOwnerInfo = pOwner;
		mOpponentInfo = pOpponent;
		if( Application.loadedLevelName == "GameplayNetwork" )
			mCurrent.Reset();
		else
			Application.LoadLevel( "GameplayNetwork");
	}
	
	//start the game locally
	//@param 1: the owner information
	//@param 2: the ai type
	public static void StartGame( PlayerInfo pOwner, eAI pAiType)
	{
		//mOwnerPlayer = pOwner;
		//mOpponentPlayer = null;
		
		if( Application.loadedLevelName == "GameplayLocal" )
			mCurrent.Reset();
		else
			Application.LoadLevel( "GameplayLocal");
	}
	
	//start the game locally( can be used for saved games)
	//@param: the game information
	public static void StartGame( GameInfo pInfo)
	{}
	
	#endregion
	
	#region CALLBACKS
	
	protected override void Awake ()
	{
		base.Awake ();
		if( mOwnerInfo == null )
		{
			mOwnerInfo = GameHelpers.GetLocalPlayerInfo();
			mOpponentInfo = mOwnerInfo;
		}
		
		mGameinfo = new GameInfo( mOwnerInfo, mOpponentInfo, 0, 0);
		ownerPlayer.info = mOwnerInfo;
		opponentPlayer.info = mOpponentInfo;
	}
	
	public virtual void OnGameStart()
	{
		mOwnerPlayer.OnGameStart();
		mOpponentPlayer.OnGameStart();
		mTable.OnGameStart();
	}
	
	public virtual void OnIntroStart()
	{
		mOwnerPlayer.OnIntroStart();
		mOpponentPlayer.OnIntroStart();
		mTable.OnIntroStart();
	}
	
	public virtual void OnIntroEnd()
	{
		mOwnerPlayer.OnIntroEnd();
		mOpponentPlayer.OnIntroEnd();
		mTable.OnIntroEnd();
	}
	
	public virtual void OnPlayerGoalStart( PlayerIngame pGoalee)
	{
		mOwnerPlayer.OnPlayerGoalStart( pGoalee);
		mOpponentPlayer.OnPlayerGoalStart( pGoalee);
		mTable.OnPlayerGoalStart( pGoalee);
	}
	
	public virtual void OnPlayerGoalEnd()
	{
		mOwnerPlayer.OnPlayerGoalEnd();
		mOpponentPlayer.OnPlayerGoalEnd();
		mTable.OnPlayerGoalEnd();
	}
	
	protected override void OnPause (bool pWillPause)
	{
		mOwnerPlayer.OnPause( pWillPause);
		mOpponentPlayer.OnPause( pWillPause);
		mTable.OnPause( pWillPause);
	}
	
	public virtual void Reset()
	{
		mOwnerPlayer.Reset();
		mOpponentPlayer.Reset();
		mTable.Reset();
		OnGameStart();
	}
	
	public virtual void OnGameEnd( PlayerIngame pWinner)
	{
		mOwnerPlayer.OnGameEnd( pWinner);
		mOpponentPlayer.OnGameEnd( pWinner);
		mTable.OnGameEnd( pWinner);
	}
	
	public override void OnUpdate (float pCurrentTime)
	{
		base.OnUpdate (pCurrentTime);
		mOwnerPlayer.OnUpdate( pCurrentTime);
		mOpponentPlayer.OnUpdate( pCurrentTime);
		mTable.OnUpdate( pCurrentTime);
	} 
	#endregion
}
