using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/tk2d9SliceSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dSlicedSprite : tk2dBaseSprite
{
	Mesh mesh;
	Vector2[] meshUvs;
	Vector3[] meshVertices;
	Color[] meshColors;
	int[] meshIndices;
	
	public float borderTop = 0.2f, borderBottom = 0.2f;
	public float borderLeft = 0.2f, borderRight = 0.2f;
	
	void Awake()
	{
		// This will not be set when instantiating in code
		// In that case, Build will need to be called
		if (collection)
		{
			// reset spriteId if outside bounds
			// this is when the sprite collection data is corrupt
			if (_spriteId < 0 || _spriteId >= collection.Count)
				_spriteId = 0;
			
			Build();
		}
	}
	
	protected void OnDestroy()
	{
		if (mesh)
		{
#if UNITY_EDITOR
			DestroyImmediate(mesh);
#else
			Destroy(mesh);
#endif
		}
	}
	
	new protected void SetColors(Color[] dest)
	{
		Color c = _color;
        if (collection.premultipliedAlpha) { c.r *= c.a; c.g *= c.a; c.b *= c.a; }
		for (int i = 0; i < dest.Length; ++i)
			dest[i] = c;
	}
	
	protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
	{
		var sprite = collection.spriteDefinitions[spriteId];
		
		if (sprite.positions.Length == 4)
		{
			float sx = _scale.x;
			float sy = _scale.y;
			
			Vector3[] srcVert = sprite.positions;
			Vector3 dx = srcVert[1] - srcVert[0];
			Vector3 dy = srcVert[2] - srcVert[0];
			
			Vector2[] srcUv = sprite.uvs;
			Vector2 duvx = sprite.uvs[1] - sprite.uvs[0];
			Vector2 duvy = sprite.uvs[2] - sprite.uvs[0];
			
			Vector3 origin = new Vector3(srcVert[0].x * sx, srcVert[0].y * sy, srcVert[0].z * _scale.z);
			
			Vector3[] originPoints = new Vector3[4] {
				origin,
				origin + dy * borderBottom,
				origin + dy * (sy - borderTop),
				origin + dy * sy
			};
			Vector2[] originUvs = new Vector2[4] {
				srcUv[0],
				srcUv[0] + duvy * borderBottom,
				srcUv[0] + duvy * (1 - borderTop),
				srcUv[0] + duvy,
			};
			
			for (int i = 0; i < 4; ++i)
			{
				meshVertices[i * 4 + 0] = originPoints[i];
				meshVertices[i * 4 + 1] = originPoints[i] + dx * borderLeft;
				meshVertices[i * 4 + 2] = originPoints[i] + dx * (sx - borderRight);
				meshVertices[i * 4 + 3] = originPoints[i] + dx * sx;
				meshUvs[i * 4 + 0] = originUvs[i];
				meshUvs[i * 4 + 1] = originUvs[i] + duvx * borderLeft;
				meshUvs[i * 4 + 2] = originUvs[i] + duvx * (1 - borderRight);
				meshUvs[i * 4 + 3] = originUvs[i] + duvx;
			}
		}
		else
		{
			for (int i = 0; i < vertices.Length; ++i)
				vertices[i] = Vector3.zero;
		}
	}
	
	void SetIndices()
	{
		meshIndices = new int[9 * 6] {
			0, 4, 1, 1, 4, 5,
			1, 5, 2, 2, 5, 6,
			2, 6, 3, 3, 6, 7,
			4, 8, 5, 5, 8, 9,
			5, 9, 6, 6, 9, 10,
			6, 10, 7, 7, 10, 11,
			8, 12, 9, 9, 12, 13,
			9, 13, 10, 10, 13, 14,
			10, 14, 11, 11, 14, 15
		};		
	}
	
	public override void Build()
	{
		meshUvs = new Vector2[16];
		meshVertices = new Vector3[16];
		meshColors = new Color[16];
		SetIndices();
		
		SetGeometry(meshVertices, meshUvs);
		SetColors(meshColors);
		
		Mesh newMesh = new Mesh();
		newMesh.vertices = meshVertices;
		newMesh.colors = meshColors;
		newMesh.uv = meshUvs;
		newMesh.triangles = meshIndices;
		
		GetComponent<MeshFilter>().mesh = newMesh;
		mesh = GetComponent<MeshFilter>().sharedMesh;
		
		UpdateMaterial();
	}
	
	protected override void UpdateGeometry() { UpdateGeometryImpl(); }
	protected override void UpdateColors() { UpdateColorsImpl(); }
	protected override void UpdateVertices() { UpdateGeometryImpl(); }
	
	
	protected void UpdateColorsImpl()
	{
#if UNITY_EDITOR
		// This can happen with prefabs in the inspector
		if (meshColors == null || meshColors.Length == 0)
			return;
#endif
		
		SetColors(meshColors);
		mesh.colors = meshColors;
	}

	protected void UpdateGeometryImpl()
	{
#if UNITY_EDITOR
		// This can happen with prefabs in the inspector
		if (mesh == null)
			return;
#endif
		SetGeometry(meshVertices, meshUvs);
		mesh.vertices = meshVertices;
		mesh.uv = meshUvs;
		mesh.RecalculateBounds();
	}
	
	protected override void UpdateMaterial()
	{
		if (renderer.sharedMaterial != collection.spriteDefinitions[spriteId].material)
			renderer.material = collection.spriteDefinitions[spriteId].material;
	}
	
	protected override int GetCurrentVertexCount()
	{
#if UNITY_EDITOR
		if (meshVertices == null)
			return 0;
#endif
		return 16;
	}
}
