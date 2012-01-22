using UnityEngine;
using System.Collections;

public class GUIProgressbar : GUIObject {
	
	public Transform mParentTransform;				//the transform to be scaled
	float mCurrentValue = 100;						//the current value. just use for initiali
	public float mMaxValue = 100;					//the max value
	Vector3 mCurrentScale;
	float mScaleX;
	
	protected override void Start ()
	{
		base.Start ();
		if( mParentTransform == null )
			mParentTransform = mTransform;
			
		mCurrentScale = mParentTransform.localScale;
		mScaleX = mCurrentScale.x;
	}
	
	public void Reset()
	{
		valueP = mMaxValue;
	}
	
	public virtual float  valueP
	{
		get{ return mCurrentValue; }
		set
		{
			if( value >= 0 )	
				mCurrentValue = value;
			else
				mCurrentValue = 0; 
				
			mCurrentScale.x = (mScaleX * value)/mMaxValue;
			mParentTransform.localScale = mCurrentScale;
		}
	}
}
