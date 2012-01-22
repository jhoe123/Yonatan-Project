using UnityEngine;
using System.Collections;

public class GUILabel : IGUIUnity {
	GUIStyle mLabelStyle;
	
	public override GUISkin guiSkin {
		get {
			return base.guiSkin;
		}
		set {
			base.guiSkin = value;
			mLabelStyle = mSkin.label;
		}
	}
	
	protected override void OnRender ()
	{
		//GUI.depth = mRenderPriority;
		GUI.Label( mRect, mContent, mLabelStyle);
		base.OnRender ();
	}
}
