using UnityEngine;
using System.Collections;

public class PlayerIngame : MonoBehaviour {
	
	PlayerInfo mInfo;
	[SerializeField]
	GameObject mBallGuard;				//the ballguard being controlled by player
	[SerializeField]
	GameObject mScoreProto;				//to be instantiated
	
	GameObject[] mScoreGUIs;
	int mCurrentScore = 0;
	
	#region PROPERTIES
	public GameObject ballGuard
	{
		get{ return mBallGuard;}
	}
	
	public PlayerInfo info
	{
		get{ return mInfo;}
		set
		{
			mInfo = value;
		}
	}
	#endregion
	
	//constructor
	protected virtual void Awake()
	{
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
	}
	
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
