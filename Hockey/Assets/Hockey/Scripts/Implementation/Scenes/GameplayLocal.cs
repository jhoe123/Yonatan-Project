using UnityEngine;
using System.Collections;

public class GameplayLocal : GameplayScene {
	
	PlayerController mPlayerController;
	
	protected override void Awake ()
	{
		mOwnerPlayer = mPlayer1;
		mOpponentPlayer = mPlayer2;
		mPlayerController = (PlayerController)FindObjectOfType( typeof(PlayerController));
		base.Awake ();
	}
	
	public override void OnUpdate (float mCurrentTime)
	{
		base.OnUpdate (mCurrentTime);
		mPlayerController.OnUpdate( mCurrentTime);
	}
}
