using UnityEngine;
using System.Collections;

public abstract class GUIListview : GUIButton1 {
	
	protected ArrayList mItems;								//the items for the combo box
	bool mIsVisible = true;
	GUIListItem mSelectedItem;								//the index of currently selected item
	
	public ArrayList items
	{
		get{ return mItems; }
	}
	
	//use to add an item to this object
	//@param: the object to be added
	public abstract void AddItem( Object pObject );
	
	//use to remove the item given the index
	//@param: the index of item to remove
	public void RemoveItem( int pIndex )
	{
		int length = mItems.Count;
		for( int i=0; i<length; i++)
		{
			GUIListItem item = mItems[i] as GUIListItem;
			if( item.index == pIndex )
				mItems.Remove( item );
		}
	}
	
	public GUIListItem currentSelected
	{
		get{ return mSelectedItem;}
		set
		{
			mSelectedItem = value;
		}
	}
	
	//get the item gicen the index
	//@param: the index of item to search
	//@return: return item, null if not found
	public GUIListItem GetItem( int pIndex )
	{
		int length = mItems.Count;
		for( int i=0; i<length; i++)
		{
			GUIListItem item = mItems[i] as GUIListItem;
			if( item.index == pIndex )
				return item;
		}
		
		return null;
	}
	
	//Use to toggle the visibility of all items
	//@value: true if visible while false if not
	public bool toggleVisibilityItems
	{
		get{ return mIsVisible; }
		set
		{
			if( mIsVisible != value )
			{
				mIsVisible = value;
				for( int i =0; i<mItems.Count; i++ )
				{
					(mItems[i] as GUIObject).gameObject.active = value;
				}
			}
		}
	}
	
	//callback when an item was selected
	//@param: the item that was selected
	public virtual void OnSelectItem( GUIListItem pItem )
	{
		currentSelected = pItem;
		toggleVisibilityItems = false;
	}
	
	protected override void Start ()
	{
		base.Start ();
		Component[] listItems = GetComponentsInChildren( typeof( GUIObject)) ;
		if( listItems != null )
		{
			mItems = new ArrayList();
			for( int i=0; i<listItems.Length; i++)
			{
				if( listItems[i] as GUIListItem != null)
					mItems.Add( listItems[i]);
			}
		}
	}
}
