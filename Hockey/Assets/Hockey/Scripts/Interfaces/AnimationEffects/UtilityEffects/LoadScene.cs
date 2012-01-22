using UnityEngine;
using System.Collections;

public class LoadScene : Effect {
	
	public string mSceneName;
	
	public override bool Play ()
	{
		if( base.Play ())
		{
			Application.LoadLevelAsync( mSceneName);
			return true;
		}
		
		return false;
	}
}
