using UnityEngine;
using System.Collections;
using System;

public class GUIController : MonoBehaviour {
	
	public enum eGUIDepthTest
	{
		Layer,						//test based on the layers
		ZTest,						//test positive z
		YTest,						//test positive y
		XTest,						//test positive x
	}
	
	//params
	public static GUIController mCurrent;
	public static Camera mGUICamera;				//the current gui cam
	public static Camera mSceneCamera;				//the scene camera
	public static AudioSource mAudioSource;
	
	//gui depth testing
	public static eGUIDepthTest mDepthTestType = eGUIDepthTest.Layer;//gui depth testing. the gui priority based on the test type
	public static bool mDepthTestNegate = false;	//if true, this negate the values of the depth testing while false if not
	public static int mDepthTestCount
	{
		get{ return GUIObject._DepthTestCount; }
		set{ GUIObject._DepthTestCount = value; }
	}
	
	//gui designs
	public static GUISkin mDefaultSkin;				//the default skin
	
	//layers
	public static GUIObject mCurrentGUILayer;
	public static GUIObject mRootGUIObject;			//the root guiobject which is in the layer 0;
	public static ArrayList mLayers;				//the guilayers
	static int mCurrentLayer = 0;
	static int mLayerCount = 0;
	
	public static eStateList mState;				//current state of the GUI controller
	
	//gui callbacks
	public static GUIDelegate mOnGUIPush;			//to be called when a gui was push
	public static UpdateDelegate mUpdate;			//to be called on guiupdate
	
	//gui datas
	public static GUIPressable mLastGUIPush;		//the last that was pressed
	
	void Awake()
	{
		if( mDefaultSkin == null )
			mDefaultSkin = new GUISkin();
		mDepthTestCount = 3;
		mCurrent = this;	
		GameObject obj2 = new GameObject("Layer0");
		obj2.transform.parent = transform;
		mRootGUIObject = obj2.AddComponent( typeof(GUIObject) ) as GUIObject;
		mRootGUIObject.mLayer = -1;
		mLayers = new ArrayList();
		mLayers.Add( mRootGUIObject);
		mCurrentGUILayer = mRootGUIObject;
		OnLevelWasLoaded( 0);
		
		mAudioSource = gameObject.AddComponent<AudioSource>();
		
		Debug.Log("init controller");
	}
	
	void OnLevelWasLoaded( int pLevel )
	{
		GameObject obj = GameObject.FindWithTag("GUICamera");
		if( obj != null )
			mGUICamera = obj.camera; 
		else
			mGUICamera = Camera.main;
		
		mSceneCamera = Camera.main;
	}
	
	//assign the a layer to guiobject
	//@param: the guiobject to be assigned
	public static void AssignLayer( GUIObject pObject )
	{
		int layer = pObject.mLayer;
		if( layer >= mLayers.Count )
		{
			for( int i = mLayers.Count; i <= layer; i++)
			{
				GameObject obj3 = new GameObject("Layer"+i);
				obj3.transform.parent = mCurrent.transform;
				GUIObject obj = obj3.AddComponent(typeof(GUIObject)) as GUIObject; 
				//obj.mLayer = i;
				mLayers.Add( obj);
			}
		}
		
		(mLayers[ layer ] as GUIObject).AddChild( pObject);
		mLayerCount = mLayers.Count;
	}
	
	static int mPreviousLayer = 0;
	//use to revert from the previous layer
	public static void UndoLayer()
	{
		SetLayer( mPreviousLayer );
	}
	
	//use to set the current layer 
	public static void SetLayer( int pLayer )
	{
		mCurrentGUILayer = mLayers[ pLayer] as GUIObject;
		mCurrentGUILayer.Invoke("SetupRegion", 0.2f);
		mPreviousLayer = mCurrentLayer;
		mCurrentLayer = pLayer;
	}
	
	#region SOUNDS AND EFFECTS
	public static void PlayGUIAudio( AudioClip pClip, float pVolume )
	{
		mAudioSource.PlayOneShot( pClip, pVolume);
	}
	#endregion
	
	float mTmpTime;
	void FixedUpdate()
	{
		mTmpTime = Time.time;
		if( mUpdate != null )
			mUpdate(  mTmpTime);
	}
}
