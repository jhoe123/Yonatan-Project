using UnityEngine;
using System.Collections;

/*
 * GUITextList.cs
 * Jhoemar Pagao (c) 2011
 * jhoemar.pagao@gmail.com
 * 
 * the the implementation for texlist
 * */
public class GUITextList : IGUIUnity {
	
	public delegate void OnHoverItemDelegate( int pIndex, sListItem pItem );
	
	public enum eItemState
	{
		Normal,
		Hover,
		Pressed,
	}
	
	public struct sListItem
	{
		public GUIStyle mStyle;					//the current style
		public GUIContent mContent;				//the current content
		public Rect mRect;						//the drawinf rect	
		public eItemState mState;				//the current state of item
		
		public sListItem( GUIStyle pStyle, GUIContent pContent, Rect pRect)
		{
			mStyle = new GUIStyle( pStyle);
			mContent = pContent;
			mRect = pRect;
			mState = GUITextList.eItemState.Normal;
		}
	}
	public bool mUserDefualtStyle = true;
	public float mSpacing = -7;									//the spacing each letter
	public Rect mItemRect = new Rect(0,0, 100, 40);				//the rect for items
	public Rect mViewRect = new Rect( 0, 0, 400,400);
	ArrayList mItems = new ArrayList();
	int mItemCount;
	sListItem mLastAdded;
	Vector2 mScrollPos;
	
	//events
	public OnHoverItemDelegate mOnHoverItem;					//to be called on hover item
	public OnHoverItemDelegate mOnUnhoverItem;					//to be called on unhover item
	
	public sListItem lastAdded
	{
		get{ return mLastAdded;}
	}
	
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
	
	//use to add text on the list
	//@param 1: the text to be inserted
	//@param 2: the alignment of text on the list
	//@return: return the created item from the text added
	public virtual sListItem AddText( string pText, int pIndex )
	{
		sListItem item = new sListItem(mStyle, new GUIContent(pText,null, pText), mRect);
		mItems.Insert( pIndex, item);
		mItemCount++;
		mViewRect.height = (mItemCount * mSpacing) + mItemRect.height;
		return item;
	}
	
	
	sListItem mTmpItem;
	//use to search the text
	//@param: the text to be search
	//@return: return the text that was searched
	//TODO: should return nothing
	public sListItem SearchText( string pText )
	{
		for( int i=0; i<mItemCount; i++)
		{
			mTmpItem = (sListItem)mItems[i];
			if( mTmpItem.mContent.text.Equals(pText) )
				return mTmpItem;
		}
		
		return mTmpItem;
	}

	public ArrayList items
	{
		get{ return mItems;}
	}
	
	public Vector2 scrollPosition
	{
		get{ return mScrollPos;}
		set{ mScrollPos = value;}
	}
	
	public void Reverse()
	{
		mItems.Reverse();
	}

	
	public void Clear()
	{
		mItems.Clear();
		mItemCount =0;
	}
	
	sListItem mLastItemHovered;
	bool mIsHovered = false;
	protected override void OnRender ()
	{
		if( !mUserDefualtStyle )
			mScrollPos = GUI.BeginScrollView( mRect, mScrollPos, mViewRect);
		else
			mScrollPos = GUI.BeginScrollView( mRect, mScrollPos, mViewRect, mStyle, mStyle);
		
		mIsHovered = false;
		//do unhover and hovering events
		for( int i=0; i<mItemCount; i++)
		{
			sListItem item = (sListItem)mItems[i];
			mItemRect.y = i * mSpacing;	
			GUI.Label( mItemRect, item.mContent, item.mStyle );
			if( GUI.tooltip.Equals(item.mContent.text) )
			{
				if( item.mState != GUITextList.eItemState.Hover )
				{
					if( mOnUnhoverItem != null && mLastItemHovered.mState == eItemState.Hover )
					{
						mOnUnhoverItem( 0, mLastItemHovered);
						mLastItemHovered.mState = eItemState.Normal;
					}
					
					item.mState = eItemState.Hover;
					
					mLastItemHovered = item;
					if( mOnHoverItem != null )
						mOnHoverItem( i, item);
					OnHoverItem( i,item);
					
				}
				mIsHovered = true;
			}
		}
		
		if( !mIsHovered && mOnUnhoverItem != null && mLastItemHovered.mState == eItemState.Hover)
		{
			mOnUnhoverItem( 0, mLastItemHovered);
			mLastItemHovered.mState = eItemState.Normal;
		}
		
			
		base.OnRender ();
		GUI.EndScrollView();
	}
	
	public virtual void OnHoverItem( int pIndex, sListItem pItem )
	{}
}
