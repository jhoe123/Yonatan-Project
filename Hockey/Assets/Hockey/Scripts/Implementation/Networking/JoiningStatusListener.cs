using UnityEngine;
using System.Collections;

public interface JoiningStatusListener{
	
	void OnStartSearch();
	
	void OnRoomCreated();
	
	void OnRoomJoined( HostData pData);
	
	void OnRoomReady();
	
	void OnRoomClose();
	
	void OnError();
}
