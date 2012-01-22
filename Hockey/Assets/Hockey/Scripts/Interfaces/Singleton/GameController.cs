#if UNITY_ANDROID || UNITY_IPHONE
	#define UNITY_TOUCH 
#endif 

using UnityEngine;
using System.Collections;

/// <summary>
/// Loads/unload the scene.
/// Handles input events for all platforms
/// 
/// Usage:
/// 	Scene Loading - To be able to load a scene type(unity scene) it should be first initialize on the scene variables
/// 
/// TODO: currently it is only able to load real types of unity scene.
/// </summary>
public class GameController : MonoBehaviour {
	
#if !UNITY_TOUCH || UNITY_EDITOR
	public delegate void OnResolutionChangedDelegate( Vector2 pLastResolution, Vector2 pCurrentResolution );
#endif
	
	public static GameController mCurrent;
	public static Camera mMainCamera;									//the main camera
	static Scene mCurrentScene;											//The current scene
	eStateList mState;													//the current state
	
	public static Vector3 mCursorScreentPoint;							//the current cursor pointer				
	static bool[] mIsEvent = new bool[]{ false, false, false, false};	//true if the state was triggered while false if not
	
	public static Vector3 mLastMovePos = Vector3.zero;
	public static string mLastKeyboardInput = "";
	public static bool[] mMouseIndexPressed = {false, false, false};	//true if the current mouse id pressed		
	public static float mLastScrollSpeed = 0;							
	
	//input handling
	public static MouseMoveDelegate mMouseMoveEvent;					//to be called when the mouse is moving
	public static MouseCallbackDelegate mMouseEvent;					//to be called when triggered mouse push states events
	public static MouseCallbackDelegate mLateMouseEvent;				//to be called after the mouse was triggered
	public static KeyboardCallbackDelegate mKeyboardEvent;				//to be called when triggered keyboard events
	public static MouseScrollDelegate mMouseScrollEvent;				//to be called when scrolling is started
	public static AcceleratorDelegate mOnAccelerate;					//to be called when the accelelometer changed
	
	//scenes
	public string mIntroScene = "Intro";
	public string mMenuScene ="Menu";									//the menu scene
	public string[] mLevelScene = new string[]{"Level"};				//the list of level in sequence, 
																		//to load the level increment the current level index
	
	//scene events
	public static SceneLoadDelegate mBeforeLoad;						//to be called before the loading
	public static SceneLoadDelegate mAfterLoad;							//to be called after loading
	
	public static int mCurrentLevelIndex;								//the current level loaded
	
	//delegates
	public static UpdateDelegate mUpdateDelegate;						//to be called when updating the scene
	public static UpdateDelegate mUpdateRealTimeDelegate;				//to be called when updating realtime
	public static SceneLoadDelegate mOnLoadBefore;						//to be called before loading the scene
	public static SceneLoadDelegate mOnLoadAfter;						//to be called after loading the scene
#if !UNITY_TOUCH || UNITY_EDITOR
	public static OnResolutionChangedDelegate mOnResolutionChanged;		//to be called when resolution was change
	
	//resolution vars
	public static Vector2 mLastScreenRect;								//the last resolution
	public static Vector2 mCurrentScreenRect;							//the current resolution
	Event mGUIEvent;													//the GUI event								
	float mScreenWidth;
	float mScreenHeight;
#endif
	
	void Awake()
	{
		mCurrent = this;
		DontDestroyOnLoad( gameObject);
		mMainCamera = Camera.mainCamera;
		
#if !UNITY_TOUCH || UNITY_EDITOR
		//resolution
		mCurrentScreenRect = new Vector2( Screen.width, Screen.height);
		mLastScreenRect = mCurrentScreenRect;
		mScreenWidth = Screen.width;
		mScreenHeight = Screen.height;
		mGUIEvent = Event.current;
#endif
	}
	
	void Start()
	{
		mCurrentScene = FindObjectOfType( typeof(Scene) ) as Scene;
		if( mAfterLoad != null )
			mAfterLoad( mCurrentScene, 0);
	}
	
	//this create a game controller if there is no existing one
	//@return: true if created a gamecontroller while false if not
	public static bool CreateIfNotExist()
	{
		if( mCurrent != null )
			return false;
			
		GameObject controller = new GameObject("_Controllers" );
		controller.AddComponent( typeof(GameController) );
		controller.AddComponent( typeof(GUIController));
		controller.AddComponent( typeof(EffectController));
		return true;
	}

	public static void SetState( eStateList pState)
	{
		
	}
	
	//this loads the specified scene type
	public static void LoadScene( eSceneType pType )
	{
		if( mBeforeLoad != null )
			mBeforeLoad( mCurrentScene, 0);
		
		string level2Load = "";
		switch( pType )
		{
		case eSceneType.Level:
			if( mCurrentLevelIndex < mCurrent.mLevelScene.Length )
			{
				level2Load =  mCurrent.mLevelScene[ mCurrentLevelIndex];
				
			}
#if UNITY_EDITOR
			else{
				Debug.LogError("GAMECONTROLLER: unable to load the level with index of " + mCurrentLevelIndex);
			}
#endif
			break;
			
		case eSceneType.Menu:
			level2Load = mCurrent.mMenuScene;
			break;
		}
		
		Application.LoadLevel( level2Load );
	}
	
	public Scene currentScene
	{
		get{ return mCurrentScene; }
	}
	
	void OnLevelWasLoaded( int pLevel )
	{
		mMainCamera = Camera.mainCamera;
		mCurrentScene = FindObjectOfType( typeof(Scene) ) as Scene;
	}
	
