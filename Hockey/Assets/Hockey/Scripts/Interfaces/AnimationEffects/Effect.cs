using UnityEngine;
using System.Collections;
using System;

/*
 * A complete implementation of effect.
 * Able to:
 *	- play animation
 *	- play audio clip
 *	- instantiate object
 *	- timed play
 *	- timed stop
 *	- timed Destroy
 *	- can be repeated
 *
 * How to use:
 * 		To use features, just change the declared variables to desired values
 * 		to play the effect call Play()
 * 		to pause the effect call Pause()
 * 		to stop the effect call Stop()
 * 		to destroy this effect just call Destroy()
 *		 
 * */
public class Effect : MonoBehaviour {
	
	//simultaneous effects
	public Effect mEffectOnPlay;					//the effect to be played on play
	public Effect mEffectOnStop;					//the effect to be played on stop
	public Effect mEffectOnDestroy;					//the effect to be played on destroy
	
	public Animation mAnimation;					//animation to play
	public AudioClip mClipOnPlay;					//clip to play
	public AudioClip mClipOnEnd;					//clip on end
	public float mStopTime = 3;						//will stop after the given time. if time < 0 then it will not stop
	public float mStartTime = -1;					//will autoplay on the given time. if time < 0 then it will not autoplay;
													//time == 0 will play on wake
	public float mDestroyTime = -1;					//if time >= 0 then it will count to destroy after the stop() was called, while
													//time < 0 then manually destroy
	public float mEnableTime = -1;					//will play after this effect enabled
	public float mDisableTime = -1;					//will play an exit effect. this will postpome the deactivation
	public int mRepeatCount = 0;					//if -1 then repeat forever, 0 if will not repeat, val > 0 repeat with the given val
	public bool mUpdateOnPause = false;				//true if this will be updated on pause while false if not		
	protected int mCurrentRepeatC = 0;
	
	//to be called when this effect was initialized
	protected virtual void Start()
	{
		InvokeInternal( mUpdateOnPause, "Play", mStartTime);
	}
	
	//to be called when this effect was repeated.  this will replay the effect
	//@param: the number of repeats
	protected virtual void OnRepeat( int pRepeatCount )
	{}
	
	protected bool mPlayed = false;
	//play the effect
	//@return: true if this effect was played while false if not
	public virtual bool Play()
	{
		if( !mPlayed )
		{	
			if( mAnimation != null )
				mAnimation.Play();
			
			if( mClipOnPlay != null )
				AudioSource.PlayClipAtPoint( mClipOnPlay, transform.position );
			
			if( mStopTime > -1)
				InvokeInternal( mUpdateOnPause, "Stop", mStopTime);
			
			mPlayed = true;
			
			if( mEffectOnPlay != null )
				mEffectOnPlay.Play();
			return true;
			
		}else
			return false;
	}
	
	
	//stop the effect
	public virtual void Stop()
	{	
		if( mPlayed )
		{
			mPlayed = false;
			
			if( mClipOnEnd != null )
				AudioSource.PlayClipAtPoint( mClipOnEnd, transform.position );
			
			if( mAnimation != null )
				mAnimation.Stop();
			
			//test for repeats
			if( mRepeatCount == 0 )
			{	
				if( mDestroyTime > -1)
					InvokeInternal( mUpdateOnPause, "Destroy", mDestroyTime );
				
				if( mDisableTime > -1)
					gameObject.SetActiveRecursively( false);
			}else
			{
				if( mRepeatCount > mCurrentRepeatC || mRepeatCount < 0)
				{
					mCurrentRepeatC++;
					OnRepeat( mCurrentRepeatC );
					Play();
				}
				else
				{
					mCurrentRepeatC = 0;
					if( mDestroyTime > -1)
						Invoke( "Destroy", mDestroyTime );
				}
			}
			
			if( mEffectOnStop != null )
				mEffectOnStop.Play();
		}
	}
	
	float mElapsedEffect;
	float mTargetTime;
	string mMethodName;
	//use to invoke the method
	//@param 1: true if this will invoke on pause while false if not
	//@param 2: the method to be invoke
	//@param 3: the to invoke
	protected void InvokeInternal( bool pPlayonPause, string pMethodName, float pTime)
	{
		if( pTime >= 0)
		{
			if( pPlayonPause == false)
			
				Invoke( pMethodName, pTime);
			else
			{
				mMethodName = pMethodName;
				mTargetTime = pTime;
				mElapsedEffect = Time.realtimeSinceStartup;
				GameController.mUpdateRealTimeDelegate += InternalUpdate;
			}
		}
	}
	
	protected virtual void OnEnable()
	{
		if( mEnableTime > -1 )
			InvokeInternal( mUpdateOnPause, "Play", mEnableTime);
	}
	
	protected virtual void OnDisable()
	{
		GameController.mUpdateRealTimeDelegate -= InternalUpdate;
		mPlayed = false;
		mCurrentRepeatC = 0;
		if( mDisableTime > -1 )
		{
			gameObject.SetActiveRecursively( true);
			InvokeInternal( mUpdateOnPause, "Play", mDisableTime);		
		}
	}
	
	void InternalUpdate( float pCurrentTime )
	{
		if( pCurrentTime - mElapsedEffect > mTargetTime )
		{
			if( this != null)
				Invoke( mMethodName, 0);
			GameController.mUpdateRealTimeDelegate -= InternalUpdate;
		}
	}
	
	//destroy the effect
	public virtual void Destroy()
	{	
		SObject.DeleteObject( this );
		if( mEffectOnDestroy != null )
			mEffectOnDestroy.Play();
	}
}
