using UnityEngine;
using System.Collections;

/*
 * This destroy the given object when played
 * */
public class ObjectDestroyer : Effect {
	
	public GameObject mObject2Destroy;				//the object to destroy when got destroyed
	
	protected override void Start ()
	{
		base.Start ();
		if( mObject2Destroy == null)
			mObject2Destroy = gameObject;
	}
	
	public override bool Play ()
	{
		SObject.DeleteObject( mObject2Destroy );
		return base.Play ();
	}
}
