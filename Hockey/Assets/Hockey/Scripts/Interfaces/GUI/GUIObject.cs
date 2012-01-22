using UnityEngine;
using System.Collections;

/*
 * GUIObject.cs
 * Jhoemar Pagao (c) 2011
 * jhoemar.pagao@gmail.com
 * 
 * The main interface for all guiobjects
 * The gui object can be pressable or focusable.
 * */
public class GUIObject : SObject {

	public int mLayer = 0;					//the current layer of the guiobject
	protected bool mEnabled = true;			//true if this gui is enabled while false if not
	private GUIObject mParent;				//the parent guiObject
	protected ArrayList mChildren;			//the children
	
	//the 2d screen coordinates
	protected float mMinX;								
	protected float mMinY;
	protected float mMaxX;
	protected float mMaxY;
	
	//Pressable variables
	GUIPressable mThisPressable;			//the casted pressable of this type. null if this is not a pressable
	static ArrayList mChildrenPressable;	//all guipressables
	static ArrayList mChildrenFocusable;	//all focusable
	
	protected Camera mCam;					//the personal camera for setting up gui region
	int mRenderBatch;
	public Transform mTransform;			//the transform
	public Renderer mRenderer;				//the renderer to be used in grtting the bounds
	public Bounds meshBound;
	
	protected virtual void OnEnable()
	{
		mEnabled = true;	
	}
	
	protected virtual void OnDisable()
	{
		mEnabled =false;
		if( mIsFocus )
		{
			mIsFocus = false;
			(this as GUIFocusable).OnUnfocus( Vector3.one);	
		}
		
		//reset if this for GUIPressables
		if( mThisPressable != null && mThisPressable.isPressed )
			mThisPressable.OnRelease( GameController.mCursorScreentPoint, 0);
	}
	
	//property for the parent of guiobject
	public GUIObject parent
	{
		get{ return mParent; }
		set{
			
			//if( mParent != null /*&& mParent != value*/ )
			//	mParent.RemoveChild( this);
			
			mParent = value;
		}
	}
	
	public Vector2 min
	{
		get{ return new Vector2( mMinX, mMinY); }
	}
	
	public Vector2 max
	{
		get{ return new Vector2( mMaxX, mMaxY); }
	}
	
	protected override void Awake ()
	{
		base.Awake ();
		mTransform = transform;
		if( mRenderer == null )
			mRenderer = renderer;
	
		if( mParent == null )
		{
			//add itself to the parent guiobject, if no parent add itself to the root guiobject
			Transform parent = transform.parent;
			if( parent != null)
				mParent = parent.GetComponent( typeof(GUIObject) ) as GUIObject;
			
			if( mParent == null )
				StartCoroutine( SetDefaultParent() );
			else
				mParent.AddChild( this);
		}	
		mThisPressable = this as GUIPressable;
	}
	
	protected override void Start ()
	{
		base.Start ();
		if( gameObject.layer != LayerMask.NameToLayer("GUIObject") )
			mCam = GUIController.mSceneCamera;
		else
			mCam = GUIController.mGUICamera;
		
		SetChildrenState( eStateList.Initialize, null);
		SetupRegion(); 
	}
	
	protected override void OnUnInit ()
	{
		base.OnUnInit ();
		if( mParent != null )
		{
			mParent.mChildren.Remove( this);
			SetChildrenState( eStateList.UnInitialize, null);
		}
	}
	
	IEnumerator SetDefaultParent()
	{
		if( Time.timeSinceLevelLoad < 1)
			yield return new WaitForFixedUpdate();
		else
			yield return null;
		
		if( mLayer > -1 )
		{
			GUIController.AssignLayer( this);	
		}
	}
	
	public void SetChildrenState( eStateList pState, object[] pParams )
	{
	}
	
	//call this to add a child on a guiobject
	//@param: the child to be add
	public virtual void AddChild( GUIObject pObject )
	{
		if( mChildren == null )
			mChildren = new ArrayList();
		
		//pObject.mLayer = mLayer;
		mChildren.Add( pObject);
		pObject.parent = this;
		
		if( pObject as GUIPressable != null )
		{
			if( mChildrenPressable == null )
			{
				mChildrenPressable = new ArrayList();
				GameController.mMouseEvent += OnPressableEvent;
			}
			
			mChildrenPressable.Add( pObject );
		}
		
#if !UNITY_TOUCH || UNITY_EDITOR
		GUIFocusable focusable = pObject as GUIFocusable ;
		if( focusable != null && focusable as IGUIUnity == null )
		{
			if( mChildrenFocusable == null)
			{
				mChildrenFocusable = new ArrayList();
				GameController.mMouseMoveEvent += OnCursorMove;
			}
			mChildrenFocusable.Add( focusable);
		}
#endif
		//Debug.Log( "added children " + pObject + " to " + name );
	}
	
