using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class tk2dEditorUtility
{
	public static string CreateNewPrefab(string name) // name is the filename of the prefab EXCLUDING .prefab
	{
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string dirPrefix = "";
		if (assetPath.Length > 0)
		{
			dirPrefix = Application.dataPath + "/" + assetPath.Substring(7);
			dirPrefix = dirPrefix.Replace('\\', '/');
			if ((File.GetAttributes(dirPrefix) & FileAttributes.Directory) != FileAttributes.Directory)
			{
				for (int i = dirPrefix.Length - 1; i > 0; --i)
				{
					if (dirPrefix[i] == '/')
					{
						dirPrefix = dirPrefix.Substring(0, i);
						break;
					}
				}
			}
			dirPrefix += "/";
		}
		else
		{
			dirPrefix = Application.dataPath + "/";
		}
		
		// find a unique filename
		string fname = name + ".prefab";
		if (File.Exists(dirPrefix + fname))
		{
			for (int i = 0; i < 100; ++i)
			{
				fname = name + i.ToString() + ".prefab";
				if (!File.Exists(dirPrefix + fname))
					break;
			}
		}
		if (File.Exists(dirPrefix + fname))
		{
			EditorUtility.DisplayDialog("Fatal error", "Please rename sprite collections", "Ok");
			return null;
		}
		
        string path = dirPrefix + fname;
		path = path.Substring(Application.dataPath.Length - 6);
			
		return path;
	}
	
	
	const string indexPath = "Assets/.tk2d.prefab";
	static tk2dIndex index = null;
	
	public static tk2dIndex GetExistingIndex()
	{
		if (index == null)
		{
			GameObject go = AssetDatabase.LoadAssetAtPath( indexPath, typeof(GameObject) ) as GameObject;
			if (go)
			{
				index = go.GetComponent<tk2dIndex>();
			}
		}
		return index;
	}
	
	public static tk2dIndex GetOrCreateIndex()
	{
		tk2dIndex thisIndex = GetExistingIndex();
		if (thisIndex == null)
		{
			CreateIndex();
			thisIndex = GetExistingIndex();
		}
		
		return thisIndex;
	}
	
	public static void CommitIndex()
	{
		if (index)
		{
			EditorUtility.SetDirty(index);
		}
	}
	
	static void CreateIndex()
	{
		GameObject go = new GameObject();
		tk2dIndex newIndex = go.AddComponent<tk2dIndex>();
		go.active = false;
		
		List<string> rebuildSpriteCollectionPaths = new List<string>();
		
		// check all prefabs to see if we can find any objects we are interested in
		List<string> allPrefabPaths = new List<string>();
		Stack<string> paths = new Stack<string>();
		paths.Push(Application.dataPath);
		while (paths.Count != 0)
		{
			string path = paths.Pop();
			string[] files = Directory.GetFiles(path, "*.prefab");
			foreach (var file in files)
			{
				allPrefabPaths.Add(file.Substring(Application.dataPath.Length - 6));
			}
			
			foreach (string subdirs in Directory.GetDirectories(path)) 
				paths.Push(subdirs);
		}	
		
		// Check all prefabs
		int currPrefabCount = 1;
		foreach (string prefabPath in allPrefabPaths)
		{
			EditorUtility.DisplayProgressBar("Rebuilding Index", "Scanning project folder...", (float)currPrefabCount / (float)(allPrefabPaths.Count));
			
			GameObject iterGo = AssetDatabase.LoadAssetAtPath( prefabPath, typeof(GameObject) ) as GameObject;
			if (!iterGo) continue;
			
			tk2dSpriteCollection spriteCollection = iterGo.GetComponent<tk2dSpriteCollection>();
			tk2dFont font = iterGo.GetComponent<tk2dFont>();
			tk2dSpriteAnimation anims = iterGo.GetComponent<tk2dSpriteAnimation>();
			
			if (spriteCollection) 
			{
				tk2dSpriteCollectionData spriteCollectionData = spriteCollection.spriteCollection;
				if (spriteCollectionData)
				{
					if (spriteCollectionData.version < 1)
					{
						rebuildSpriteCollectionPaths.Add( AssetDatabase.GetAssetPath(spriteCollection ));
					}
					newIndex.AddSpriteCollectionData( spriteCollectionData );
				}
			}
			else if (font) newIndex.AddFont(font);
			else if (anims) newIndex.AddSpriteAnimation(anims);
			else
			{
				iterGo = null;
				System.GC.Collect();
			}

			tk2dEditorUtility.UnloadUnusedAssets();	
			++currPrefabCount;
		}
		EditorUtility.ClearProgressBar();
		
		// Create index
		Object p = EditorUtility.CreateEmptyPrefab(indexPath);
		EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		GameObject.DestroyImmediate(go);
		
		// unload all unused assets
		tk2dEditorUtility.UnloadUnusedAssets();
		
		// Rebuild invalid sprite collections
		if (rebuildSpriteCollectionPaths.Count > 0)
		{
			EditorUtility.DisplayDialog("Upgrade required",
			                            "Please wait while your sprite collection is upgraded.",
			                            "Ok");
			
			int count = 1;
			foreach (var scPath in rebuildSpriteCollectionPaths)
			{
				tk2dSpriteCollection sc = AssetDatabase.LoadAssetAtPath(scPath, typeof(tk2dSpriteCollection)) as tk2dSpriteCollection;
				EditorUtility.DisplayProgressBar("Rebuilding Sprite Collections", "Rebuilding Sprite Collection: " + sc.name, (float)count / (float)(rebuildSpriteCollectionPaths.Count));

				tk2dSpriteCollectionBuilder.Rebuild(sc);
				sc = null;
				
				tk2dEditorUtility.UnloadUnusedAssets();
				
				++count;
			}
			
			EditorUtility.ClearProgressBar();
		}
	}
	
	[System.ObsoleteAttribute]
	static T[] FindPrefabsInProjectWithComponent<T>() where T : Component
	// returns null if nothing is found
	{
		List<T> allGens = new List<T>();
		
		Stack<string> paths = new Stack<string>();
		paths.Push(Application.dataPath);
		while (paths.Count != 0)
		{
			string path = paths.Pop();
			string[] files = Directory.GetFiles(path, "*.prefab");
			foreach (var file in files)
			{
				GameObject go = AssetDatabase.LoadAssetAtPath( file.Substring(Application.dataPath.Length - 6), typeof(GameObject) ) as GameObject;
				if (!go) continue;
				
				T gen = go.GetComponent<T>();
				if (gen)
				{
					allGens.Add(gen);
				}
			}
			
			foreach (string subdirs in Directory.GetDirectories(path)) 
				paths.Push(subdirs);
		}
		
		if (allGens.Count == 0) return null;
		
		T[] allGensArray = new T[allGens.Count];
		for (int i = 0; i < allGens.Count; ++i)
			allGensArray[i] = allGens[i];
		return allGensArray;
	}
	
	public static GameObject CreateGameObjectInScene(string name)
	{
		string realName = name;
		int counter = 0;
		while (GameObject.Find(realName) != null)
		{
			realName = name + counter++;
		}
		
        GameObject go = new GameObject(realName);
		if (Selection.activeGameObject != null)
		{
			string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
			if (assetPath.Length == 0) go.transform.parent = Selection.activeGameObject.transform;
		}
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;	
        return go;
	}
	
	public static void DrawMeshBounds(Mesh mesh, Transform transform, Color c)
	{
		var e = mesh.bounds.extents;
		Vector3[] boundPoints = new Vector3[] {
			mesh.bounds.center + new Vector3(-e.x, e.y, 0.0f),
			mesh.bounds.center + new Vector3( e.x, e.y, 0.0f),
			mesh.bounds.center + new Vector3( e.x,-e.y, 0.0f),
			mesh.bounds.center + new Vector3(-e.x,-e.y, 0.0f),
			mesh.bounds.center + new Vector3(-e.x, e.y, 0.0f) };
		
		for (int i = 0; i < boundPoints.Length; ++i)
			boundPoints[i] = transform.TransformPoint(boundPoints[i]);
		
		Handles.color = c;
		Handles.DrawPolyLine(boundPoints);
	}
	
	public static void UnloadUnusedAssets()
	{
		Object previousSelectedObject = Selection.activeObject;
		Selection.activeObject = null;

		EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
		System.GC.Collect();
		
		index = null;

		Selection.activeObject = previousSelectedObject;
	}	
}
