using UnityEngine;
using System.Collections;

public class RotationAnimator : Effect {
	
	public Vector3[] mRotationVal;
	public bool mIsAnimateLocal = true;
	Transform mTransform;
	
	protected override void Start ()
	{
		base.Start ();
		mTransform = transform;
	}
	
	public override bool Play ()
	{
		if( base.Play ())
		{
			if( mIsAnimateLocal )
			{
				
			}
			
			return true;
		}
		
		return false;
	}
}
