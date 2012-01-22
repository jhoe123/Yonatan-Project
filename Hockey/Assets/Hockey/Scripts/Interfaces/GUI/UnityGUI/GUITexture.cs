using UnityEngine;
using System.Collections;

public class GUITexture : IGUIUnity {
	
	public Texture mTexture;						//the texture to be render
	public ScaleMode mMode = ScaleMode.StretchToFill;//the mode for scaling
	public bool mIsAlphaBlend = true;				//true if alpha blend while false of not
	public float mAspect = 1;						//the adpect of the texture
	
	protected override void OnRender ()
	{
		if( enabled )
		{
			//GUI.depth = mRenderPriority;
			GUI.DrawTexture( mRect, mTexture, mMode, mIsAlphaBlend, mAspect);
			base.OnRender ();
		}
	}
}
