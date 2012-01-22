using UnityEngine;
using System.Collections;

public class GUIAlign : MonoBehaviour {
	public float mDistance = 1;
	
	void Start()
	{
		Transform transforms = GUIController.mGUICamera.transform;
		transform.position = transforms.position + (transforms.forward * mDistance);
	}	
}
