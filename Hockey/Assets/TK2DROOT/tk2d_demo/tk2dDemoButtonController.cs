using UnityEngine;
using System.Collections;

public class tk2dDemoButtonController : MonoBehaviour 
{
	float spinSpeed = 0.0f;
	
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(Vector3.up, spinSpeed * Time.deltaTime);
	}
	
	void SpinLeft()
	{
		spinSpeed = 4.0f;
	}
	
	void SpinRight()
	{
		spinSpeed = -4.0f;
	}
	
	void StopSpinning()
	{
		spinSpeed = 0.0f;
	}
}
