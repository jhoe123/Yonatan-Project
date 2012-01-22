using UnityEngine;
using System.Collections;

public class PositionAnimator : Effect {
	
	public bool mLocalPoints = true;				//true if this os local points while false if world
	public Vector3[] mPoints;						//the position to move
	Vector3 mOrigStartPos;
	Vector3[] mAnimatedValues;
	int mCurrentI;
	Vector3 mStartPos;
	Vector3 mTargetPos;
	Transform mTransform;
	Vector3 mCurrentPosition;
	float mElapsed;
	float mAnimateTime;
	int mPointsLength;
	
	protected override void Start ()
	{
		base.Start ();
		mTransform = transform;
		mCurrentPosition = mTransform.position;
		
		mOrigStartPos = mCurrentPosition;
		SetupPosition();
	}
	
	public void SetStartPosition( Vector3 pPosition )
	{
		mOrigStartPos = pPosition;
	}
	
	void SetupPosition()
	{
		mPointsLength = mPoints.Length;
		mAnimateTime = mStopTime / mPointsLength;
		if( mLocalPoints )
		{
			mAnimatedValues = new Vector3[mPointsLength];
			Vector3 mTmp = mTransform.position;
			for( int i=0; i< mPointsLength; i++)
			{
				mTmp = mPoints[i] + mTmp;
				mAnimatedValues[i] = mTmp;
			}
		}else
			mAnimatedValues = mPoints;
	}
	
	public override bool Play ()
	{
		if( base.Play ())
		{
			if( !mUpdateOnPause )
				GameController.mUpdateDelegate += OnUpdate;
			else
				GameController.mUpdateRealTimeDelegate += OnUpdate;
			
			mStartPos = mTransform.position;
			//if( mTransform.position != mOrigStartPos )
				SetupPosition();
			
			mCurrentI = 0;
			mTargetPos = mAnimatedValues[mCurrentI];
			mElapsed = Time.time;
			return true;
		}
		
		return false;
	}
	
	public override void Stop ()
	{
		if( mPlayed )
		{
			if( !mUpdateOnPause )
				GameController.mUpdateDelegate -= OnUpdate;
			else
				GameController.mUpdateRealTimeDelegate -= OnUpdate;
			
			base.Stop ();
		}
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
		Stop();
	}
	
	float time;
	protected virtual void OnUpdate( float pCurrentTime )
	{
		time = (pCurrentTime - mElapsed )/ mAnimateTime;
		mCurrentPosition = Vector3.Lerp( mStartPos, mTargetPos, time);
		if( mCurrentI+1 < mPointsLength && time > 1)
		{
			mStartPos = mTargetPos;
			mCurrentI++;
			mTargetPos = mAnimatedValues[mCurrentI];
			mElapsed = pCurrentTime;
		}
		mTransform.position = mCurrentPosition;
	}
}
