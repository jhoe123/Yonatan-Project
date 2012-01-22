using UnityEngine;
using System.Collections;
using System;

/*
 * Implementation for list of simple buttons
 * 
 * */
public class GUISelectionGrid : IGUIUnity {
	
	public delegate void OnItemSelected( int pItemIndex);
	
	public bool mWillScroll = false;
	public bool mShowHorizontal = true;
	public bool mShowVertical = true;												
	public GUIContent[] mContents = new GUIContent[]{ new GUIContent() };				//the items on the grid list
	public int mInitialSelection = 0;
	public int mInitialColumns = 3;
	int mColumns = 0;
	public int mSelected = 0;															//the current selected item
	Rect mLocalRect = new Rect(0,0, 100, 100);
	GUIStyle mStyle;
	
	public override GUISkin guiSkin {
		get {
			return base.guiSkin;
		}
		set {
			base.guiSkin = value;
			mStyle = value.scrollView;
		}
	}
	
	//events
	public OnItemSelected mOnItemSelected;
	
	protected override void Start ()
	{
		base.Start ();
		mSelected = mInitialSelection;
		SetupRegion();
	}
	
	public void AddText( string pText )
	{
		int l = mContents.Length;
		Array.Resize<GUIContent>( ref mContents, l +1);
		mContents[l] = new GUIContent(pText);
	}
	
	public void AddText( string pText, int pIndex )
	{
		ArrayList list = new ArrayList();
		list.AddRange( mContents);
		list.Insert( pIndex, new GUIContent( pText));
		mContents = list.ToArray( typeof(GUIContent)) as GUIContent[];
	}
	
	public void Clear()
	{
		mContents = new GUIContent[]{ new GUIContent() };
	}
	
	public void Sort()
	{
		Array.Sort( mContents);	
	}
	
	public override void SetupRegion ()
	{
		base.SetupRegion ();
		mColumns = mInitialColumns;
		mLocalRect.width = mRect.xMax - mRect.xMin;
		mLocalRect.height = mRect.yMax - mRect.yMin;
	}
	 
	public Vector2 scrollPosition
	{
		get{ return mScrollView;}
		set{ mScrollView = value;}
	}
	
	Vector2 mScrollView;
	protected override void OnRender()
	{
		if( mEnabled )
		{
			GUI.depth = mRenderPriority;
			if( !mWillScroll )
			{	
				int s = GUI.SelectionGrid( mRect, mSelected, mContents, mInitialColumns, mStyle);
				if( s != mSelected )
				{
					mSelected = s;
					OnCellSelected( s);
					if( mOnItemSelected != null )
						mOnItemSelected( s);
				}
			}else
			{
				mScrollView = GUI.BeginScrollView( mRect , mScrollView, mLocalRect, mShowHorizontal, mShowVertical);
				int s = GUI.SelectionGrid( mLocalRect, mSelected, mContents, mInitialColumns, mStyle);
				if( s != mSelected )
				{
					mSelected = s;
					OnCellSelected( s);
					if( mOnItemSelected != null )
						mOnItemSelected( s);
				}
				
				GUI.EndScrollView();
			}
		}
		//Debug.Log( GUIUtility.GetStateObject( typeof(GUIObject), 2 ));
		base.OnRender();
	}
	
	//callback when the cell was selected
	//@param: the index from the list
	public virtual void OnCellSelected( int pIndex)
	{}
}
