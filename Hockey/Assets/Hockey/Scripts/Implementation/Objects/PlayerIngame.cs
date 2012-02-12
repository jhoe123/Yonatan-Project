using UnityEngine;
using System.Collections;

public class PlayerIngame : MonoBehaviour {
	
	PlayerInfo mInfo;
	[SerializeField]
	protected GameObject mBallGuard;	//the ballguard being controlled by player
	GameObject mScoreProto;				//to be instantiated
	
	GameObject[] mScoreGUIs;
	int mCurrentScore = 0;
	
	#region PROPERTIES
	public GameObject ballGuard
	{
		get{ return mBallGuard;}
	}
	
	//the current player information
	public virtual PlayerInfo info
	{
		get{ return mInfo;}
		set
		{
			//for current player use the bottom scorer
			if( this == GameplayScene.bottomPlayer )
				mScoreProto = GameplayScene.mCurrent.mBotGUI_Scorer;
			else
				mScoreProto = GameplayScene.mCurrent.mTopGUI_Scorer;
			
			//initialize the scorer
			mScoreGUIs = new GameObject[ GameplayScene.maxGoal ];
			mScoreGUIs[0] = mScoreProto;
			Vector3 pos = mScoreProto.transform.position;
			for( int i=1; i<mScoreGUIs.Length; i++)
			{
				mScoreGUIs[i] = (GameObject)Instantiate( mScoreProto);
				mScoreGUIs[i].transform.position = pos + (Vector3.left *-i* 15);
				mScoreGUIs[i].active = false;
			}
			mScoreGUIs[0].active = false;
			mCurrentScore = 0;
				
			mInfo = value;
		}
	}
	#endregion
	
	//constructor
	protected virtual void Awake()
	{}
	
	#region CALLBACKS
	public virtual void OnGameStart()
	{}
	
	public virtual void OnIntroStart()
	{}
	
	public virtual void OnIntroEnd()
	{}
	
	public virtual void OnPlayerGoalStart( PlayerIngame pGoalee)
	{
		if( pGoalee == this)
		{
			mScoreGUIs[mCurrentScore].active = true;
			mCurrentScore++;
		}
	}
	
	public virtual void OnPlayerGoalEnd()
	{}
	
	public virtual void OnPause (bool pWillPause)
	{}
	
	public virtual void Reset()
	{
		mCurrentScore = 0;
		for( int i=0; i<mScoreGUIs.Length; i++)
			mScoreGUIs[i].active = false;
	}
	
	public virtual void OnGameEnd( PlayerIngame pWinner)
	{}
	
	public virtual void OnUpdate( float pCurrentTime)
	{}
	#endregion
}
