using UnityEngine;
using System.Collections;

public class AutoPlayOnline : MonoBehaviour, JoiningStatusListener {
	
	void Start()
	{
		GameHelpers.AutoJoinGame( this);
	}
	
	void OnDestroy()
	{
		GameHelpers.AutoJoinCancel();
	}
	
	public void OnStartSearch ()
	{
		Debug.Log( "OnStartSearch");
	}
	
	public void OnRoomJoined (HostData pData)
	{
		Debug.Log("OnRoomJoined");
	}
	
	public void OnRoomCreated ()
	{
		Debug.Log("OnRoomCreated");
	}
	
	public void OnRoomReady ()
	{
		Debug.Log("RoomReady");
	}
	
	public void OnRoomClose ()
	{
		Debug.Log("RoomClose");
	}
	
	public void OnError ()
	{
		throw new System.NotImplementedException ();
	}
}