	public static bool GetInputState( InputType pType )
	{
		return mIsEvent[ (int)pType];
	}
	
#if !UNITY_TOUCH || UNITY_EDITOR
	void OnGUI()
	{
		mGUIEvent = Event.current;
		mCursorScreentPoint = Input.mousePosition;

		if( mKeyboardEvent != null && mGUIEvent.isKey )		
		{
			mKeyboardEvent( mGUIEvent.keyCode );
		}
	}
#endif
	
	float mLastDistance;
	float mElapsedTime;
	float mTmpTime;
	void Update()
	{
		mTmpTime = Time.realtimeSinceStartup;
		
#if !UNITY_TOUCH || UNITY_EDITOR
		if( mMouseEvent != null)
		{
			bool isPressed = Input.GetMouseButton( 0);
			if( mMouseIndexPressed[0] != isPressed )
			{
				mMouseEvent( InputType.Mouse, 0, isPressed, mCursorScreentPoint );
				mMouseIndexPressed[0] = isPressed;
			
				if( mLateMouseEvent != null )
					mLateMouseEvent( InputType.Mouse, 0, isPressed, mCursorScreentPoint );
			}
				
			isPressed = Input.GetMouseButton( 1);
			if( mMouseIndexPressed[1] != isPressed )
			{
				mMouseEvent( InputType.Mouse, 1, isPressed, mCursorScreentPoint );
				mMouseIndexPressed[1] = isPressed;
			
				if( mLateMouseEvent != null )
					mLateMouseEvent( InputType.Mouse, 1, isPressed, mCursorScreentPoint );
			}
				
			isPressed = Input.GetMouseButton( 2);
			if( mMouseIndexPressed[2] != isPressed )
			{
				mMouseEvent( InputType.Mouse, 2, isPressed, mCursorScreentPoint );
				mMouseIndexPressed[2] = isPressed;
				
				if( mLateMouseEvent != null )
					mLateMouseEvent( InputType.Mouse, 2, isPressed, mCursorScreentPoint );
			}
		}
#endif
		
		if( mTmpTime - mElapsedTime > 0.02f )
		{
			mElapsedTime = mTmpTime;
			
			if( mUpdateRealTimeDelegate != null)
				mUpdateRealTimeDelegate( mTmpTime);
				
#if !UNITY_TOUCH || UNITY_EDITOR  
			if( mMouseScrollEvent != null )
			{
				float val = Input.GetAxis("Mouse ScrollWheel");
				if( 0 != val)
				{
					mLastScrollSpeed = val;
					mMouseScrollEvent( val);
				}
			}
		
			if( mMouseMoveEvent != null)
			{
				if( mLastMovePos != mCursorScreentPoint)
				{
					mLastMovePos = mCursorScreentPoint;
					mMouseMoveEvent( mCursorScreentPoint);
				}
			}
#endif
		}
		
#if UNITY_TOUCH && !UNITY_EDITOR
		switch( Input.touchCount )
		{
		case 1:
			
			Touch touch1 = Input.touches[0];
			mCursorScreentPoint = touch1.position;
			TouchPhase phase = touch1.phase;
			
			switch( phase )
			{ 
				case TouchPhase.Ended:
					if( mMouseEvent != null )
					{
						mMouseEvent( InputType.Touch, 0, false, mCursorScreentPoint);
						mMouseIndexPressed[ 0] = false;
						if( mLateMouseEvent != null )
							mLateMouseEvent( InputType.Mouse, 0, false, mCursorScreentPoint );
					}
					break;
					
				case TouchPhase.Began:
					if( mMouseEvent != null )
					{
						mMouseEvent( InputType.Touch, 0, true, mCursorScreentPoint);
						mMouseIndexPressed[ 0] = true;
						if( mLateMouseEvent != null )
							mLateMouseEvent( InputType.Mouse, 0, true, mCursorScreentPoint );
					}
					break;
					
				case TouchPhase.Moved:
					if( mMouseMoveEvent != null )
					{
						mLastMovePos = mCursorScreentPoint;
						mMouseMoveEvent( mCursorScreentPoint);
					}
					break;
			}
			break;
			
		case 2:
			touch1 = Input.touches[0];
			mCursorScreentPoint = touch1.position;
			Touch touch2 = Input.touches[1];
			if( mMouseScrollEvent != null && ( touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved ) )
			{
				float distance = Vector2.Distance( touch1.position, touch2.position );
				float val = 0;
				if( mLastDistance > distance )
					val = -0.1f;
				else
					val = 0.1f;
				mMouseScrollEvent( val);
				mLastDistance = distance;
			}
			break;
		}
		
		if( mOnAccelerate != null )
		{
			Vector3 mTmpAcc = Input.acceleration;
			mOnAccelerate( mTmpAcc);
		}
#endif
/*
	}
	
	float mLastDistance;
	void FixedUpdate()
	{
*/
#if !UNITY_TOUCH
		//process screen resolution changes
		if( mOnResolutionChanged != null )
		{
			if( Screen.width != mScreenWidth || Screen.height != mScreenHeight )
			{
				mScreenWidth = Screen.width;
				mScreenHeight = Screen.height;
				mLastScreenRect = mCurrentScreenRect;
				mLastScreenRect = new Vector2( mScreenWidth, mScreenHeight);
				
				mOnResolutionChanged( mLastScreenRect, mCurrentScreenRect);
			}
		}
#endif
		
		mTmpTime = Time.time;
		if( mUpdateDelegate != null )
			mUpdateDelegate(  mTmpTime);	
			
		mCurrentScene.OnUpdate( mTmpTime);
	}
}
