using UnityEngine;
using System.Collections;

public class GUIScrollView : IGUIUnity {
	
	protected Vector2 mScrollPos;											//the scroll position
	protected Rect mViewRect = new Rect( 0, 0, 400,400);					//the view rect
	protected GUIStyle mHorizontalStyle;									//the style for horizontal bar
	protected GUIStyle mVerticalStyle;										//the style for vertical bar
	
	public Vector2 scrollPosition
	{
		get{ return mScrollPos; }
		set{ mScrollPos = value; }
	}
	
	public override GUISkin guiSkin {
		get {
			return base.guiSkin;
		}
		set {
			base.guiSkin = value;
			mHorizontalStyle = mSkin.horizontalScrollbar;
			mVerticalStyle = mSkin.verticalScrollbar;
		}
	}
	
	public override void AddChild (GUIObject pObject)
	{
		base.AddChild (pObject);
		SetupRegion();
		mScrollPos.y = 100000;
	}
	
	public override void SetupRegion ()
	{
		base.SetupRegion ();
		if( mChildren != null )
		{
			float maxY = mMinY;
			float maxX = 0;
			for( int i=0; i < mChildren.Count; i++ )
			{
				GUIObject obj = mChildren[i] as GUIObject;
				if( obj.min.y < maxY )
					maxY = obj.min.y;
				
				if( obj.max.x > maxX )
					maxX = obj.max.x;
			}
			
			mViewRect.width = maxX - mMinX;
			mViewRect.height = mMaxY - maxY;
		}
		//mScrollPos
	}
	
	//use to get the gui by its screen position
	//@param: the screen position
	//@return: return the gui at screen pos while null if nothing
	public IGUIUnity GetGUI( Vector2 pScreenPosition )
	{
		pScreenPosition.x = (pScreenPosition.x * mRect.width)/Screen.width;
		pScreenPosition.y = ((pScreenPosition.y + mScrollPos.y) * ( Mathf.Abs((mChildUnity[0] as GUIObject).max.y - (mChildUnity[mChildUnity.Count-1] as GUIObject).max.y)))/Screen.height;
		Debug.Log( pScreenPosition);
		
		int l= mChildUnity.Count;
		for( int i=0; i<l; i++)
		{
			if( (mChildUnity[i] as IGUIUnity).mRect.Contains( pScreenPosition))
				return mChildUnity[i] as IGUIUnity;
		}
		return null;
	}
	
	//use to clear the content for the scroll view
	//@param: the arrays of guiobject which will not be deleted
	public void Clear( GUIObject[] pExceptions)
	{
		if( pExceptions != null )
		{
			//Debug.Log( "Exception " + (pExceptions[0] as GUILabel).mContent.text );
			for( int i=0; i< mChildUnity.Count; i++)
			{
				GUIObject obj = mChildUnity[i] as GUIObject;
				for( int j=0; j<pExceptions.Length; j++)
				{
					//Debug.Log( obj.mContent.text);
					if( obj == pExceptions[j])
						continue;
					
					//Debug.Log("Deleted " + obj.mContent.text);
					DeleteObject( obj);
					Destroy( obj.gameObject );
					Destroy( obj);
					RemoveChild( obj);
				}
			}
			
			SetupRegion();
		}else
			Clear();
	}
	
	public void Clear()
	{
		for( int i=0; i<mChildren.Count; i++)
			DeleteObject( mChildren[i] as GUIObject);
		
		mChildren.Clear();
		SetupRegion();
	}
	
	//use to move the children
	//@param: the position to be added
	public void MoveChildren( Vector3 pLocalTransform )
	{
		if( mChildren != null)
		{
			for( int i=0; i<mChildren.Count; i++)
			{
				(mChildren[i] as GUIObject).mTransform.position += pLocalTransform;
				(mChildren[i] as GUIObject).SetupRegion();
			}
		}
	}
	
	protected override void OnRender ()
	{
		mScrollPos = GUI.BeginScrollView( mRect, mScrollPos, mViewRect, mHorizontalStyle, mVerticalStyle);
		
		OnRenderScrollContent();
		base.OnRender ();
		GUI.EndScrollView();
	}
	
	//this render all contents inside the scroll bar
	protected virtual void OnRenderScrollContent()
	{}
}
