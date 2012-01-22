using UnityEngine;
using System.Collections;

public class GUIUnityButton : IGUIUnity {

	protected override void OnRender ()
	{
		if( GUI.Button( mRect, mContent ))
		  OnRelease();
		   
		base.OnRender ();
	}
	
	public virtual void OnRelease()
	{
		
	}

}
