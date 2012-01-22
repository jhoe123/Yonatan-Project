using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dBatchedSprite
{
	public string name = ""; // for editing
	public int spriteId = 0;
	public Quaternion rotation = Quaternion.identity;
	public Vector3 position = Vector3.zero;
	public Vector3 localScale = Vector3.one;
	public Color color = Color.white;
	public bool alwaysPixelPerfect = false;
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dStaticSpriteBatcher : MonoBehaviour
{
	[SerializeField] Vector3[] meshVertices;
	[SerializeField] Color[] meshColors;
	[SerializeField] Vector2[] meshUvs;
	[SerializeField] int[] meshIndices;
	public tk2dBatchedSprite[] batchedSprites = null;
	public tk2dSpriteCollectionData spriteCollection = null;
	
	void Awake()
	{
		Build();
	}
	
	public void Build()
	{
		if (!spriteCollection || batchedSprites == null || batchedSprites.Length == 0)
		{
			Mesh emptyMesh = new Mesh();
			GetComponent<MeshFilter>().mesh = emptyMesh;
		}
		else
		{
			int numVertices = 0;
			int numIndices = 0;
			foreach (var sprite in batchedSprites) 
			{
				var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
				numVertices += spriteData.positions.Length;
				numIndices += spriteData.indices.Length;
			}
			
			meshVertices = new Vector3[numVertices];
			meshColors = new Color[numVertices];
			meshUvs = new Vector2[numVertices];
			meshIndices = new int[numIndices];
			
			int currVertex = 0;
			int currIndex = 0;
			
			foreach (var sprite in batchedSprites)
			{
				var spriteData = spriteCollection.spriteDefinitions[sprite.spriteId];
				
				for (int i = 0; i < spriteData.indices.Length; ++i)
				{
					meshIndices[currIndex + i] = currVertex + spriteData.indices[i];
				}
				
				for (int i = 0; i < spriteData.positions.Length; ++i)
				{
					Vector3 pos = spriteData.positions[i];
					pos.x *= sprite.localScale.x;
					pos.y *= sprite.localScale.y;
					pos.z *= sprite.localScale.z;
					pos = sprite.rotation * pos;
					pos += sprite.position;
					meshVertices[currVertex + i] = pos;
					meshUvs[currVertex + i] = spriteData.uvs[i];
					meshColors[currVertex + i] = sprite.color;
				}
				
				currIndex += spriteData.indices.Length;
				currVertex += spriteData.positions.Length;
			}
			
			Mesh mesh = new Mesh();
	        mesh.vertices = meshVertices;
	        mesh.uv = meshUvs;
	        mesh.colors = meshColors;
	        mesh.triangles = meshIndices;
			mesh.RecalculateBounds();
			GetComponent<MeshFilter>().mesh = mesh;
			
			// Only one material supported for now
			if (renderer.sharedMaterial != spriteCollection.materials[0])
				renderer.material = spriteCollection.materials[0];
		}
	}
}
