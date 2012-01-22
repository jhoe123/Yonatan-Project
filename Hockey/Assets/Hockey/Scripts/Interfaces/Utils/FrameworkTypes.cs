using UnityEngine;
using System.Collections;

public enum eSceneType
{
	Intro,
	Menu,
	Level,
	Credits,
}

public enum eStateList
{
	Nothing,
	OnCreate,
	Initialize,
	Enabled,
	Disabled,
	Pause,
	UnPause,
	Reset,
	UnInitialize,
	
	//scene
	SceneStarted,
}

public enum eEnemyState
{
	Idle,
	Walking,
	Attacking,
}

public enum InputType
{
	Touch = 0,
	Mouse = 1,
	Scroll = 2,
	Keyboard = 3,
	Joystick = 4,	
}

public enum eWeaponType
{
	Default = 0,
	Blaster = 1,		//or default
	Bomb = 2,
	Fire = 3,
	Snow = 4,
}

public enum ActorType
{
	NPC,			//invulnerable to all
	Comrade,
	Opponent,
	Monster,		//enemy of both comarade and opponent
}

public struct sRoomInfo
{
	public string mRoomId;
	public string mRoomName;							
	public ArrayList mConnectedUsers;							//the dota users on the games
	public float mEndTime;
	public float mStartedTime;
}