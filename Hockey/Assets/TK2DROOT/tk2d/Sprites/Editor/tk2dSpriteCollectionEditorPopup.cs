using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class tk2dSpriteCollectionEditorPopup : EditorWindow 
{
	tk2dSpriteCollection gen;
	int currSprite = 0;
	
	public void SetGenerator(tk2dSpriteCollection _gen)
	{
		gen = _gen;
	}
	
	bool alphaBlend = true;
	float displayScale = 1.0f;
	Texture2D[] flashTextures = null;
	int currFlashTex = 0;
	bool previewFoldoutEnabled = true;
	
	void OnDestroy()
	{
		tk2dSpriteThumbnailCache.ReleaseSpriteThumbnailCache();
	}
	
    void OnGUI() 
	{
		if (!gen)
		{
			EditorGUILayout.BeginVertical();
			GUILayout.Label("not loaded");
			EditorGUILayout.EndVertical();
			return;
		}
		
		if (flashTextures == null || flashTextures[0] == null)
		{
			currFlashTex = 0;
			Color[] colors = new Color[] { Color.red, Color.magenta, Color.cyan, Color.blue, Color.green, Color.white };
			flashTextures = new Texture2D[colors.Length];
			for (int i = 0; i < colors.Length; ++i)
			{
				flashTextures[i] = new Texture2D(1, 1);
				flashTextures[i].SetPixel(0, 0, colors[i]);
				flashTextures[i].Apply();
			}
		}
		
		if (currSprite < 0 || currSprite >= gen.textureRefs.Length || currSprite >= gen.textureParams.Length) 
		{	
			currSprite = 0;
		}
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.BeginVertical( GUILayout.MaxWidth(256.0f) );
		GUILayout.Space(8.0f);
		if (GUILayout.Button("Commit")) tk2dSpriteCollectionBuilder.Rebuild(gen);

		string[] spriteNames = new string[gen.textureRefs.Length];
		for (int i = 0; i < gen.textureRefs.Length; ++i)
			spriteNames[i] = gen.textureParams[i].name;
		int newSprite = EditorGUILayout.Popup(currSprite, spriteNames);
		if (newSprite != currSprite)
		{
			currSprite = newSprite;
		}

		var param = gen.textureParams[currSprite];
		
		if (param.fromSpriteSheet)
		{
			EditorGUILayout.LabelField("SpriteSheet", "Frame: " + param.regionId);
			EditorGUILayout.LabelField("Name", param.name);
		}
		else
		{
			param.name = EditorGUILayout.TextField("Name", param.name);
		}
		
		if (!param.fromSpriteSheet)
		{
			param.additive = EditorGUILayout.Toggle("Additive", param.additive);
			param.scale = EditorGUILayout.Vector3Field("Scale", param.scale);
			param.anchor = (tk2dSpriteCollectionDefinition.Anchor)EditorGUILayout.EnumPopup("Anchor", param.anchor);
			if (param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom)
			{
				EditorGUILayout.BeginHorizontal();
				param.anchorX = EditorGUILayout.FloatField("anchorX", param.anchorX);
				bool roundAnchorX = GUILayout.Button("R", GUILayout.MaxWidth(32));
				EditorGUILayout.EndHorizontal();
	
				EditorGUILayout.BeginHorizontal();
				param.anchorY = EditorGUILayout.FloatField("anchorY", param.anchorY);
				bool roundAnchorY = GUILayout.Button("R", GUILayout.MaxWidth(32));
				EditorGUILayout.EndHorizontal();
				
				if (roundAnchorX) param.anchorX = Mathf.Round(param.anchorX);
				if (roundAnchorY) param.anchorY = Mathf.Round(param.anchorY);
			}
			
			if (!gen.allowMultipleAtlases)
			{
				param.dice = EditorGUILayout.Toggle("Dice", param.dice);
				if (param.dice)
				{
					param.diceUnitX = EditorGUILayout.IntField("X", param.diceUnitX);
					param.diceUnitY = EditorGUILayout.IntField("Y", param.diceUnitY);
				}
			}
			
			param.pad = (tk2dSpriteCollectionDefinition.Pad)EditorGUILayout.EnumPopup("Pad", param.pad);
			EditorGUILayout.Separator();
		}
		
		// Warning message
		if (gen.allowMultipleAtlases)
		{
			Color bg = GUI.backgroundColor;
			GUI.backgroundColor = new Color(1.0f, 0.7f, 0.0f, 1.0f);
			GUILayout.TextArea("NOTE: Dicing is not allowed when multiple atlas build is enabled.");
			GUI.backgroundColor = bg;			
		}
		
		EditorGUILayout.EndVertical();
		
		GUILayout.Space(8.0f);
		
		
		// Preview part
		
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(192.0f));
		GUILayout.Space(8.0f);
		
		if (gen.spriteCollection.version < 1)
		{
			GUILayout.Label("No preview data.\nPlease rebuild sprite collection.");
		}
		else
		{
			previewFoldoutEnabled = EditorGUILayout.Foldout(previewFoldoutEnabled, "Preview");
			if (previewFoldoutEnabled)
			{
				var tex = tk2dSpriteThumbnailCache.GetThumbnailTexture(gen.spriteCollection, currSprite);
				DrawPreviewFoldout(param, tex);
			}
		}

		EditorGUILayout.EndVertical();
		
		
		EditorGUILayout.EndHorizontal();
    }
	
	void DrawPreviewFoldout(tk2dSpriteCollectionDefinition param, Texture2D displayTexture)
	{
		alphaBlend = GUILayout.Toggle(alphaBlend, "AlphaBlend");
		
		EditorGUILayout.BeginHorizontal();
		displayScale = EditorGUILayout.FloatField("PreviewScale", displayScale);
		displayScale = Mathf.Max(displayScale, 0.0f);
		if (GUILayout.Button("Reset")) displayScale = 1.0f;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Width: " + displayTexture.width); GUILayout.Label("Height: " + displayTexture.height); 
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		if (gen.spriteCollection != null && gen.spriteCollection.spriteDefinitions != null)
		{
			EditorGUILayout.BeginHorizontal();
			var thisSpriteData = gen.spriteCollection.spriteDefinitions[currSprite];
			GUILayout.Label("Vertices: " + thisSpriteData.positions.Length); GUILayout.Label("Triangles: " + thisSpriteData.indices.Length / 3);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		
		float offsetX = (param.anchorX < 0.0f)?-param.anchorX * displayScale:0.0f;
		float offsetY = (param.anchorY < 0.0f)?-param.anchorY * displayScale:0.0f;
		
		Rect r = GUILayoutUtility.GetRect(64.0f, displayTexture.height + 2 + offsetY);
		int border = 8;
		r.x += offsetX + border + 1;
		r.y += offsetY + border + 1;
		r.width = displayTexture.width * displayScale;
		r.height = displayTexture.height * displayScale;
		GUI.DrawTexture(r, displayTexture, ScaleMode.ScaleAndCrop, alphaBlend);
		
		// Draw outline
		{
			Vector3[] pt = new Vector3[] {
				new Vector3(r.x - 1, r.y - 1, 0.0f),
				new Vector3(r.x + r.width + 1, r.y - 1, 0.0f),
				new Vector3(r.x + r.width + 1, r.y + r.height + 1, 0.0f),
				new Vector3(r.x - 1, r.y + r.height + 1, 0.0f),
				new Vector3(r.x - 1, r.y - 1, 0.0f)
			};
			Handles.color = Color.black;
			Handles.DrawPolyLine(pt);
		}
	
		if (param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom)
		{
			float pixelSize = displayScale;
			switch (param.anchor)
			{
			case tk2dSpriteCollectionDefinition.Anchor.UpperLeft: break;
			case tk2dSpriteCollectionDefinition.Anchor.UpperCenter: r.x += r.width / 2.0f; break;
			case tk2dSpriteCollectionDefinition.Anchor.UpperRight: r.x += r.width - pixelSize; break;
			case tk2dSpriteCollectionDefinition.Anchor.LowerLeft: r.y += r.height - 1.0f; break;
			case tk2dSpriteCollectionDefinition.Anchor.LowerCenter: r.y += r.height - pixelSize; r.x += r.width / 2.0f; break;
			case tk2dSpriteCollectionDefinition.Anchor.LowerRight: r.y += r.height - pixelSize; r.x += r.width - pixelSize; break;
			case tk2dSpriteCollectionDefinition.Anchor.MiddleLeft: r.y += r.height / 2.0f; break;
			case tk2dSpriteCollectionDefinition.Anchor.MiddleCenter: r.y += r.height / 2.0f; r.x += r.width / 2.0f; break;
			case tk2dSpriteCollectionDefinition.Anchor.MiddleRight: r.y += r.height / 2.0f; r.x += r.width - pixelSize; break;
			case tk2dSpriteCollectionDefinition.Anchor.Custom: r.x += param.anchorX * pixelSize; r.y += param.anchorY * pixelSize; break;
			}
			
			r.width = 1.0f * displayScale;
			r.height = 1.0f * displayScale;
			
			if (param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom &&
			    Event.current.isMouse && Event.current.button == 0)
			{
				// Event.current.mousePosition;
				Vector2 v = HandleUtility.WorldToGUIPoint(Event.current.mousePosition);
				Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(v.x, v.y, 0));
				Handles.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(100,100, 0));
			}

			GUI.DrawTexture(r, flashTextures[currFlashTex++ % flashTextures.Length]);
			
			Repaint();
		}		
	}
}
