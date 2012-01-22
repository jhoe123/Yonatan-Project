using UnityEngine;
using System.Collections;

public class GUITabItem :  GUIObject, GUIPressable {

	GUITab mParentTab;				//the parent tab
	bool mIsUsed = false;
	
	protected override void Start ()
	{
		base.Start ();
		mParentTab = parent as GUITab;
		isUsed = false;
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
		isUsed = false;
	}
	
	//true if the item is used while false if not
	public virtual bool isUsed
	{
		get{ return mIsUsed; }
		set
		{
			foreach( Transform trans in transform)
				trans.gameObject.active = value;
		}
	}
	
	bool mIsPressed = false;
	public bool isPressed
	{
		get{ return mIsPressed;}
		set{ mIsPressed = value;}
	}
	
	public virtual void OnPush( Vector3 pMousePosition, int pTouchIndex)
	{}
	
	public virtual void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		mParentTab.OnSetItem( this );
	}
}
