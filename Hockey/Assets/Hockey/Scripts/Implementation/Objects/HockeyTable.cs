using UnityEngine;
using System.Collections;

public class HockeyTable : MonoBehaviour {
	
	[SerializeField]
	GameObject mBall;
	public GameObject ball
	{ get{ return mBall; }}
	
	[SerializeField]
	GameObject mTopGuard;
	public GameObject topGuard
	{	get{return mTopGuard;}}
	
	[SerializeField]
	GameObject mBottomGuard;
	public GameObject bottomGuard
	{	get{return mBottomGuard;}}
	
	Rigidbody mRigid;
	public Rigidbody ballBody
	{ get{ return mRigid; }}
	
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
