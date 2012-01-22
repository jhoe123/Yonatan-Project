using UnityEngine;
using System.Collections;

/*
 * implementation for scroll bar
 * 
 * How to use:
 *		- this requires a guiprogressbar
 *		- to do some event when a value changed. override the OnScroll() method
 * */
[RequireComponent( typeof(GUIProgressbar) )]
public class GUIScrollBar : GUIButton_SFX {
	
	public Transform mButtonHandle;
	GUIProgressbar mProgressBar;
	float mWidth;
	Vector3 mPosition;
	protected float mCurrentValue;									//the current value of scrollbar
	
	protected override void Awake ()
	{
		base.Awake ();
		mProgressBar = GetComponent<GUIProgressbar>();
		if( mButtonHandle != null )	
			mPosition = mButtonHandle.position;
	}
	
	float mMinXW;
	float mWWidth;
	public override void SetupRegion ()
	{
		base.SetupRegion ();
		mMinXW = meshBound.min.x;
		mWWidth = meshBound.max.x - meshBound.min.x;
		mWidth = mMaxX - mMinX;
	}
	
	//use to scroll the scroll bar
	//@param: the cursor x position
	//@return: return if scrolled while false if not
	public void ScrollByInput( float pXPos )
	{
		if( pXPos < mMaxX && pXPos > mMinX)
		{
			float maxVal = mProgressBar.mMaxValue;
			if( mButtonHandle != null )
			{
				mPosition.x = (( (pXPos - mMinX) * mWWidth)/mWidth) + mMinXW;
				mButtonHandle.position = mPosition;
			}	
			
			mCurrentValue = ((pXPos - mMinX) * maxVal)/ mWidth;
			mProgressBar.valueP = mCurrentValue;
			OnScroll( mCurrentValue);
		}
	}
	
	public void ScrollByVal( float pVal )
	{
		//Debug.Log( pVal + " " + (mMinX + ((pVal * mWidth)/mProgressBar.mMaxValue)) );
		//compute for xpos
		ScrollByInput( mMinX + ((pVal * mWidth)/mProgressBar.mMaxValue));
	}
	
	//to be called when this got scrolled
	//@param: the current value of scrolledåå
	protected virtual void OnScroll( float pValue )
	{
		
	}
	
	public override void OnPush (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnPush (pMousePosition, pTouchIndex);
		GameController.mUpdateRealTimeDelegate += OnUpdate;
	}
	
	public override void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		base.OnRelease (pMousePosition, pTouchIndex);
		GameController.mUpdateRealTimeDelegate -= OnUpdate;
	}
	
	public override void OnUpdate (float mCurrentTime)
	{
		base.OnUpdate (mCurrentTime);
		if( isPressed )
			ScrollByInput( GameController.mCursorScreentPoint.x );
	}
}
