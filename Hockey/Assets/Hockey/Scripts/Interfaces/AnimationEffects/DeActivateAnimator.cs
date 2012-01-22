using UnityEngine;
using System.Collections;

/*
 * An effect that deactivates/activate the gameobject
 * */
public class DeActivateAnimator : Effect {
	
	public GameObject mObject2Process;				//the object to be manipulated
	public bool mWillActivate = false;				//true if the object2process will be activated while false if deactivate
	
	protected virtual void Awake()
	{
		if( mObject2Process == null)
			mObject2Process = gameObject;	
	}
	
	public override bool Play ()
	{
		//if( base.Play () )
		//{
			mObject2Process.SetActiveRecursively( mWillActivate );
		//	return true;
		//}
		//else
		//	return false;
		return base.Play();
	}
	
	public override void Stop ()
	{
		base.Stop ();
		//Debug.Log( mStopTime);
		mObject2Process.SetActiveRecursively( !mWillActivate );
		//Debug.Log(name);

	}
}
