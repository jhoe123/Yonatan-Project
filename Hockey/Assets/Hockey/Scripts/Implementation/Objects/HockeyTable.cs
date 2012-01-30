using UnityEngine;
using System.Collections;

public class HockeyTable : MonoBehaviour {
	
	//reset varialbles
	Vector3 mBallResetPos;
	Vector3 mGTopResetPos;
	Vector3 mBTopResetPos;
	
	#region PROPERTIES
	//the main ball
	[SerializeField]
	Ball mBall;
	public Ball ball
	{ get{ return mBall; }}
	
	//the top guard
	[SerializeField]
	GameObject mTopGuard;
	public GameObject topGuard
	{	get{return mTopGuard;}}
	
	//the bottom guard
	[SerializeField]
	GameObject mBottomGuard;
	public GameObject bottomGuard
	{	get{return mBottomGuard;}}
	
	//the top goal
	[SerializeField]
	GameObject mTopGoal;
	public GameObject topGoal
	{	get{return mTopGoal;}}
	
	//the bottom goal
	[SerializeField]
	GameObject mBottomGoal;
	public GameObject bottomGoal
	{	get{return mBottomGoal;}}
	
	//the ball rigid body
	Rigidbody mRigid;
	public Rigidbody ballBody
	{ get{ return mRigid; }}
	#endregion
	
	protected virtual void Awake()
	{
		mBallResetPos = mBall.transform.position;
		mGTopResetPos = mTopGuard.transform.position;
		mBTopResetPos = mBottomGuard.transform.position;
		mRigid = mBall.rigidbody;
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
		mRigid.velocity = Vector3.zero;
	}
	
	//when the goal end
	public virtual void OnPlayerGoalEnd()
	{
		Reset();
	}
	
	public virtual void OnPause (bool pWillPause)
	{}
	
	//use to reset the table and its entities
	public virtual void Reset()
	{
		mBall.transform.position = mBallResetPos;
		mTopGuard.transform.position = mGTopResetPos;
		mBottomGuard.transform.position = mBTopResetPos;
		mRigid.velocity = Vector3.zero;
	}
	
	public virtual void OnGameEnd( PlayerIngame pWinner)
	{}
	
	public virtual void OnUpdate( float pCurrentTime)
	{}
	#endregion
}