	//use to remove all child
	public virtual void RemoveChild( GUIObject pObject)
	{
		if( pObject.mParent == this )
		{
			SObject.DeleteObject( pObject);
			mChildren.Remove( pObject );
		}
		
		if( pObject as GUIPressable != null )
			mChildrenPressable.Remove( pObject);
		
#if !UNITY_TOUCH || UNITY_EDITOR
		if( pObject as GUIFocusable != null )
			mChildrenFocusable.Remove( pObject);
#endif
	}
	
	//this will remove all child og the gui
	public virtual void RemoveAllChild()
	{
		for( int i=0; i<mChildren.Count; i++)
			SObject.DeleteObject( mChildren[i] as SObject);
		
		mChildren.Clear();
		
		if( mChildrenPressable != null )
			mChildrenPressable.Clear();
		
#if !UNITY_TOUCH || UNITY_EDITOR
		if( mChildrenFocusable != null )
			mChildrenFocusable.Clear();
#endif
	}
	
	//use to return all children attacjed to this
	public ArrayList GetChildren()
	{
		return mChildren;
	}
	
	//use to setup the min and max of the 2d region
	public virtual void SetupRegion()
	{
		if(mRenderer != null && mCam != null)
		{
			meshBound = mRenderer.bounds;
			Vector3 tmpMin = mCam.WorldToScreenPoint( meshBound.min );
			Vector3 tmpMax = mCam.WorldToScreenPoint( meshBound.max );
			mMinX = tmpMin.x;
			mMinY = tmpMin.y;
			mMaxX = tmpMax.x;
			mMaxY = tmpMax.y ;
		}
		
		if( mChildren  == null )
			return;
	
		int count = mChildren.Count;
		for( int i=0; i < count; i+=10)
		{
			(mChildren[i] as GUIObject).SetupRegion( );
				
			if( i+1 <count )
				(mChildren[i+1] as GUIObject).SetupRegion( );
			else 
				return;
							
			if( i+2 < count )
				(mChildren[i+2] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+3 < count )
				(mChildren[i+3] as GUIObject).SetupRegion( );
			else 
				return;
		
			if( i+4 < count )
				(mChildren[i+4] as GUIObject).SetupRegion( );
			else 
				return;
			
			if( i+5 < count )
				(mChildren[i+5] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+6 < count )
				(mChildren[i+6] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+7 < count )
				(mChildren[i+7] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+8 < count )
				(mChildren[i+8] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+9 < count)
				(mChildren[i+9] as GUIObject).SetupRegion( );
			else 
				return;
				
			if( i+10 < count)
				(mChildren[i+10] as GUIObject).SetupRegion( );
			else 
				return;
		}
	}
	
	//use to test iether the point is insode in this object AABB box
	//@return: true if the point is inside while false if not
	public bool IsPointInside( float pX, float pY )
	{
		if( !(( (pX > mMinX) && (pX < mMaxX ) ) &&
			( (pY > mMinY) && (pY < mMaxY) )) 
		)
			return false;
		else
			return true;		
	}
	
#if !UNITY_TOUCH || UNITY_EDITOR
	bool mIsFocus = false;
	static GUIFocusable mLastFocusable;
	static float mElapsedTime;
	static void OnCursorMove( Vector3 pPos )
	{
		if( Time.time - mElapsedTime < 0.1f )
			return;
		
		mElapsedTime = Time.time;
		mTmpObject = DepthTest( mChildrenFocusable.ToArray( typeof(GUIObject)) as GUIObject[], pPos);
		if( mTmpObject != mLastFocusable )
		{
			if( mLastFocusable != null )
				mLastFocusable.OnUnfocus( pPos);
				
			mLastFocusable = mTmpObject as GUIFocusable;
			if( mLastFocusable != null )
				mLastFocusable.OnFocus( pPos);
		}
	}
#endif
	
	//this toggle the enable for children
	//@param 1: true if this will be enable while false if not
	public void SetEnableRecursively( bool pIsEnable )
	{
		enabled = pIsEnable;
		if( mChildren != null )
		{
			int count = mChildren.Count;
			for( int i=0; i< count; i++)
			{
				if( mChildren[i] != null)	
					(mChildren[i] as GUIObject).SetEnableRecursively( pIsEnable);
			}
		}
	}
	
	#region PRESSABLE FUNCTIONALITIES
	
	static GUIPressable mLastPressed; 
	static GUIObject mTmpObject;
	//static int mTmpPressableCount;
	
