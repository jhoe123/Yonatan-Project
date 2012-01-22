using UnityEngine;
using System.Collections;

public class LerpAnimator : MonoBehaviour {
	
	public Transform mTransform;						//the transform to be animate
	public Vector3 mStartPos;
	public Vector3 mTargetPos;
	public float mAnimateTime = 2;	
	public LerpAnimatorDelegate mOnEndCallback;			//to be called when end animator
	public float mTimeStarted;
	
	void Start()
	{
		mTimeStarted = Time.time;
	}
	
	void FixedUpdate()
	{
		float time = ( Time.time - mTimeStarted)/mAnimateTime;
		mTransform.position = Vector3.Lerp( mStartPos, mTargetPos, time);
		if( time >= 1)
		{
			if( mOnEndCallback != null )
				mOnEndCallback( this);
			Destroy( this);
		}
	}
}
