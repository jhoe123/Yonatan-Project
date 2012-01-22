using UnityEngine;
using System.Collections;

public class ScaleAnimator : Effect {

	public Vector3 mStartScale =  Vector3.zero;						//the start color to be animate
	public Vector3 mEndScale = Vector3.one;							//the end color to animate
	public bool mIsPingpongLoop = true;								//this creates a smooth loop
	Material mMat;															
	float mElapsed;
	Transform mTransform;
	
	//to be called when this script starts
	protected override void Start ()
	{
		base.Start ();
		mMat = renderer.material;
		mTransform = transform;
	}
	
	//to be called when this will be starting
	public override bool Play ()
	{
		if( base.Play () )
		{	
			transform.localScale = mStartScale;
			GameController.mUpdateRealTimeDelegate += OnUpdate;
			mElapsed = Time.realtimeSinceStartup;
			return true;
		}else
			return false;
	}
	
	//to be called when this effect will be repeated
	//@param: the total repeatation
	protected override void OnRepeat (int pRepeatCount)
	{
		base.OnRepeat (pRepeatCount);
		if( mIsPingpongLoop )
		{
			Vector3 tmpScale = mStartScale;
			mStartScale = mEndScale;
			mEndScale = tmpScale;
		}
	}
	
	//to stop the effect
	public override void Stop ()
	{
		base.Stop ();
		GameController.mUpdateRealTimeDelegate -= OnUpdate;
	}
	
	void OnUpdate( float pCurrentTime )
	{
		if( mTransform != null )
		{
			float time = ( pCurrentTime - mElapsed )/ mStopTime; 
			mTransform.localScale = Vector3.Slerp( mStartScale, mEndScale, time);
		}
	}
}
