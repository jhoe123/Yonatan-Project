using UnityEngine;
using System.Collections;

/// <summary>
/// every scene in unity should have this instance initialize first.
/// It will automatiocally create create managers needed if they are not exist.
/// </summary>
public class Scene : SObject {
	public static Scene mCurrent;						//the current scene
	public float mLogicUpdateRate = 0.2f;				//the logic update rate in seconds
	float mElapsedTime;
	
	//logic update
	public static UpdateDelegate mOnLogicUpdate;		//to be called on logic update
	
	protected override void Awake ()
	{
		base.Awake ();
		mCurrent = this;
		//create a controller if they are not exist
		GameController.CreateIfNotExist();
		mElapsedTime = Time.time;
	}
	
	protected virtual void OnDestroy()
	{
		mCurrent = null;
	}
	
	bool mIsPause = false;
	//pause the active scene
	public static bool pause
	{
		get{ return mCurrent.mIsPause; }
		
		set
		{	
			if( mCurrent.mIsPause != value)
			{
				mCurrent.mIsPause = value;
				if( mCurrent != null)
				{
					if( !value)
						mCurrent.OnPause( false);
					else
						mCurrent.StartCoroutine( Loop());
				}
			}
		}
	}
	
	static IEnumerator Loop()
	{
		yield return new WaitForSeconds(0.09f);
		mCurrent.OnPause( true);
	}
	
	protected virtual void OnPause( bool pWillPause)
	{	
		Time.timeScale = ( pWillPause )? 0: 1;
	}
	
	public virtual void OnDisable()
	{
		mOnLogicUpdate = null;
	}
	
	public override void OnUpdate (float mCurrentTime)
	{
		base.OnUpdate (mCurrentTime);
		if( mOnLogicUpdate != null &&  mCurrentTime - mElapsedTime > mLogicUpdateRate )
		{
			mOnLogicUpdate( mCurrentTime);
			mElapsedTime = mCurrentTime;
		}
	}
}
