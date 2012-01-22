using UnityEngine;
using System.Collections;

public class GUIButton1 : GUIObject, GUIPressable, GUIFocusable {
	
#if !UNITY_TOUCH
	public KeyCode[] mShortcutKeys;								//OnPush and OnRelease will be triggered when pressed
#endif
	
	public Color mHoverColor = Color.yellow;					//the color to hover
	public string mTargetColor = "_Color";						//the color name in layer to modify
	protected Material mMat;
	protected Color mOriginalColor;
	bool mIsPressed = false;									//trues if this button was pressed while false if not
	
	protected override void Awake ()
	{
		base.Awake ();
		mMat = renderer.material;
		mOriginalColor = mMat.GetColor( mTargetColor);
		
#if !UNITY_TOUCH
		GameController.mKeyboardEvent +=OnKeyEvent;
#endif
	}

#if !UNITY_TOUCH
	protected override void OnUnInit ()
	{
		base.OnUnInit ();
		GameController.mKeyboardEvent -=OnKeyEvent;
	}
#endif
	
	public bool isPressed
	{
		get{ return mIsPressed; }
		set{ mIsPressed = value;} 
	}
	
	//this will be called when the button was released from push
	//@param 1: the current cursor position
	//@param 2: the current cursor index
	public virtual void OnRelease( Vector3 pCursorPosition, int pCursorIndex)
	{}
	
	//this will be called when the button was push/pressed
	//@param 1: the current cursor position
	//@param 2: the current cursor index
	public virtual void OnPush( Vector3 pCursorPosition, int pCursorIndex )
	{}
	
	//callback on focus on this button
	public virtual void OnFocus (Vector3 pCursorPos)
	{
		mMat.SetColor( mTargetColor,  mHoverColor);
	}
	
	//callback onUnfocus on this button
	public virtual void OnUnfocus (Vector3 pCursorPos)
	{
		mMat.SetColor( mTargetColor,  mOriginalColor);
	}
	
#if !UNITY_TOUCH
	
	bool mIsAllSatisfy;
	//callback on keyevent
	//@param: true if anykey button was pressed while false if not
	public virtual void OnKeyEvent( KeyCode pCode)
	{
		if( mEnabled )
		{
			if( mShortcutKeys != null && mShortcutKeys.Length > 0)
			{
				mIsAllSatisfy = true;
				for( int i=0; i<mShortcutKeys.Length; i++)
				{
					if( pCode != mShortcutKeys[i] )
					{
						mIsAllSatisfy = false;
						break;
					}
				}
				
				if( mIsPressed )
				{
					if( !mIsAllSatisfy && mIsPressed )
					{
						isPressed = false;
						OnRelease( GameController.mCursorScreentPoint, -1);
					}
				}else
				{
					if( mIsAllSatisfy )
					{
						isPressed = true;
						OnPush( GameController.mCursorScreentPoint, -1);
					}
				}
			}
		}
	}
#endif
}
