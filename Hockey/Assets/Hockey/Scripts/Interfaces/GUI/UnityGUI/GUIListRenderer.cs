using UnityEngine;
using System.Collections;

/*
 * Class that implements the rendering of control list in a fast way.
 * 
 * */
public class GUIListRenderer : GUIScrollView {
	
	#region TYPEDEF
	public delegate void OnItemRender( sListItem pItem);
	
	public struct sListItem
	{
		public int mID;
		public GUIContent mContent;
		public Rect mRect;
		
		public sListItem( int pID, GUIContent pContent, Rect pRect)
		{
			mID = pID;
			mContent = pContent;
			mRect = pRect;
		}
	}
	#endregion
	
	ArrayList mItems = new ArrayList();		//the item on the renderer list
	int mItemCount;
	
	public OnItemRender mOnRender;			//callback when this will be rendered
	float mMaxWidth;
	float mMaxHeight;
	
	//use to add item
	public void AddItem( int pID, GUIContent pContent, Rect pRect)
	{
		mItems.Add( new sListItem( pID, pContent, pRect));
		mItemCount++;
		if( pRect.yMax > mMaxHeight)
			mMaxHeight = pRect.yMax;
		
		if( pRect.xMax > mMaxWidth)
			mMaxWidth = pRect.xMax;
		
		mViewRect.width = mMaxWidth-20;
		mViewRect.height = mMaxHeight-20;
	}
	
	public void ClearItems()
	{
		mItems.Clear();
		mItemCount = 0;
	}
	
	protected override void OnRenderScrollContent ()
	{
		for(int i=0; i<mItemCount; i++)
			mOnRender( (sListItem)mItems[i]);
	}
}
