using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dFontChar
{
    public Vector3 p0, p1;
    public Vector3 uv0, uv1;
    public float advance;
}

[System.Serializable]
public class tk2dFontKerning
{
	public int c0, c1;
	public float amount;
}

public class tk2dFontData : MonoBehaviour		
{
    public float lineHeight;

	public tk2dFontChar[] chars;
	public tk2dFontKerning[] kerning;
	
	public float largestWidth;
	
	public Material material;
}
