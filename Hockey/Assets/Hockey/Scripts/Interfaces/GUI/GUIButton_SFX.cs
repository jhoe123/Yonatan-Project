using UnityEngine;
using System.Collections;

public class GUIButton_SFX : GUIButton2 {

	public AudioClip mOnPushSound;
	public AudioClip mOnReleaseSound;
	
	public override void OnPush (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnPush (pMousePosition, pTouchIndex);
		if( mOnPushSound != null )
			GUIController.PlayGUIAudio( mOnPushSound, 1);
	}
	
	public override void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnRelease (pMousePosition, pTouchIndex);
		if( mOnReleaseSound != null )
			GUIController.PlayGUIAudio( mOnReleaseSound, 1);
	}
	
}
