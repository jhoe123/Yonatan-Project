using UnityEngine;
using System.Collections;
using System;

public class IGUIUnity : GUIObject {
	
	public bool mIsFocusable = false;				//true if this object will ablr to focus or not
	public int mRenderPriority = 0;					//this will render on the given priority( like front or back rendering)
	public Rect mRect;
	public GUIContent mContent = new GUIContent();
	protected ArrayList mChildUnity;				//the unity children guis
	int mChildUCount = 0; 
	protected bool mWillRenderAlone = true;			//if true this will render alone, false if will render by parent
	
	#region SKINS
	
	[SerializeField]
	protected GUISkin mSkin;						//the skin to be use
	
	//property that sets the current skin of the gui
	public virtual GUISkin guiSkin
	{
		get{ return mSkin; }
		set
		{
			mSkin = value;
		}
	}
	
	//init
	protected override void Start ()
	{
		base.Start ();
		if( mSkin == null)
			mSkin = GUIController.mDefaultSkin;	
		
		guiSkin = mSkin;
	}
	
	//this sets the skin to child and itself
	//@param: the skin to set
	public void SetSkinToChild( GUISkin pSkin)
	{
		guiSkin = pSkin;
		
		for( int i=0; i<mChildUCount; i++)
			(mChildUnity[i] as IGUIUnity).guiSkin = pSkin;
	}
	
	#endregion
	
	public override void AddChild (GUIObject pObject)
	{
		base.AddChild (pObject);
		IGUIUnity obj = pObject as IGUIUnity;
		if( obj != null )
		{
			if(mChildUnity == null)
				mChildUnity = new ArrayList();
			
			if( obj.mRenderPriority < mChildUCount )
				mChildUnity.Insert( obj.mRenderPriority, pObject);
			else
				mChildUnity.Add( pObject);
			
			mChildUCount ++;
			
			obj.mRenderPriority += mRenderPriority;
			
			//setupRegion
			obj.mWillRenderAlone = false;
			obj.SetupRegion();
		}
	}

	public override void RemoveChild (GUIObject pObject)
	{
		base.RemoveChild (pObject);
		mChildUnity.Remove( pObject);
		mChildUCount--;
	}
	
	public override void SetupRegion ()
	{
		base.SetupRegion ();
		float y = Screen.height - mMaxY;
		
		IGUIUnity unityGUI = parent as IGUIUnity;
		
		if( !mWillRenderAlone )
		{
			if( unityGUI as GUIScrollView != null )
			{	
				if( mCam == null)
				{
					Invoke( "SetupRegion", 0.1f);
					return;
				} 
				
				Bounds bound = unityGUI.renderer.bounds;
				Vector3 tmpMin = mCam.WorldToScreenPoint( bound.min );
				Vector3 tmpMax = mCam.WorldToScreenPoint( bound.max );
				mRect = new Rect( mMinX - tmpMin.x , y - ( Screen.height - tmpMax.y), mMaxX - mMinX, mMaxY - mMinY);
			}
			else
				mRect = new Rect( mMinX, y, mMaxX - mMinX, mMaxY - mMinY);
		}else

			mRect = new Rect( mMinX, y, mMaxX - mMinX, mMaxY - mMinY);
	}
	
	int i = 0;
	protected bool mIsGUIFocus = false;
	protected virtual void OnRender()
	{
		//test for any focus
		if( mIsFocusable )
		{
			if( GUI.tooltip.Length > 0 )
				mIsGUIFocus = true;
			else
				mIsGUIFocus = false;
		}
		
		for( i = 0; i<mChildUCount; i+=4 )
		{
			(mChildUnity[i] as IGUIUnity).OnRender();
			
			if( i+1 < mChildUCount) 
			{
				(mChildUnity[i+1] as IGUIUnity).OnRender();
				
				if( i+2 < mChildUCount )
				{
					(mChildUnity[i+2] as IGUIUnity).OnRender();
				
					if( i+3 < mChildUCount )
					{
						(mChildUnity[i+3] as IGUIUnity).OnRender();	
					}
				}
			}
			
		}
	}
	
	void OnGUI()
	{
		if( enabled && mWillRenderAlone )
		{
			GUI.depth = mRenderPriority;
			OnRender();
		}
	}
}
