using UnityEngine;
using System.Collections;

public class PlayerIngameNetwork : PlayerIngame {
	
	Transform mBallTransform;
	Rigidbody mBallRigid;
	NetworkView mView;
	bool mIsCurrentPlayer;
	Vector3 mLastPosition;
	Vector3 mCurrentPosition;
	
	public override PlayerInfo info {
		get {
			return base.info;
		}
		set {
			base.info = value;
			if( GameplayScene.currentPlayer != this )
				mIsCurrentPlayer = false;
			else
				mIsCurrentPlayer = true;
		}
	}
	
	protected override void Awake ()
	{
		base.Awake ();
		mView = networkView;
		mBallRigid = mBallGuard.rigidbody;
		mBallTransform = mBallGuard.transform;
	} 
	
	[RPC]
	//@Host: callback when the client change position
	void OnGuardChangedPosition( Vector3 pPosition )
	{
		mBallRigid.MovePosition( pPosition);
	}
	
	public override void OnUpdate (float pCurrentTime)
	{
		base.OnUpdate (pCurrentTime);
		
		if( mIsCurrentPlayer )
		{
			mCurrentPosition = mBallRigid.position;
			if( mLastPosition != mCurrentPosition )
			{
				mLastPosition = mCurrentPosition;
				mView.RPC( "OnGuardChangedPosition", RPCMode.Others, mCurrentPosition);
			}
		}
	}

}
