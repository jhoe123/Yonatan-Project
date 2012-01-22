using UnityEngine;
using System.Collections;

public abstract class tk2dBaseSprite : MonoBehaviour
{
    public tk2dSpriteCollectionData collection;
	
	[SerializeField] protected Color _color = Color.white;
	[SerializeField] protected Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f);
	[SerializeField] protected int _spriteId = 0;
	public bool pixelPerfect = false;

	public Color color 
	{ 
		get { return _color; } 
		set 
		{
			if (value != _color)
			{
				_color = value;
				UpdateColors();
			}
		} 
	}
	
	public Vector3 scale 
	{ 
		get { return _scale; } 
		set
		{
			if (value != _scale)
			{
				_scale = value;
				UpdateVertices();
			}
		}
	}
	
	public int spriteId 
	{ 
		get { return _spriteId; } 
		set 
		{
			if (value != _spriteId)
			{
				value = Mathf.Clamp(value, 0, collection.spriteDefinitions.Length - 1);
				if (GetCurrentVertexCount() != collection.spriteDefinitions[value].indices.Length)
				{
					_spriteId = value;
					UpdateGeometry();
				}
				else
				{
					_spriteId = value;
					UpdateVertices();
				}
				UpdateMaterial();
			}
		} 
	}
	
	public void SwitchCollectionAndSprite(tk2dSpriteCollectionData newCollection, int newSpriteId)
	{
		if (collection != newCollection)
		{
			collection = newCollection; 
			UpdateMaterial();
		}
		_spriteId = -1; // force an update
		spriteId = newSpriteId;
	}
	
	public void MakePixelPerfect()
	{
		float s = 1.0f;
		tk2dPixelPerfectHelper pph = tk2dPixelPerfectHelper.inst;
		if (pph)
		{
			if (pph.CameraIsOrtho)
			{
				s = pph.scaleK;
			}
			else
			{
				s = pph.scaleK + pph.scaleD * transform.position.z;
			}
		}
		else if (Camera.main)
		{
			if (Camera.main.isOrthoGraphic)
			{
				s = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = (transform.position.z - Camera.main.transform.position.z);
				s = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fov, zdist);
			}
		}
		scale = new Vector3(Mathf.Sign(scale.x) * s, Mathf.Sign(scale.y) * s, Mathf.Sign(scale.z) * s);
	}	
		
	
	protected abstract void UpdateMaterial(); // update material when switching spritecollection
	protected abstract void UpdateColors(); // reupload color data only
	protected abstract void UpdateVertices(); // reupload vertex data only
	protected abstract void UpdateGeometry(); // update full geometry (including indices)
	protected abstract int  GetCurrentVertexCount(); // return current vertex count
	
	public abstract void Build();
	
	public int GetSpriteIdByName(string name)
	{
		for (int i = 0; i < collection.Count; ++i)
		{
			if (collection.spriteDefinitions[i].name == name) return i;
		}
		return 0; // default to first sprite
	}
	
	protected int GetNumVertices()
	{
		return collection.spriteDefinitions[spriteId].positions.Length;
	}
	
	protected int GetNumIndices()
	{
		return collection.spriteDefinitions[spriteId].indices.Length;
	}
	
	protected void SetPositions(Vector3[] dest)	
	{
		var sprite = collection.spriteDefinitions[spriteId];
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
		{
			dest[i].x = sprite.positions[i].x * _scale.x;
			dest[i].y = sprite.positions[i].y * _scale.y;
			dest[i].z = sprite.positions[i].z * _scale.z;
		}
	}
	
	protected void SetColors(Color[] dest)
	{
		Color c = _color;
        if (collection.premultipliedAlpha) { c.r *= c.a; c.g *= c.a; c.b *= c.a; }
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; ++i)
			dest[i] = c;
	}
	
	protected Bounds GetBounds()
	{
		var sprite = collection.spriteDefinitions[_spriteId];
		return new Bounds(new Vector3(sprite.boundsData[0].x * _scale.x, sprite.boundsData[0].y * _scale.y, sprite.boundsData[0].z * _scale.z),
		                  new Vector3(sprite.boundsData[1].x * _scale.x, sprite.boundsData[1].y * _scale.y, sprite.boundsData[1].z * _scale.z));
	}
	
	// Unity functions
	public void Start()
	{
		if (pixelPerfect)
			MakePixelPerfect();
	}
}
