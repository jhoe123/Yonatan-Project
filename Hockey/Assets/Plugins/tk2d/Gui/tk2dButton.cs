using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/GUI/tk2dButton")]
public class tk2dButton : MonoBehaviour 
{
	public Camera viewCamera;
	
	// Button Up = normal state
	// Button Down = held down
	// Button Pressed = after it is pressed and activated
	
	public string buttonDownSprite = "button_down";
	public string buttonUpSprite = "button_up";
	public string buttonPressedSprite = "button_up";
	
	int buttonDownSpriteId = -1, buttonUpSpriteId = -1, buttonPressedSpriteId = -1;
	
	public AudioClip buttonDownSound = null;
	public AudioClip buttonUpSound = null;
	public AudioClip buttonPressedSound = null;

	// Delegates
	public delegate void ButtonHandlerDelegate(tk2dButton source);
	
	// Messaging
	public GameObject targetObject = null;
    public string messageName = "";
	public event ButtonHandlerDelegate ButtonPressedEvent;
	
	// Auto fire
	public event ButtonHandlerDelegate ButtonAutoFireEvent;
	public event ButtonHandlerDelegate ButtonDownEvent;
	public event ButtonHandlerDelegate ButtonUpEvent;
	
	tk2dBaseSprite sprite;
	bool buttonDown = false;
	
	public float targetScale = 1.1f;
	public float scaleTime = 0.05f;
	public float pressedWaitTime = 0.3f;
	
	// Use this for initialization
	void Start () 
	{
		if (viewCamera == null)
		{
			// Find a camera parent 
            Transform node = transform;
            while (node && node.camera == null)
            {
                node = node.parent;
            }
            if (node && node.camera != null) 
			{
				viewCamera = node.camera;
			}

			// ...otherwise, use the main camera
			if (viewCamera == null)
			{
				viewCamera = Camera.main;
			}
		}
		
		sprite = GetComponent<tk2dBaseSprite>();
		
		// Further tests for sprite not being null aren't necessary, as the IDs will default to -1 in that case. Testing them will be sufficient
		if (sprite)
		{
			// Change this to use animated sprites if necessary
			// Same concept here, lookup Ids and call Play(xxx) instead of .spriteId = xxx
			if (buttonDownSprite.Length > 0) 	buttonDownSpriteId 		= sprite.GetSpriteIdByName(buttonDownSprite);
			if (buttonUpSprite.Length > 0) 		buttonUpSpriteId 		= sprite.GetSpriteIdByName(buttonUpSprite);
			if (buttonPressedSprite.Length > 0) buttonPressedSpriteId 	= sprite.GetSpriteIdByName(buttonPressedSprite);
		}
		
		if (collider == null)
		{
			BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
			Vector3 colliderExtents = newCollider.extents;
			colliderExtents.z = 0.2f;
			newCollider.extents = colliderExtents;
		}
	}
	
	// Modify this to suit your audio solution
	// In our case, we have a global audio manager to play one shot sounds and pool them
	void PlaySound(AudioClip source)
	{
		if (audio && source)
		{
			audio.PlayOneShot(source);
		}
	}
	
	IEnumerator coScale(Vector3 defaultScale, float startScale, float endScale)
    {
		Vector3 scale = defaultScale;
		float s = 0.0f;
		while (s < scaleTime)
		{
			float t = Mathf.Clamp01(s / scaleTime);
			float scl = Mathf.Lerp(startScale, endScale, t);
			scale = defaultScale * scl;
			transform.localScale = scale;
			
			s += Time.deltaTime;
			yield return 0;
		}
		
		transform.localScale = defaultScale * endScale;
    }
	
	IEnumerator coHandleButtonPress()
	{
		buttonDown = true; // inhibit processing in Update()
		bool buttonPressed = true; // the button is currently being pressed
		
		Vector3 defaultScale = transform.localScale;
		
		// Button has been pressed for the first time, cursor/finger is still on it
		if (targetScale != 1.0f)
		{
			// Only do this when the scale is actually enabled, to save one frame of latency when not needed
			yield return StartCoroutine( coScale(defaultScale, 1.0f, targetScale) );
		}
		PlaySound(buttonDownSound);
		if (buttonDownSpriteId != -1)
			sprite.spriteId = buttonDownSpriteId;
		
		while (Input.GetMouseButton(0))
		{
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
			bool colliderHit = collider.Raycast(ray, out hitInfo, 1.0e8f);
            if (buttonPressed && !colliderHit)
			{
				if (targetScale != 1.0f)
				{
					// Finger is still on screen / button is still down, but the cursor has left the bounds of the button
					yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f) );
				}
				PlaySound(buttonUpSound);
				if (buttonUpSpriteId != -1)
					sprite.spriteId = buttonUpSpriteId;
				
				if (ButtonDownEvent != null)
				{
					ButtonDownEvent(this);
				}

				buttonPressed = false;
			}
			else if (!buttonPressed & colliderHit)
			{
				if (targetScale != 1.0f)
				{
					// Cursor had left the bounds before, but now has come back in
					yield return StartCoroutine( coScale(defaultScale, 1.0f, targetScale) );
				}
				PlaySound(buttonDownSound);
				if (buttonDownSpriteId != -1)
					sprite.spriteId =  buttonDownSpriteId;
				
				if (ButtonUpEvent != null)
				{
					ButtonUpEvent(this);
				}

				buttonPressed = true;
			}
			
			if (buttonPressed && ButtonAutoFireEvent != null)
			{
				ButtonAutoFireEvent(this);
			}
			
			yield return 0;
		}
		
		if (buttonPressed)
		{
			if (targetScale != 1.0f)
			{
				// Handle case when cursor was in bounds when the button was released / finger lifted
				yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f) );
			}
			PlaySound(buttonPressedSound);
			if (buttonPressedSpriteId != -1)
				sprite.spriteId = buttonPressedSpriteId;
				
			if (targetObject)
			{
				targetObject.SendMessage(messageName);
			}

			if (ButtonUpEvent != null)
			{
				ButtonUpEvent(this);
			}
			
			if (ButtonPressedEvent != null)
			{
				ButtonPressedEvent(this);
			}
			
			yield return new WaitForSeconds(pressedWaitTime);
			if (buttonUpSpriteId != -1)
				sprite.spriteId = buttonUpSpriteId;
		}
		
		buttonDown = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!buttonDown && Input.GetMouseButtonDown(0))
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (collider.Raycast(ray, out hitInfo, 1.0e8f))
            {
				StartCoroutine(coHandleButtonPress());
            }
        }
	}
}
