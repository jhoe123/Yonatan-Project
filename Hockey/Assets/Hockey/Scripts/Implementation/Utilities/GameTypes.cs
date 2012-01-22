using UnityEngine;
using System.Collections;

/*
 * Holds player information
 * */
public class PlayerInfo
{
	public string id;				//the id of player
	public string name;				//the name of player
	public NetworkPlayer networkPlayer;//the network player
	
	bool mIsOnline = false;			//true if online while false if not
	public bool isOnline
	{ get{ return mIsOnline;}}
	
	public PlayerInfo( string pId, string pName, bool pIsOnline)
	{
		id = pId;
		name = pName;
		mIsOnline = pIsOnline;
	}
}

/*Holds game information*/
public class GameInfo
{
	
	public PlayerInfo owner;
	public PlayerInfo opponent;
	public int ownerScore;
	public int opponentScore;
	public PlayerInfo winner;
	public PlayerInfo losser;
	public System.DateTimeOffset startDate;
	public System.DateTimeOffset endDate;
	
	public GameInfo( PlayerInfo pOwner, PlayerInfo pOpponent, int pOwnerScore, int pOpponentScore)
	{
		owner = pOwner;
		opponent = pOpponent;
		ownerScore = pOwnerScore;
		opponentScore = pOpponentScore;
	}
}

public enum eAI
{
	Easy,
	Medium,
	Hard,
	Expert
}

