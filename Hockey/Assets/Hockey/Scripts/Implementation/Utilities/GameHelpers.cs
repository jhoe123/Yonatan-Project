using UnityEngine;
using System.Collections;

public class GameHelpers  {
	
	static PlayerInfo mLocalPlayer;
	//use to get the local player info
	public static PlayerInfo GetLocalPlayerInfo()
	{
		if( mLocalPlayer == null)
			mLocalPlayer = new PlayerInfo( "lcl", "Local Player", false);
		
		return mLocalPlayer;
	}
}
