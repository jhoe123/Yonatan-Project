using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	void OnTriggerEnter( Collider pGoal)
	{
		//if the ball collide to bottom. it mean the top player goal
		if( pGoal.tag == "Bottom" )
			GameplayScene.mCurrent.OnPlayerGoalStart( GameplayScene.topPlayer);
		else
			GameplayScene.mCurrent.OnPlayerGoalStart( GameplayScene.bottomPlayer);
	}
}
