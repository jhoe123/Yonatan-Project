using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dSpriteDefinition
{
	public string name;
	public Vector3[] boundsData;
    public Vector3[] positions;
    public Vector2[] uvs;
    public int[] indices = new int[] { 0, 3, 1, 2, 3, 0 };
	public Material material;
	
	public string sourceTextureGUID;
	public bool extractRegion;
	public int regionX, regionY, regionW, regionH;
}

public class tk2dSpriteCollectionData : MonoBehaviour 
{
	public const int CURRENT_VERSION = 1;
	
	[HideInInspector]
	public int version;
	
    [HideInInspector]
    public tk2dSpriteDefinition[] spriteDefinitions;
	
    [HideInInspector]
    public bool premultipliedAlpha;
	
	// legacy data
    [HideInInspector]
	public Material material;	
	
	[HideInInspector]
	public Material[] materials;
	
	[HideInInspector]
	public Texture[] textures;
	
	[HideInInspector]
	public bool allowMultipleAtlases;
	
	[HideInInspector]
	public string spriteCollectionGUID;
	
	[HideInInspector]
	public string spriteCollectionName;
	
    public int Count { get { return spriteDefinitions.Length; } }
}
