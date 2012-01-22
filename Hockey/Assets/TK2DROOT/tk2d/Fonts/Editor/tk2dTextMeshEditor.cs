using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(tk2dTextMesh))]
class tk2dTextMeshEditor : Editor
{
	tk2dFont[] allBmFontImporters = null;	// all generators
	
    public override void OnInspectorGUI()
    {
        tk2dTextMesh textMesh = (tk2dTextMesh)target;
		
		// maybe cache this if its too slow later
		if (allBmFontImporters == null) allBmFontImporters = tk2dEditorUtility.GetOrCreateIndex().GetFonts();
		
		if (allBmFontImporters != null)
        {
			if (textMesh.font == null)
			{
				textMesh.font = allBmFontImporters[0].data;
			}
			
			int currId = -1;
			string[] fontNames = new string[allBmFontImporters.Length];
			for (int i = 0; i < allBmFontImporters.Length; ++i)
			{
				fontNames[i] = allBmFontImporters[i].name;
				if (allBmFontImporters[i].data == textMesh.font)
				{
					currId = i;
				}
			}
			
			int newId = EditorGUILayout.Popup("Font", currId, fontNames);
			if (newId != currId)
			{
				textMesh.font = allBmFontImporters[newId].data;
				textMesh.renderer.material = allBmFontImporters[newId].material;
			}
			
			textMesh.maxChars = EditorGUILayout.IntField("Max Chars", textMesh.maxChars);
			textMesh.text = EditorGUILayout.TextField("Text", textMesh.text);
			textMesh.anchor = (TextAnchor)EditorGUILayout.EnumPopup("Anchor", textMesh.anchor);
			textMesh.kerning = EditorGUILayout.Toggle("Kerning", textMesh.kerning);
			textMesh.scale = EditorGUILayout.Vector3Field("Scale", textMesh.scale);
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("HFlip"))
			{
				Vector3 s = textMesh.scale;
				s.x *= -1.0f;
				textMesh.scale = s;
				GUI.changed = true;
			}
			if (GUILayout.Button("VFlip"))
			{
				Vector3 s = textMesh.scale;
				s.y *= -1.0f;
				textMesh.scale = s;
				GUI.changed = true;
			}			
			
			if ( GUILayout.Button("Make Pixel Perfect", GUILayout.ExpandWidth(true) ))
			{
				if (tk2dPixelPerfectHelper.inst) tk2dPixelPerfectHelper.inst.Setup();
				textMesh.MakePixelPerfect();
				GUI.changed = true;
			}
			textMesh.pixelPerfect = GUILayout.Toggle(textMesh.pixelPerfect, "Always", GUILayout.Width(60.0f));
			EditorGUILayout.EndHorizontal();
			
			textMesh.useGradient = EditorGUILayout.Toggle("Use Gradient", textMesh.useGradient);
			if (textMesh.useGradient)
			{
				textMesh.color = EditorGUILayout.ColorField("Top Color", textMesh.color);
				textMesh.color2 = EditorGUILayout.ColorField("Bottom Color", textMesh.color2);
			}
			else
			{
				textMesh.color = EditorGUILayout.ColorField("Color", textMesh.color);
			}
			
			if (GUI.changed)
			{
				textMesh.Commit();
				EditorUtility.SetDirty(textMesh);
			}
		}
	}
}
