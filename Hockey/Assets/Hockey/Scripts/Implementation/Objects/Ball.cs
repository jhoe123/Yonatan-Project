using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	
	public AudioClip mSoundOnCollision;
	AudioSource mAudio;
	Rigidbody mRigid;
	
	// Use this for initialization
	void Start () {
		mAudio = audio;
		if( mAudio == null )
			mAudio = gameObject.AddComponent<AudioSource>();
		mAudio.clip = mSoundOnCollision;
		mRigid = rigidbody;
	}
	
	//play sound on collision
	void OnCollisionEnter( Collision pCollision)
	{
		//Debug.Log( mRigid.velocity.magnitude) ;
		mAudio.pitch = (mRigid.velocity.magnitude/30.0f) + 0.2f;
		mAudio.Play();
	}
	
	//check if goaled
	void OnTriggerEnter( Collider pGoal)
	{	
		if( GameplayNetwork.isHost)
		{
			//if the ball collide to bottom. it mean the top player goal
			if( pGoal.tag == "Bottom" )
				GameplayScene.mCurrent.OnPlayerGoalStart( GameplayScene.topPlayer);
			else
				GameplayScene.mCurrent.OnPlayerGoalStart( GameplayScene.bottomPlayer);
		}
	}
	
}
