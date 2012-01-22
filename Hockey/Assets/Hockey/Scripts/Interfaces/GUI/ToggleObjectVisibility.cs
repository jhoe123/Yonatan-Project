using UnityEngine;
using System.Collections;

public class ToggleObjectVisibility : GUIButton_SFX {

	public bool mWillShow;
	public GameObject mObject2ToggleTrue;
	public GameObject mObject2ToggleFalse;
	
	public override void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnRelease (pMousePosition, pTouchIndex);
		if( mObject2ToggleTrue != null )
		{
			mObject2ToggleTrue.SetActiveRecursively( mWillShow );
			//mObject2ToggleTrue.SendMessage( "SetEnableRecursively", mWillShow);
		}
		
		if( mObject2ToggleFalse != null )
		{
			mObject2ToggleFalse.SetActiveRecursively( !mWillShow );
			//mObject2ToggleFalse.SendMessage( "SetEnableRecursively", !mWillShow);
		}
	}
	
}
