using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dSpriteCollectionDefinition
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
		Custom
    }
	
	public enum Pad
	{
		Default,
		BlackZeroAlpha,
		Extend,
	}
	
	public string name = "";
	
    public bool additive = false;
    public Vector3 scale = new Vector3(1,1,1);
    
	[HideInInspector]
    public Texture2D texture;
	
	[HideInInspector] [System.NonSerialized]
	public Texture2D thumbnailTexture;
	
	public Anchor anchor = Anchor.MiddleCenter;
	public float anchorX, anchorY;
    public Object overrideMesh;
	
	public bool dice = false;
	public int diceUnitX = 64;
	public int diceUnitY = 0;
	
	public Pad pad = Pad.Default;
	
	public bool fromSpriteSheet = false;
	public bool extractRegion = false;
	public int regionX, regionY, regionW, regionH;
	public int regionId;
	
	public void CopyFrom(tk2dSpriteCollectionDefinition src)
	{
		additive = src.additive;
		scale = src.scale;
		texture = src.texture;
		anchor = src.anchor;
		anchorX = src.anchorX;
		anchorY = src.anchorY;
		overrideMesh = src.overrideMesh;
		dice = src.dice;
		diceUnitX = src.diceUnitX;
		diceUnitY = src.diceUnitY;
		pad = src.pad;
		
		fromSpriteSheet = src.fromSpriteSheet;
		extractRegion = src.extractRegion;
		regionX = src.regionX;
		regionY = src.regionY;
		regionW = src.regionW;
		regionH = src.regionH;
		regionId = src.regionId;
	}
}

[System.Serializable]
public class tk2dSpriteSheetSource
{
    public enum Anchor
    {
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight,
    }	
	
	public Texture2D texture;
	public int tilesX, tilesY;
	public int numTiles = 0;
	public Anchor anchor = Anchor.MiddleCenter;
	public tk2dSpriteCollectionDefinition.Pad pad = tk2dSpriteCollectionDefinition.Pad.Default;
	public Vector3 scale = new Vector3(1,1,1);
}

public class tk2dSpriteCollection : MonoBehaviour 
{
    // legacy data
    [HideInInspector]
    public tk2dSpriteCollectionDefinition[] textures;

    // new method
    public Texture2D[] textureRefs;
	public tk2dSpriteSheetSource[] spriteSheets;
	
	[HideInInspector]
	public int maxTextureSize = 1024;
	
	public enum TextureCompression
	{
		Uncompressed,
		Reduced16Bit,
		Compressed
	}
	[HideInInspector]
	public TextureCompression textureCompression = TextureCompression.Uncompressed;
	
	[HideInInspector]
	public int atlasWidth, atlasHeight;
	[HideInInspector]
	public float atlasWastage;
	[HideInInspector]
	public bool allowMultipleAtlases = false;
	
	[HideInInspector]
    public tk2dSpriteCollectionDefinition[] textureParams;
    
	public tk2dSpriteCollectionData spriteCollection;
    public bool premultipliedAlpha = true;
	
	public Material[] atlasMaterials;
	public Texture2D[] atlasTextures;

	public int targetHeight = 640;
	public float targetOrthoSize = 1.0f;
	
	public bool pixelPerfectPointSampled = false;
	
	[HideInInspector]
	public bool autoUpdate = true;
}
