using UnityEngine;
using System.Collections;

public class GUITextBox : IGUIUnity {
	
	public int mMaxLength = 10;
	GUIStyle mStyle;
	
	public override GUISkin guiSkin {
		get {
			return base.guiSkin;
		}
		set {
			base.guiSkin = value;
			mStyle = mSkin.textField;
		}
	}
	
	protected override void OnRender ()
	{
		mContent.text = GUI.TextField( mRect, mContent.text, mStyle );
		base.OnRender ();
	}
}
