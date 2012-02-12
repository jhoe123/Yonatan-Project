using UnityEngine;
using System.Collections;

public class GameplayLocal : GameplayScene {
	
	protected override void Awake ()
	{
		mOwnerPlayer = mPlayer1;
		mOpponentPlayer = mPlayer2;
		
		base.Awake ();
	}
}
