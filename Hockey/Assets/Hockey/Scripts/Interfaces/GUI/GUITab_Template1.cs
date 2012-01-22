using UnityEngine;
using System.Collections;

public class GUITab_Template1 : GUITab {
	public Color mSelectedColor = Color.yellow;
	public string mTargetColor = "_Emission";
	
	protected override void Start ()
	{
		base.Start ();
		OnSetItem( mCurrentItem );
	}
	
	public override void OnSetItem (GUITabItem pItem)
	{
		if( mCurrentItem != null )
			mCurrentItem.renderer.material.SetColor( mTargetColor, Color.gray);
		
		base.OnSetItem (pItem);
		mCurrentItem.renderer.material.SetColor( mTargetColor, mSelectedColor);
	}
}
