using UnityEngine;
using System.Collections;

/*
 * PlayerController.cs
 * Jhoemar Pagao (c) 2011
 * jhoemar.pagao@gmail.com
 * 
 * Use to control the assigned player
 * */
public class PlayerController : GUIObject, GUIPressable {
	
	[SerializeField]
	PlayerIngame mPlayer;
	public PlayerIngame player
	{ get{ return mPlayer;}}
	
	Transform mGuardTrans;
	Rigidbody mGuardRigid;
	Camera mWordCam;
	float mY;							
	int mTableLayer;							//the layer tag
	
	float mMidLimit = 7.5f;						//the limit
	bool mIsBottomPlayer = false;
	
	//initialization
	protected override void Start ()
	{	
		if( mPlayer != null )
			Initialize( mPlayer);
	}
	
	public void Initialize( PlayerIngame pPlayer)
	{
		mPlayer = pPlayer;
		mGuardTrans = player.ballGuard.transform;
		mWordCam = Camera.mainCamera;
		base.Start();
		mY = mGuardTrans.position.y;
		mGuardRigid = mGuardTrans.rigidbody;
		mTableLayer = 1 << (LayerMask.NameToLayer("Table") );
		
		if( GameplayScene.bottomPlayer == mPlayer)
			mIsBottomPlayer = true;
		else
			mIsBottomPlayer = false;
		
		//init camera location and alignment
		mWordCam.transform.position = mGuardTrans.position + (mGuardTrans.forward * -4 ) + new Vector3(0, 5,0);
		mWordCam.transform.LookAt( GameplayScene.table.ball.transform);
	}
	
	//callback when start pressing 
	public void OnPush (Vector3 pCursorPosition, int pCursorIndex)
	{
		ScreenToWorld();
	}
	
	//callback when release pressing
	public void OnRelease (Vector3 pCursorPosition, int pCursorIndex)
	{}
	
	bool mIsPressed = false;
	public bool isPressed {
		get {
			return mIsPressed;
		}
		set {
			mIsPressed = value;
		}
	}
	
	//move the player guard based on the input
	RaycastHit mResult;
	void ScreenToWorld()
	{
		Physics.Raycast( mWordCam.ScreenPointToRay( GameController.mCursorScreentPoint), out mResult, Mathf.Infinity, mTableLayer);
		mClipPosition = mResult.point;
		mClipPosition.y = mY;
		
		//limit the player guard.
		if( mIsBottomPlayer)
		{
			if( mClipPosition.z > mMidLimit )
				mClipPosition.z = mMidLimit;
		}else
		{
			if( mClipPosition.z < mMidLimit )
				mClipPosition.z = mMidLimit;
		}
		   
		mGuardRigid.MovePosition( mClipPosition);
	}
	
	Vector3 mClipPosition;
	public override void OnUpdate (float mCurrentTime)
	{
		base.OnUpdate (mCurrentTime);
		if( mIsPressed )
			ScreenToWorld();
	}
}