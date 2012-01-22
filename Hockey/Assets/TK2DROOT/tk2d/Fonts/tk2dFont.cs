using UnityEngine;
using System.Collections;

public class tk2dFont : MonoBehaviour 
{
	public Object bmFont;
	public Material material;
	public Texture texture;
    public bool dupeCaps = false; // duplicate lowercase into uc, or vice-versa, depending on which exists
	public bool flipTextureY = false;
	
	public int targetHeight = 640;
	public float targetOrthoSize = 1.0f;
	
	[HideInInspector] [System.NonSerialized]
	public int numCharacters = 256;
	
	public tk2dFontData data;
}
