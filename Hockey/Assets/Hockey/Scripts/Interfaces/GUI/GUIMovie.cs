using UnityEngine;
using System.Collections;

public class GUIMovie : GUIObject, GUIPressable {
	
	
	bool mIsPressed = false;
	public bool isPressed
	{
		get{ return mIsPressed;}
		set{ mIsPressed = value; }
	}
	
	public virtual void OnPush( Vector3 pCursorPos, int pCursorIndex )
	{}
	
	public virtual void OnRelease( Vector3 pCursorPos, int pCursorIndex )
	{}
	
#if UNITY_WEB
	MovieTexture mTexture;
	AudioSource mAudioSource;
	
	public override void SetState (eStateList pState, object[] pParams)
	{
		base.SetState (pState, pParams);
		if( pState == eStateList.Initialize )
		{
			mTexture = (mRenderer as MeshRenderer).material.mainTexture as MovieTexture;
			mAudioSource = audio;
			if( mAudioSource == null )
				mAudioSource = gameObject.AddComponent<AudioSource>();
			
			mAudioSource.clip = mTexture.audioClip;
			Stop();
		}
	}
	
	float mVolume = 1;
	public float volume
	{
		get{ return mVolume; }
		set{ 
			mVolume = value;
			mAudioSource.volume = value;
		}
	}
	
	public bool isPlaying
	{
		get{ return mTexture.isPlaying; }
	}
	
	public virtual void Play( float pVolume )
	{
		mTexture.Play();
		volume = pVolume;
		mAudioSource.Play();
	}
	
	public virtual void Pause()
	{
		mTexture.Pause();
		mAudioSource.Pause();
	}
	
	public virtual void Stop()
	{
		mTexture.Stop();
		mAudioSource.Stop();
	}
#endif
}
