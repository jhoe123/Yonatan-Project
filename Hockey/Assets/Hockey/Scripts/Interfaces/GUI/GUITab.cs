using UnityEngine;
using System.Collections;

public class GUITab : GUIObject {

	public GUITabItem mCurrentItem;				//the current item of the tab
	
	public GUITabItem[] mItems;						//the items in the current tab
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		foreach( Transform trans in transform )
			trans.gameObject.active = true;
		
		if( mCurrentItem != null )
			mCurrentItem.isUsed = true;	
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
		foreach( Transform trans in transform )
			trans.gameObject.active = false;
	}
	
	//to be called when swtting the item for this tab
	//@param: the item to be setted 
	public virtual void OnSetItem( GUITabItem pItem )
	{
		if( mCurrentItem != null )
			mCurrentItem.isUsed = false;
		
		mCurrentItem = pItem;
		pItem.isUsed = true;
	}
}
