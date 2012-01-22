using UnityEngine;
using System.Collections;

public class RotateTransform : Effect {
	
	public Transform mTargetTransform;    				//the target transform to be rotated
	public Vector3 mRotateValues = Vector3.one;			//this will make the transform rotate
	
	protected override void Start ()
	{
		if( mTargetTransform == null )
			mTargetTransform = transform;
		base.Start ();
	}
	
	public override bool Play ()
	{
		if( base.Play ())
		{
			if( mUpdateOnPause )
				GameController.mUpdateRealTimeDelegate += OnUpdate;
			else
				GameController.mUpdateDelegate += OnUpdate;
			return true;
		}
		else
			return false;
	}
	
	public override void Stop ()
	{
		base.Stop ();
		if( mUpdateOnPause )
			GameController.mUpdateRealTimeDelegate -= OnUpdate;
		else
			GameController.mUpdateDelegate -= OnUpdate;
	}
	
	void OnUpdate( float pTime )
	{
		mTargetTransform.Rotate( mRotateValues);
	}
}
