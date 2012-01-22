using UnityEngine;
using System.Collections;

public interface GUIFocusable
{
	void OnFocus( Vector3 pCursorPos );
	
	void OnUnfocus( Vector3 pCursorPos );
	
}