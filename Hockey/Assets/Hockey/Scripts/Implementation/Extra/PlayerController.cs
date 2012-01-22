using UnityEngine;
using System.Collections;

[RequireComponent( typeof(PlayerIngame))]
public class PlayerController : GUIObject, GUIPressable {
	
	[SerializeField]
	PlayerIngame mPlayer;
	public PlayerIngame player
	{ get{ return mPlayer;}}
	
	Transform mGuardTrans;
	Camera mWordCam;
	float mY;
	
	protected override void Start ()
	{
		mGuardTrans = player.ballGuard.transform;
		mWordCam = GameController.mMainCamera;
	}
	
	public void OnPush (Vector3 pCursorPosition, int pCursorIndex)
	{
		mGuardTrans.position = mWordCam.ScreenPointToRay( pCursorPosition).GetPoint( mY);
	}
	
	public void OnRelease (Vector3 pCursorPosition, int pCursorIndex)
	{}
	
	bool mIsPressed = false;
	public bool isPressed {
		get {
			return mIsPressed;
		}
		set {
			mIsPressed = value;
		}
	}
	
	public override void OnUpdate (float mCurrentTime)
	{
		base.OnUpdate (mCurrentTime);
		if( mIsPressed )
			mGuardTrans.position = mWordCam.ScreenPointToRay( GameController.mCursorScreentPoint).GetPoint( mY);
	}
}