	//callback events for the gui pressables
	static void OnPressableEvent( InputType pType, int pMouseIndex, bool pIsPressed, Vector3 pMousePosition )
	{
		if( pIsPressed )
		{
			mTmpObject = DepthTest( mChildrenPressable.ToArray( typeof(GUIObject)) as GUIObject[], pMousePosition );
			if( mTmpObject != null)
			{
				mLastPressed = mTmpObject as GUIPressable;
				mLastPressed.isPressed = true;
				mLastPressed.OnPush( pMousePosition, pMouseIndex );
			}
		
		}else
		{
			if( mLastPressed != null )
			{
				mLastPressed.isPressed = false;
				mLastPressed.OnRelease( pMousePosition, pMouseIndex);		
				mLastPressed = null;
			}
		}
	}
	
	#endregion
	
	#region DEPTH TESTING
	
	//use to set the max depth test
	public static int _DepthTestCount
	{
		get{ return mMaxDeptTest; }
		set{
			mMaxDeptTest = value;
			mDepthTestObjects = new GUIObject[value];
		}
	}
	
	static int mMaxDeptTest;
	static int mPriorityI = 0;
	static float mTmpX;
	static float mTmpY;
	static int mDepthTC = 0;
	static GUIObject[] mDepthTestObjects;
	static GUIObject mLastPriority;
	static GUIObject mFocus;
	static GUIObject DepthTest( GUIObject[] pObjects, Vector3 pScreenPosition )
	{
		mPriorityI = 0;
		mDepthTC = pObjects.Length;
		mTmpX = pScreenPosition.x;
		mTmpY = pScreenPosition.y;
			for( int i=0; i<mDepthTC; i+=6)
			{
				if( mPriorityI < mMaxDeptTest )
				{
					if(  pObjects[i].mEnabled && pObjects[i].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i];
						mPriorityI++;
					}
				}else
					break;
				
				if( mPriorityI < mMaxDeptTest && i+1<mDepthTC )
				{
					if(  pObjects[i+1].mEnabled && pObjects[i+1].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i+1];
						mPriorityI++;
					}
					
				}else
					break;
				
				if( mPriorityI < mMaxDeptTest && i+2<mDepthTC )
				{
					if(  pObjects[i+2].mEnabled && pObjects[i+2].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i+2];
						mPriorityI++;
					}
					
				}else
					break;
					
				if( mPriorityI < mMaxDeptTest && i+3<mDepthTC )
				{
					if(  pObjects[i+3].mEnabled && pObjects[i+3].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i+3];
						mPriorityI++;
					}
					
				}else
					break;
				
				if( mPriorityI < mMaxDeptTest && i+4<mDepthTC )
				{
					if(  pObjects[i+4].mEnabled && pObjects[i+4].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i+4];
						mPriorityI++;
					}
					
				}else
					break;
				
				if( mPriorityI < 3 && i+5<mDepthTC )
				{
					if(  pObjects[i+5].mEnabled && pObjects[i+5].IsPointInside(mTmpX, mTmpY) )
					{
						mDepthTestObjects[mPriorityI] = pObjects[i+5];
						mPriorityI++;
					}
					
				}else
					break;
			}
			//Debug.Log( mPriorityI);
			if( mPriorityI > 0 )
			{
				
				mLastPriority = mDepthTestObjects[0];
				
				if( mPriorityI > 1)
				{
					//depth testing
					switch( GUIController.mDepthTestType )
					{
					case GUIController.eGUIDepthTest.Layer:
						if( !GUIController.mDepthTestNegate)
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mLayer > mLastPriority.mLayer )
									mLastPriority = mFocus;
							}
						}else
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mLayer < mLastPriority.mLayer )
									mLastPriority = mFocus;
							}
						}
						break;
					case GUIController.eGUIDepthTest.ZTest:
						if( !GUIController.mDepthTestNegate )
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.z > mLastPriority .mTransform.position.z )
									mLastPriority = mFocus;
							}
						}else
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.z < mLastPriority .mTransform.position.z )
									mLastPriority = mFocus;
							}
						}
						break;
						
					case GUIController.eGUIDepthTest.YTest:
						if( !GUIController.mDepthTestNegate )
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.y > mLastPriority .mTransform.position.y )
									mLastPriority = mFocus;
							}
						}else
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.y < mLastPriority .mTransform.position.y )
									mLastPriority = mFocus;
							}
						}
						break;
						
					case GUIController.eGUIDepthTest.XTest:
						if( !GUIController.mDepthTestNegate )
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.x > mLastPriority .mTransform.position.x )
									mLastPriority = mFocus;
							}
						}else
						{
							for( int i=1; i<mPriorityI; i++)
							{
								mFocus =  mDepthTestObjects[i];
								if( mFocus.mTransform.position.x < mLastPriority .mTransform.position.x )
									mLastPriority = mFocus;
							}
						}
						break;
					}
				}
				
				return mLastPriority;
			}
		return null;
	}
	#endregion
}
