using UnityEngine;
using System.Collections;

public class GUIButton2 : GUIButton1 {

	public Texture mButtonDown;
	public Texture mHoverTex;
	protected Texture mNormalTex;
	
	protected override void Start ()
	{
		base.Start ();
		if( mMat!=null)
			mNormalTex = mMat.GetTexture( "_MainTex");
	}
	
	public override void OnPush (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnPush (pMousePosition, pTouchIndex);
		
		if( mButtonDown != null)
			mMat.SetTexture("_MainTex", mButtonDown );
	}
	
	public override void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnRelease (pMousePosition, pTouchIndex);
		
		if( mButtonDown != null)
			mMat.SetTexture("_MainTex", mNormalTex);
	}
	
	public override void OnFocus (Vector3 pCursorPos)
	{
		base.OnFocus (pCursorPos);
		
		if( mHoverTex != null )
			mMat.SetTexture("_MainTex", mHoverTex);
	}
	
	public override void OnUnfocus (Vector3 pCursorPos)
	{
		base.OnUnfocus (pCursorPos);
		
		if( mHoverTex != null )
			mMat.SetTexture("_MainTex", mNormalTex);
	}
}

