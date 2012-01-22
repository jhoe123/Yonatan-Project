using UnityEngine;
using System.Collections;

public class HoverEffect : Effect {
	
	public bool mReverse = false;
	public float mHeightMove;
	
	Vector3 mStartPos;
	Vector3 mEndPos;
	Vector3 mSStartPos;
	Vector3 mEEndPos;
	Transform mTransform;
	float mElapsed;
	protected override void Start()
	{
		base.Start();
		mTransform = transform;
		mStartPos = mTransform.position;
		mEndPos = mStartPos + ( mTransform.forward * mHeightMove); 
		mSStartPos = mStartPos;
		mEEndPos = mEndPos;
	}
		
	//use to play the effect
	public override bool Play ()
	{
		base.Play ();
		mElapsed = Time.time;
		
		if( mReverse )
		{
			mEEndPos = mTransform.position;
			mStartPos = mEEndPos;
			mEndPos = mSStartPos;
			
			
		}else
		{
			mEEndPos = mSStartPos + ( mTransform.forward * mHeightMove); 
			mStartPos = mSStartPos;
			mEndPos = mEEndPos;
		}
		return true;
	}
	
	void FixedUpdate()
	{
		if( mPlayed )
		{
			float time = ((Time.time - mElapsed) ) / mStopTime;
			mTransform.position = Vector3.Lerp( mStartPos, mEndPos, time);
			if( time >= 1)
				Stop();	
		}
	}
}
