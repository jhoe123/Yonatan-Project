using UnityEngine;
using System.Collections;
using System;

public interface GUIPressable {
	
	bool isPressed
	{
		get;
		set;
	}
	
	//to be called when the object was push
	//@param 1: the current cursor point
	//@param 2: the cursor index was used to trigger this
	void OnPush( Vector3 pCursorPosition, int pCursorIndex );
	
	//to be called when the object was release
	//@param 1: the current cursor point
	//@param 2: the cursor index was used to trigger this
	void OnRelease( Vector3 pCursorPosition, int pCursorIndex);
	
}
