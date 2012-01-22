using UnityEngine;
using System.Collections;

public class PlayerIngame : MonoBehaviour {
	
	PlayerInfo mInfo;
	[SerializeField]
	GameObject mBallGuard;				//the ballguard being controlled by player
	
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
	
	#region CALLBACKS
	public virtual void OnGameStart()
	{}
	
	public virtual void OnIntroStart()
	{}
	
	public virtual void OnIntroEnd()
	{}
	
	public virtual void OnPlayerGoalStart( PlayerIngame pGoalee)
	{}
	
	public virtual void OnPlayerGoalEnd()
	{}
	
	public virtual void OnPause (bool pWillPause)
	{}
	
	public virtual void Reset()
	{}
	
	public virtual void OnGameEnd( PlayerIngame pWinner)
	{}
	
	public virtual void OnUpdate( float pCurrentTime)
	{}
	#endregion
}
