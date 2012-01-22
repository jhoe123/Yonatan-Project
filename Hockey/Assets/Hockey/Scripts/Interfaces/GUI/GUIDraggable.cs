using UnityEngine;
using System.Collections;

/// <summary>
/// the interface for a gui draggable which drags the object on activate
/// </summary>
public class GUIDraggable : GUIObject, GUIPressable {
	
	public bool mIsDraggable = true;						//true if ok to drag while false if not
	protected bool mIsOnDrag = false;
	bool mIsPressed = false;
	
	public bool isPressed
	{
		get{ return mIsPressed;}
		set{ mIsPressed = value; }
	}
	
	public virtual void OnPush (Vector3 pMousePosition, int pTouchIndex)
	{
		if( mIsDraggable )
		{
			GameController.mMouseMoveEvent += OnDragging;
			mIsOnDrag = true;
			OnStartDragging();
		}
	}
	
	public virtual void OnRelease (Vector3 pMousePosition, int pTouchIndex)
	{
		if( mIsOnDrag )
		{
			GameController.mMouseMoveEvent -= OnDragging;
			mIsOnDrag = false;
			SetupRegion();
			OnEndDragging();
		}
	}
	
	
	//to be called when the dragging was started
	protected virtual void OnStartDragging()
	{}
	
	//to be called when draggiong was ended
	protected virtual void OnEndDragging()
	{}
	
	/// <summary>
	/// To be called when being drag
	/// </summary>
	/// @param: the current position for dragging
	protected virtual void OnDragging( Vector3 pPos)
	{
		
	}
}
