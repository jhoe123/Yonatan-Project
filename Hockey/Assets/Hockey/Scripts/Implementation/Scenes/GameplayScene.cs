using UnityEngine;
using System.Collections;

/*
 * GameplayScene.cs
 * Jhoemar Pagao (c) 2011
 * jhoemar.pagao@gmail.com
 * 
 * Controls the gameplay state of the game
 * */
public class GameplayScene : Scene {
	
	public static GameplayScene mCurrent;								//the current instance of the game
	PlayerController mPlayerController;
	
	//guis
	public GameObject mTopGUI_Scorer;
	public GameObject mBotGUI_Scorer;
	
	[SerializeField]
	protected int mMaxGoal;
	
	[SerializeField]
	protected PlayerIngame mPlayer1;									//the player 1 instance
	
	[SerializeField]
	protected PlayerIngame mPlayer2;									//the player instance
	
	static protected PlayerInfo mOwnerInfo;
	static protected PlayerInfo mOpponentInfo;
	
	#region PROPERTIES
	
	//the current player who is controlling this device
	protected static PlayerIngame mCurrentPlayer;
	public static PlayerIngame currentPlayer
	{ get{ return mCurrentPlayer;}}
	
	//the player host of this game
	protected static PlayerIngame mOwnerPlayer;
	public static PlayerIngame ownerPlayer
	{ get{return mOwnerPlayer;}}
	
	//the player opponent of the host
	protected static PlayerIngame mOpponentPlayer;
	public static PlayerIngame opponentPlayer
	{ get{return mOpponentPlayer;}}
	
	//the player who occupied the bottom
	public static PlayerIngame bottomPlayer
	{ get{ return mCurrent.mPlayer1;}}
	
	//the player who occupied the top
	public static PlayerIngame topPlayer
	{ get{ return mCurrent.mPlayer2;}}
	
	//the current game information ongoing
	protected static GameInfo mGameinfo;
	public static GameInfo gameInfo
	{ get{ return mGameinfo;}}
	
	//the hockey table
	[SerializeField]
	HockeyTable mTable;
	public static HockeyTable table
	{ get{ return mCurrent.mTable;}}
	
	//the max goal
	public static int maxGoal
	{ get{ return mCurrent.mMaxGoal;}}
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
	
	//callback when this was initialize
	protected override void Awake ()
	{
		mCurrent = this;
		
		base.Awake ();
		
		if( mCurrentPlayer == null)
			mCurrentPlayer = mOwnerPlayer;
		
		if( mOwnerInfo == null )
		{
			mOwnerInfo = GameHelpers.GetLocalPlayerInfo();
			mOpponentInfo = mOwnerInfo;
		}
		
		mGameinfo = new GameInfo( mOwnerInfo, mOpponentInfo, 0, 0);
		ownerPlayer.info = mOwnerInfo;
		opponentPlayer.info = mOpponentInfo;
		
		mPlayerController = (PlayerController)FindObjectOfType( typeof(PlayerController));
	}
	
	//callback when the game started
	public virtual void OnGameStart()
	{
		mOwnerPlayer.OnGameStart();
		mOpponentPlayer.OnGameStart();
		mTable.OnGameStart();
	}
	
	//callback when intro started
	public virtual void OnIntroStart()
	{
		mOwnerPlayer.OnIntroStart();
		mOpponentPlayer.OnIntroStart();
		mTable.OnIntroStart();
	}
	
	//callbakc when intro end
	public virtual void OnIntroEnd()
	{
		mOwnerPlayer.OnIntroEnd();
		mOpponentPlayer.OnIntroEnd();
		mTable.OnIntroEnd();
	}
	
	//callback when a player goal
	//@param: the player who goaled
	public virtual void OnPlayerGoalStart( PlayerIngame pGoalee)
	{
		//update current game score
		if( pGoalee == mOwnerPlayer )
			mGameinfo.ownerScore ++;
		else
			mGameinfo.opponentScore++;
		
		Debug.Log( "Player GOAL: " + pGoalee.name);
		mOwnerPlayer.OnPlayerGoalStart( pGoalee);
		mOpponentPlayer.OnPlayerGoalStart( pGoalee);
		mTable.OnPlayerGoalStart( pGoalee);
		
		//end short after few seconds
		Invoke( "OnPlayerGoalEnd", 2);
	}
	
	//callback when ending the goal
	protected virtual void OnPlayerGoalEnd()
	{
		mOwnerPlayer.OnPlayerGoalEnd();
		mOpponentPlayer.OnPlayerGoalEnd();
		mTable.OnPlayerGoalEnd();
		
		//test if end game
		if( mGameinfo.ownerScore >= mMaxGoal )
			OnGameEnd( mOwnerPlayer);
		else if( mGameinfo.opponentScore >= mMaxGoal )
			OnGameEnd( mOpponentPlayer);
	}
	
	//callback when the game was pause
	protected override void OnPause (bool pWillPause)
	{
		mOwnerPlayer.OnPause( pWillPause);
		mOpponentPlayer.OnPause( pWillPause);
		mTable.OnPause( pWillPause);
	}
	
	//callback when the game was reset
	public virtual void Reset()
	{
		mOwnerPlayer.Reset();
		mOpponentPlayer.Reset();
		mTable.Reset();
		OnGameStart();
	}
	
	//callback when game ended
	//@param: the player who winned
	public virtual void OnGameEnd( PlayerIngame pWinner)
	{
		mOwnerPlayer.OnGameEnd( pWinner);
		mOpponentPlayer.OnGameEnd( pWinner);
		mTable.OnGameEnd( pWinner);
		pause = true;
		Debug.Log( "GameOver");
	}
	
	//callback for updating
	public override void OnUpdate (float pCurrentTime)
	{
		base.OnUpdate (pCurrentTime);
		mOwnerPlayer.OnUpdate( pCurrentTime);
		mOpponentPlayer.OnUpdate( pCurrentTime);
		mPlayerController.OnUpdate( pCurrentTime);
		//mTable.OnUpdate( pCurrentTime);
	} 
	#endregion
	
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		mCurrent = null;
	}
}
