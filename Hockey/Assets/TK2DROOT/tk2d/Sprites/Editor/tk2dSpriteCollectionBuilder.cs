using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class tk2dSpriteCollectionBuilder
{
	public static void ResetCurrentBuild()
	{
		currentBuild = null;
	}

	// Rebuild a sprite collection when out of date
	// Identifies changed textures by comparing GUID
	public static void RebuildOutOfDate(string[] changedPaths)
    {
		// This should only take existing indices, as we don't want to slow anything down here
		tk2dIndex index = tk2dEditorUtility.GetExistingIndex();
		if (index == null)
			return;

		tk2dSpriteCollectionData[] scg = tk2dEditorUtility.GetExistingIndex().GetSpriteCollectionData();
		if (scg == null)
			return;

        foreach (tk2dSpriteCollectionData thisScg in scg)
        {
			bool needRebuild = false;
			foreach (var def in thisScg.spriteDefinitions)
			{
				foreach (string changedPath in changedPaths)
				{
					if (def.sourceTextureGUID == AssetDatabase.AssetPathToGUID(changedPath))
					{
						needRebuild = true;
						break;
					}
				}

				if (needRebuild)
				{
					break;
				}
			}

            if (needRebuild)
            {
				tk2dSpriteCollection spriteCollectionSource = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(thisScg.spriteCollectionGUID), typeof(tk2dSpriteCollection) ) as tk2dSpriteCollection;
				if (spriteCollectionSource != null)
				{
                	Rebuild(spriteCollectionSource);
				}

				tk2dEditorUtility.UnloadUnusedAssets();
            }
        }
    }

	static int defaultPad = 2;

	static int GetPadAmount(tk2dSpriteCollection gen)
	{
		return (gen.pixelPerfectPointSampled)?0:defaultPad;
	}

	static void PadTexture(Texture2D tex, int pad, bool stretchPad)
	{
		Color bgColor = new Color(0,0,0,0);

		for (int y = 0; y < pad; ++y)
		{
			for (int x = 0; x < tex.width; ++x)
			{
				tex.SetPixel(x, y, stretchPad?tex.GetPixel(x, pad):bgColor);
				tex.SetPixel(x, tex.height - 1 - y, stretchPad?tex.GetPixel(x, tex.height - 1 - pad):bgColor);
			}
		}
		for (int x = 0; x < pad; ++x)
		{
			for (int y = 0; y < tex.height; ++y)
			{
				tex.SetPixel(x, y, stretchPad?tex.GetPixel(pad, y):bgColor);
				tex.SetPixel(tex.width - 1 - x, y, stretchPad?tex.GetPixel(tex.width - 1 - pad, y):bgColor);
			}
		}
	}

    static int NameCompare(string na, string nb)
    {
		if (na.Length == 0 && nb.Length != 0) return 1;
		else if (na.Length != 0 && nb.Length == 0) return -1;
		else if (na.Length == 0 && nb.Length == 0) return 0;

        int numStartA = na.Length - 1;

        // last char is not a number, compare as regular strings
        if (na[numStartA] < '0' || na[numStartA] > '9')
            return System.String.Compare(na, nb, true);

        while (numStartA > 0 && na[numStartA - 1] >= '0' && na[numStartA - 1] <= '9')
            numStartA--;

        int comp = System.String.Compare(na, 0, nb, 0, numStartA);

        if (comp == 0)
        {
            if (nb.Length > numStartA)
            {
                bool numeric = true;
                for (int i = numStartA; i < nb.Length; ++i)
                {
                    if (nb[i] < '0' || nb[i] > '9')
                    {
                        numeric = false;
                        break;
                    }
                }

                if (numeric)
                {
                    int numA = System.Convert.ToInt32(na.Substring(numStartA));
                    int numB = System.Convert.ToInt32(nb.Substring(numStartA));
                    return numA - numB;
                }
            }
        }

        return System.String.Compare(na, nb);
    }

	static bool CheckAndFixUpParams(tk2dSpriteCollection gen)
	{
		if (gen.textureRefs != null && gen.textureParams != null && gen.textureRefs.Length != gen.textureParams.Length)
        {
			tk2dSpriteCollectionDefinition[] newDefs = new tk2dSpriteCollectionDefinition[gen.textureRefs.Length];
			int c = Mathf.Min( newDefs.Length, gen.textureParams.Length );

			if (gen.textureRefs.Length > gen.textureParams.Length)
			{
				Texture2D[] newTexRefs = new Texture2D[gen.textureRefs.Length - gen.textureParams.Length];
				System.Array.Copy(gen.textureRefs, gen.textureParams.Length, newTexRefs, 0, newTexRefs.Length);
				System.Array.Sort(newTexRefs, (Texture2D a, Texture2D b) => NameCompare(a?a.name:"", b?b.name:""));
				System.Array.Copy(newTexRefs, 0, gen.textureRefs, gen.textureParams.Length, newTexRefs.Length);
			}

			for (int i = 0; i < c; ++i)
			{
				newDefs[i] = new tk2dSpriteCollectionDefinition();
				newDefs[i].CopyFrom( gen.textureParams[i] );
			}
			for (int i = c; i < newDefs.Length; ++i)
			{
				newDefs[i] = new tk2dSpriteCollectionDefinition();
			}
			gen.textureParams = newDefs;
        }

		// clear thumbnails on build
		foreach (var param in gen.textureParams)
		{
			param.thumbnailTexture = null;
		}

		foreach (var param in gen.textureParams)
		{
			if (gen.allowMultipleAtlases && param.dice)
			{
				EditorUtility.DisplayDialog("Error",
				                            "Multiple atlas spanning is not allowed when there are textures with dicing enabled in the SpriteCollection.",
							                "Ok");

				gen.allowMultipleAtlases = false;

				return false;
			}
		}

		return true;
	}

	static void SetUpSourceTextureFormats(tk2dSpriteCollection gen)
	{
		// make sure all textures are in the right format
		int numTexturesReimported = 0;
		List<Texture2D> texturesToProcess = new List<Texture2D>();

		for (int i = 0; i < gen.textureParams.Length; ++i)
		{
			if (gen.textureRefs[i] != null)
			{
				texturesToProcess.Add(gen.textureRefs[i]);
			}
		}
		if (gen.spriteSheets != null)
		{
			for (int i = 0; i < gen.spriteSheets.Length; ++i)
			{
				if (gen.spriteSheets[i].texture != null)
				{
					texturesToProcess.Add(gen.spriteSheets[i].texture);
				}
			}
		}
		foreach (var tex in texturesToProcess)
		{
			// make sure the source texture is npot and readable, and uncompressed
			string thisTextPath = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(thisTextPath);
            if (importer.textureType != TextureImporterType.Advanced ||
                importer.textureFormat != TextureImporterFormat.AutomaticTruecolor ||
                importer.npotScale != TextureImporterNPOTScale.None ||
                importer.isReadable != true ||
			    importer.maxTextureSize < 4096)
            {
                importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                importer.textureType = TextureImporterType.Advanced;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.isReadable = true;
				importer.mipmapEnabled = false;
				importer.maxTextureSize = 4096;

                AssetDatabase.ImportAsset(thisTextPath);

				numTexturesReimported++;
            }
		}
		if (numTexturesReimported > 0)
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

	static Texture2D ProcessTexture(bool premultipliedAlpha, bool additive, bool stretchPad, Texture2D srcTex, int sx, int sy, int tw, int th, ref SCGE.SpriteLut spriteLut, int padAmount)
	{
		// Can't have additive without premultiplied alpha
		if (!premultipliedAlpha) additive = false;

		int[] ww = new int[tw];
		int[] hh = new int[th];
		for (int x = 0; x < tw; ++x) ww[x] = 0;
		for (int y = 0; y < th; ++y) hh[y] = 0;
		int numNotTransparent = 0;
		for (int x = 0; x < tw; ++x)
		{
			for (int y = 0; y < th; ++y)
			{
				Color col = srcTex.GetPixel(sx + x, sy + y);
				if (col.a > 0)
				{
					ww[x] = 1;
					hh[y] = 1;
					numNotTransparent++;
				}
			}
		}

		if (numNotTransparent > 0)
		{
			int x0 = 0, x1 = 0, y0 = 0, y1 = 0;
			for (int x = 0; x < tw; ++x) if (ww[x] == 1) { x0 = x; break; }
			for (int x = tw - 1; x >= 0; --x) if (ww[x] == 1) { x1 = x; break; }
			for (int y = 0; y < th; ++y) if (hh[y] == 1) { y0 = y; break; }
			for (int y = th - 1; y >= 0; --y) if (hh[y] == 1) { y1 = y; break; }

			int w1 = x1 - x0 + 1;
			int h1 = y1 - y0 + 1;

			Texture2D dtex = new Texture2D(w1 + padAmount * 2, h1 + padAmount * 2);
			for (int x = 0; x < w1; ++x)
			{
				for (int y = 0; y < h1; ++y)
				{
					Color col = srcTex.GetPixel(sx + x0 + x, sy + y0 + y);
					dtex.SetPixel(x + padAmount, y + padAmount, col);
				}
			}
			PadTexture(dtex, padAmount, stretchPad);

			if (premultipliedAlpha)
			{
				for (int x = 0; x < dtex.width; ++x)
				{
					for (int y = 0; y < dtex.height; ++y)
					{
						Color col = dtex.GetPixel(x, y);
                        col.r *= col.a; col.g *= col.a; col.b *= col.a;
						col.a = additive?0.0f:col.a;
						dtex.SetPixel(x, y, col);
					}
				}
			}

			dtex.Apply();

			spriteLut.rx = sx + x0;
			spriteLut.ry = sy + y0;
			spriteLut.rw = w1;
			spriteLut.rh = h1;
			spriteLut.tex = dtex;

			return dtex;
		}
		else
		{
			return null;
		}
	}

	static void TrimTextureList(tk2dSpriteCollection gen)
	{
		// trim textureRefs & textureParams
		int lastNonEmpty = -1;
		for (int i = 0; i < gen.textureRefs.Length; ++i)
		{
			if (gen.textureRefs[i] != null) lastNonEmpty = i;
		}
		System.Array.Resize(ref gen.textureRefs, lastNonEmpty + 1);
		System.Array.Resize(ref gen.textureParams, lastNonEmpty + 1);
	}

	static bool SetUpSpriteSheets(tk2dSpriteCollection gen)
	{
		// delete textures which aren't in sprite sheets any more
		// and delete textures which are out of range of the spritesheet
		for (int i = 0; i < gen.textureRefs.Length; ++i)
		{
			if (gen.textureParams[i].fromSpriteSheet)
			{
				bool found = false;
				foreach (var ss in gen.spriteSheets)
				{
					if (gen.textureRefs[i] == ss.texture)
					{
						found = true;
						int numTiles = (ss.numTiles == 0)?(ss.tilesX * ss.tilesY):Mathf.Min(ss.numTiles, ss.tilesX * ss.tilesY);
						// delete textures which are out of range
						if (gen.textureParams[i].regionId >= numTiles)
						{
							gen.textureRefs[i] = null;
							gen.textureParams[i].fromSpriteSheet = false;
							gen.textureParams[i].extractRegion = false;
						}
					}
				}

				if (!found)
				{
					gen.textureRefs[i] = null;
					gen.textureParams[i].fromSpriteSheet = false;
					gen.textureParams[i].extractRegion = false;
				}
			}
		}

		if (gen.spriteSheets == null)
		{
			gen.spriteSheets = new tk2dSpriteSheetSource[0];
		}

		foreach (var ss in gen.spriteSheets)
		{
			// Sanity check
			if (ss.texture == null)
			{
				continue; // deleted, safely ignore this
			}
			if (ss.tilesX * ss.tilesY == 0 ||
			    (ss.numTiles != 0 && ss.numTiles > ss.tilesX * ss.tilesY))
			{
				EditorUtility.DisplayDialog("Invalid sprite sheet",
				                            "Sprite sheet '" + ss.texture.name + "' has an invalid number of tiles",
				                            "Ok");
				return false;
			}
			if ((ss.texture.width % ss.tilesX) != 0 || (ss.texture.height % ss.tilesY) != 0)
			{
				EditorUtility.DisplayDialog("Invalid sprite sheet",
				                            "Sprite sheet '" + ss.texture.name + "' doesn't match tile count",
				                            "Ok");
				return false;
			}

			int numTiles = (ss.numTiles == 0)?(ss.tilesX * ss.tilesY):Mathf.Min(ss.numTiles, ss.tilesX * ss.tilesY);

			for (int y = 0; y < ss.tilesY; ++y)
			{
				for (int x = 0; x < ss.tilesX; ++x)
				{
					// limit to number of tiles, if told to
					int tileIdx = y * ss.tilesX + x;
					if (tileIdx >= numTiles)
						break;

					// find texture in collection
					int textureIdx = -1;
					for (int i = 0; i < gen.textureParams.Length; ++i)
					{
						if (gen.textureParams[i].fromSpriteSheet
						    && gen.textureParams[i].regionId == tileIdx
						    && gen.textureRefs[i] == ss.texture)
						{
							textureIdx = i;
							break;
						}
					}

					if (textureIdx == -1)
					{
						// find first empty texture slot
						for (int i = 0; i < gen.textureParams.Length; ++i)
						{
							if (gen.textureRefs[i] == null)
							{
								textureIdx = i;
								break;
							}
						}
					}

					if (textureIdx == -1)
					{
						// texture not found, so extend arrays
						System.Array.Resize(ref gen.textureRefs, gen.textureRefs.Length + 1);
						System.Array.Resize(ref gen.textureParams, gen.textureParams.Length + 1);
						textureIdx = gen.textureRefs.Length - 1;
					}

					gen.textureRefs[textureIdx] = ss.texture;
					var param = new tk2dSpriteCollectionDefinition();
					param.fromSpriteSheet = true;
					param.name = ss.texture.name + "/" + tileIdx;
					param.regionId = tileIdx;
					param.regionW = ss.texture.width / ss.tilesX;
					param.regionH = ss.texture.height / ss.tilesY;
					param.regionX = (tileIdx % ss.tilesX) * param.regionW;
					param.regionY = (ss.tilesY - 1 - (tileIdx / ss.tilesX)) * param.regionH;
					param.extractRegion = true;

					param.pad = ss.pad;
					param.anchor = (tk2dSpriteCollectionDefinition.Anchor)ss.anchor;
					param.scale = (ss.scale.sqrMagnitude == 0.0f)?Vector3.one:ss.scale;

					gen.textureParams[textureIdx] = param;
				}
			}
		}

		return true;
	}

	static tk2dSpriteCollection currentBuild = null;
	static Texture2D[] sourceTextures;

    public static void Rebuild(tk2dSpriteCollection gen)
    {
		// avoid "recursive" build being triggered by texture watcher
		if (currentBuild == gen)
			return;

		currentBuild = gen;

        string path = AssetDatabase.GetAssetPath(gen);
		string subDirName = Path.GetDirectoryName( path.Substring(7) );
		if (subDirName.Length > 0) subDirName += "/";

		string dataDirFullPath = Application.dataPath + "/" + subDirName + Path.GetFileNameWithoutExtension(path) + "_Data";
		string dataDirName = "Assets/" + dataDirFullPath.Substring( Application.dataPath.Length + 1 ) + "/";

		if (gen.atlasTextures == null || gen.atlasTextures.Length == 0 ||
		    gen.atlasMaterials == null || gen.atlasMaterials.Length == 0 ||
		    gen.spriteCollection == null)
		{
			if (!Directory.Exists(dataDirFullPath)) Directory.CreateDirectory(dataDirFullPath);
			AssetDatabase.Refresh();
		}

		string prefabObjectPath = gen.spriteCollection?AssetDatabase.GetAssetPath(gen.spriteCollection):(dataDirName + "data.prefab");

      	if (!CheckAndFixUpParams(gen))
		{
			// Params failed check
			return;
		}

		SetUpSourceTextureFormats(gen);

		SetUpSpriteSheets(gen);

		TrimTextureList(gen);

		// blank texture used when texture has been deleted
		Texture2D blankTexture = new Texture2D(2, 2);
		blankTexture.SetPixel(0, 0, Color.magenta);
		blankTexture.SetPixel(0, 1, Color.yellow);
		blankTexture.SetPixel(1, 0, Color.cyan);
		blankTexture.SetPixel(1, 1, Color.grey);
		blankTexture.Apply();

		// make local texture sources
		sourceTextures = new Texture2D[gen.textureRefs.Length];
		for (int i = 0; i < gen.textureParams.Length; ++i)
		{
			var param = gen.textureParams[i];
			if (param.extractRegion && gen.textureRefs[i] != null)
			{
				Texture2D localTex = new Texture2D(param.regionW, param.regionH);
				for (int y = 0; y < param.regionH; ++y)
				{
					for (int x = 0; x < param.regionW; ++x)
					{
						localTex.SetPixel(x, y, gen.textureRefs[i].GetPixel(param.regionX + x, param.regionY + y));
					}
				}
				localTex.name = gen.textureRefs[i].name + "/" + param.regionId.ToString();
				localTex.Apply();
				sourceTextures[i] = localTex;
			}
			else
			{
				sourceTextures[i] = gen.textureRefs[i];
			}
		}

		// catalog all textures to atlas
		int numTexturesToAtlas = 0;
		List<SCGE.SpriteLut> spriteLuts = new List<SCGE.SpriteLut>();
		for (int i = 0; i < gen.textureParams.Length; ++i)
		{
			Texture2D currentTexture = sourceTextures[i];

			if (sourceTextures[i] == null)
			{
				gen.textureParams[i].dice = false;
				gen.textureParams[i].anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
				gen.textureParams[i].name = "";
				gen.textureParams[i].extractRegion = false;
				gen.textureParams[i].fromSpriteSheet = false;

				currentTexture = blankTexture;
			}
			else
			{
				if (gen.textureParams[i].name == null || gen.textureParams[i].name == "" || gen.textureParams[i].texture != currentTexture)
					gen.textureParams[i].name = currentTexture.name;
			}

			gen.textureParams[i].texture = currentTexture;


			if (gen.textureParams[i].dice)
			{
				// prepare to dice this up
				int diceUnitX = gen.textureParams[i].diceUnitX;
				int diceUnitY = gen.textureParams[i].diceUnitY;
				if (diceUnitX <= 0) diceUnitX = 128; // something sensible, please
				if (diceUnitY <= 0) diceUnitY = diceUnitX; // make square if not set

				Texture2D srcTex = currentTexture;
				for (int sx = 0; sx < srcTex.width; sx += diceUnitX)
				{
					for (int sy = 0; sy < srcTex.height; sy += diceUnitY)
					{
						int tw = Mathf.Min(diceUnitX, srcTex.width - sx);
						int th = Mathf.Min(diceUnitY, srcTex.height - sy);

						SCGE.SpriteLut diceLut = new SCGE.SpriteLut();
						diceLut.source = i;
						diceLut.isSplit = true;
						diceLut.sourceTex = srcTex;
						diceLut.isDuplicate = false; // duplicate diced textures can be chopped up differently, so don't detect dupes here

						Texture2D dest = ProcessTexture(gen.premultipliedAlpha, gen.textureParams[i].additive, true, srcTex, sx, sy, tw, th, ref diceLut, GetPadAmount(gen));
						if (dest)
						{
							diceLut.atlasIndex = numTexturesToAtlas++;
							spriteLuts.Add(diceLut);
						}
					}
				}
			}
			else
			{
				SCGE.SpriteLut lut = new SCGE.SpriteLut();
				lut.sourceTex = currentTexture;
				lut.source = i;

				lut.isSplit = false;
				lut.isDuplicate = false;
				for (int j = 0; j < spriteLuts.Count; ++j)
				{
					if (spriteLuts[j].sourceTex == lut.sourceTex)
					{
						lut.isDuplicate = true;
						lut.atlasIndex = spriteLuts[j].atlasIndex;
						lut.tex = spriteLuts[j].tex; // get old processed tex

						lut.rx = spriteLuts[j].rx; lut.ry = spriteLuts[j].ry;
						lut.rw = spriteLuts[j].rw; lut.rh = spriteLuts[j].rh;

						break;
					}
				}

				if (!lut.isDuplicate)
				{
					lut.atlasIndex = numTexturesToAtlas++;
					bool stretchPad = false;
					if (gen.textureParams[i].pad == tk2dSpriteCollectionDefinition.Pad.Extend) stretchPad = true;

					Texture2D dest = ProcessTexture(gen.premultipliedAlpha, gen.textureParams[i].additive, stretchPad, currentTexture, 0, 0, currentTexture.width, currentTexture.height, ref lut, GetPadAmount(gen));
					if (dest == null)
					{
						// fall back to a tiny blank texture
						lut.tex = new Texture2D(1, 1);
						lut.tex.SetPixel(0, 0, new Color( 0, 0, 0, 0 ));
						PadTexture(lut.tex, GetPadAmount(gen), stretchPad);
						lut.tex.Apply();

						lut.rx = currentTexture.width / 2; lut.ry = currentTexture.height / 2;
						lut.rw = 1; lut.rh = 1;
					}
				}

				spriteLuts.Add(lut);
			}
		}

        // Create texture
		Texture2D[] textureList = new Texture2D[numTexturesToAtlas];
        int titer = 0;
        for (int i = 0; i < spriteLuts.Count; ++i)
        {
			SCGE.SpriteLut _lut = spriteLuts[i];
			if (!_lut.isDuplicate)
			{
				textureList[titer++] = _lut.tex;
			}
        }
		
		// Build atlas
		Atlas.AtlasBuilder atlasBuilder = new Atlas.AtlasBuilder(gen.maxTextureSize, gen.maxTextureSize, gen.allowMultipleAtlases?64:1);
		if (textureList.Length > 0)
		{
			foreach (Texture2D currTexture in textureList)
			{
				atlasBuilder.AddRect(currTexture.width, currTexture.height);
			}
			if (atlasBuilder.Build() != 0)
			{
				if (atlasBuilder.HasOversizeTextures())
				{
					EditorUtility.DisplayDialog("Unable to fit in atlas",
					                            "You have a texture which exceeds the atlas size." +
					                            "Consider putting it in a separate atlas, enabling dicing, or" +
					                            "reducing the texture size",
								                "Ok");
				}
				else
				{
					EditorUtility.DisplayDialog("Unable to fit textures in requested atlas area",
					                            "There are too many textures in this collection for the requested " +
					                            "atlas size.",
								                "Ok");
				}
				return;
			}
		}
		
		// Fill atlas textures
		Atlas.AtlasData[] atlasData = atlasBuilder.GetAtlasData();
		System.Array.Resize(ref gen.atlasTextures, atlasData.Length);
		System.Array.Resize(ref gen.atlasMaterials, atlasData.Length);
		for (int atlasIndex = 0; atlasIndex < atlasData.Length; ++atlasIndex)
		{
	        Texture2D tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);

			gen.atlasWastage = (1.0f - atlasData[0].occupancy) * 100.0f;
			gen.atlasWidth = atlasData[0].width;
			gen.atlasHeight = atlasData[0].height;

			tex.Resize(atlasData[atlasIndex].width, atlasData[atlasIndex].height);

			// Clear texture, unsure if this is REALLY necessary
			for (int yy = 0; yy < tex.height; ++yy)
			{
				for (int xx = 0; xx < tex.width; ++xx)
				{
					tex.SetPixel(xx, yy, Color.black);
				}
			}

			for (int i = 0; i < atlasData[atlasIndex].entries.Length; ++i)
			{
				var entry = atlasData[atlasIndex].entries[i];
				Texture2D source = textureList[entry.index];

				if (!entry.flipped)
				{
					for (int y = 0; y < source.height; ++y)
					{
						for (int x = 0; x < source.width; ++x)
						{
							tex.SetPixel(entry.x + x, entry.y + y, source.GetPixel(x, y));
						}
					}
				}
				else
				{
					for (int y = 0; y < source.height; ++y)
					{
						for (int x = 0; x < source.width; ++x)
						{
							tex.SetPixel(entry.x + y, entry.y + x, source.GetPixel(x, y));
						}
					}
				}
			}

			tex.Apply();

			string texturePath = gen.atlasTextures[atlasIndex]?AssetDatabase.GetAssetPath(gen.atlasTextures[atlasIndex]):(dataDirName + "atlas" + atlasIndex + ".png");

			// Write filled atlas to disk
			byte[] bytes = tex.EncodeToPNG();
			System.IO.FileStream fs = System.IO.File.Create(texturePath);
			fs.Write(bytes, 0, bytes.Length);
			fs.Close();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			tex = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
			gen.atlasTextures[atlasIndex] = tex;

			// Make sure texture is set up with the correct max size and compression type
			SetUpTargetTexture(gen, tex);

	        // Create material if necessary
	        if (gen.atlasMaterials[atlasIndex] == null)
	        {
				Material mat;
	            if (gen.premultipliedAlpha)
	                mat = new Material(Shader.Find("tk2d/PremulVertexColor"));
	            else
	                mat = new Material(Shader.Find("tk2d/BlendVertexColor"));

				mat.mainTexture = tex;

				string materialPath = gen.atlasMaterials[atlasIndex]?AssetDatabase.GetAssetPath(gen.atlasMaterials[atlasIndex]):(dataDirName + "atlas" + atlasIndex + "_material.mat");
	            AssetDatabase.CreateAsset(mat, materialPath);
				AssetDatabase.SaveAssets();

				gen.atlasMaterials[atlasIndex] = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
			}
		}

        // Create prefab
		if (gen.spriteCollection == null)
		{
			Object p = EditorUtility.CreateEmptyPrefab(prefabObjectPath);
			GameObject go = new GameObject();
			go.AddComponent<tk2dSpriteCollectionData>();
			EditorUtility.ReplacePrefab(go, p);
			GameObject.DestroyImmediate(go);
			AssetDatabase.SaveAssets();

			gen.spriteCollection = AssetDatabase.LoadAssetAtPath(prefabObjectPath, typeof(tk2dSpriteCollectionData)) as tk2dSpriteCollectionData;
		}

		tk2dSpriteCollectionData coll = gen.spriteCollection;
		coll.textures = new Texture[gen.atlasTextures.Length];
		coll.materials = new Material[gen.atlasMaterials.Length];
		for (int i = 0; i < gen.atlasTextures.Length; ++i)
		{
			coll.textures[i] = gen.atlasTextures[i];
		}
        for (int i = 0; i < gen.atlasMaterials.Length; ++i)
		{
			coll.materials[i] = gen.atlasMaterials[i];
		}
		
		// Wipe out legacy data
		coll.material = null;
		
        coll.premultipliedAlpha = gen.premultipliedAlpha;
        coll.spriteDefinitions = new tk2dSpriteDefinition[gen.textureParams.Length];
		coll.version = tk2dSpriteCollectionData.CURRENT_VERSION;
		coll.spriteCollectionGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(gen));
		coll.spriteCollectionName = gen.name;
		for (int i = 0; i < coll.spriteDefinitions.Length; ++i)
		{
			coll.spriteDefinitions[i] = new tk2dSpriteDefinition();
			if (gen.textureRefs[i])
			{
				string assetPath = AssetDatabase.GetAssetPath(gen.textureRefs[i]);
				string guid = AssetDatabase.AssetPathToGUID(assetPath);
				coll.spriteDefinitions[i].sourceTextureGUID = guid;
			}
			else
			{
				coll.spriteDefinitions[i].sourceTextureGUID = "";
			}
			coll.spriteDefinitions[i].extractRegion = gen.textureParams[i].extractRegion;
			coll.spriteDefinitions[i].regionX = gen.textureParams[i].regionX;
			coll.spriteDefinitions[i].regionY = gen.textureParams[i].regionY;
			coll.spriteDefinitions[i].regionW = gen.textureParams[i].regionW;
			coll.spriteDefinitions[i].regionH = gen.textureParams[i].regionH;
		}
		coll.allowMultipleAtlases = gen.allowMultipleAtlases;
		UpdateVertexCache(gen, atlasData, coll, spriteLuts);

        // refresh existing
        tk2dSprite[] sprs = Resources.FindObjectsOfTypeAll(typeof(tk2dSprite)) as tk2dSprite[];
        foreach (tk2dSprite spr in sprs)
        {
			if (spr.collection == gen.spriteCollection)
			{
				if (spr.spriteId < 0 || spr.spriteId >= spr.collection.spriteDefinitions.Length)
            		spr.spriteId = 0;

				spr.Build();
			}
        }
		tk2dStaticSpriteBatcher[] batchedSprs = Resources.FindObjectsOfTypeAll(typeof(tk2dStaticSpriteBatcher)) as tk2dStaticSpriteBatcher[];
		foreach (var spr in batchedSprs)
		{
			if (spr.spriteCollection == gen.spriteCollection)
			{
				spr.Build();
			}
		}

		// save changes
		EditorUtility.SetDirty(gen.spriteCollection);
		EditorUtility.SetDirty(gen);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		sourceTextures = null; // need to clear, its static
		currentBuild = null;
		
		tk2dEditorUtility.GetOrCreateIndex().AddSpriteCollectionData(gen.spriteCollection);
		tk2dEditorUtility.CommitIndex();		
    }

	static void SetUpTargetTexture(tk2dSpriteCollection gen, Texture2D tex)
	{
		bool textureDirty = false;

		string targetTexPath = AssetDatabase.GetAssetPath(tex);
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(targetTexPath);
		if (gen.maxTextureSize != importer.maxTextureSize)
		{
			importer.maxTextureSize = gen.maxTextureSize;
			textureDirty = true;
		}
		TextureImporterFormat targetFormat;
		switch (gen.textureCompression)
		{
		case tk2dSpriteCollection.TextureCompression.Uncompressed: targetFormat = TextureImporterFormat.AutomaticTruecolor; break;
		case tk2dSpriteCollection.TextureCompression.Reduced16Bit: targetFormat = TextureImporterFormat.Automatic16bit; break;
		case tk2dSpriteCollection.TextureCompression.Compressed: targetFormat = TextureImporterFormat.AutomaticCompressed; break;

		default: targetFormat = TextureImporterFormat.AutomaticTruecolor; break;
		}

		if (targetFormat != importer.textureFormat)
		{
			importer.textureFormat = targetFormat;
			textureDirty = true;
		}

		// Make sure texture is set to point filtered when no pad mode is selected
		FilterMode targetFilterMode = gen.pixelPerfectPointSampled?FilterMode.Point:FilterMode.Bilinear;
		if (tex.filterMode != targetFilterMode)
		{
			importer.filterMode = targetFilterMode;
			EditorUtility.SetDirty(importer);
			textureDirty = true;
		}


		if (textureDirty)
		{
			EditorUtility.SetDirty(importer);
			AssetDatabase.ImportAsset(targetTexPath);
		}
	}

    static void UpdateVertexCache(tk2dSpriteCollection gen, Atlas.AtlasData[] packers, tk2dSpriteCollectionData coll, List<SCGE.SpriteLut> spriteLuts)
    {
        float scale = 2.0f * gen.targetOrthoSize / gen.targetHeight;
		int padAmount = GetPadAmount(gen);

        for (int i = 0; i < sourceTextures.Length; ++i)
        {
			SCGE.SpriteLut _lut = null;
			for (int j = 0; j < spriteLuts.Count; ++j)
			{
				if (spriteLuts[j].source == i)
				{
					_lut = spriteLuts[j];
					break;
				}
			}

            tk2dSpriteCollectionDefinition thisTexParam = gen.textureParams[i];
			Atlas.AtlasData packer = null;
			Atlas.AtlasEntry atlasEntry = null;
			int atlasIndex = 0;
			foreach (var p in packers)
			{
				if ((atlasEntry = p.FindEntryWithIndex(_lut.atlasIndex)) != null)
				{
					packer = p;
					break;
				}
				++atlasIndex;
			}
			float fwidth = packer.width;
    	    float fheight = packer.height;

            int tx = atlasEntry.x + padAmount, ty = atlasEntry.y + padAmount, tw = atlasEntry.w - padAmount * 2, th = atlasEntry.h - padAmount * 2;
            int sd_y = packer.height - ty - th;

			float uvOffsetX = 0.001f / fwidth;
			float uvOffsetY = 0.001f / fheight;

            Vector2 v0 = new Vector2(tx / fwidth + uvOffsetX, 1.0f - (sd_y + th) / fheight + uvOffsetY);
            Vector2 v1 = new Vector2((tx + tw) / fwidth - uvOffsetX, 1.0f - sd_y / fheight - uvOffsetY);

            Mesh mesh = null;
            Transform meshTransform = null;
            GameObject instantiated = null;

            if (thisTexParam.overrideMesh)
            {
				// Disabled
                instantiated = GameObject.Instantiate(thisTexParam.overrideMesh) as GameObject;
                MeshFilter meshFilter = instantiated.GetComponentInChildren<MeshFilter>();
                if (meshFilter == null)
                {
                    Debug.LogError("Unable to find mesh");
                    GameObject.DestroyImmediate(instantiated);
                }
                else
                {
                    mesh = meshFilter.sharedMesh;
                    meshTransform = meshFilter.gameObject.transform;
                }
            }

            if (mesh)
            {
                coll.spriteDefinitions[i].positions = new Vector3[mesh.vertices.Length];
                coll.spriteDefinitions[i].uvs = new Vector2[mesh.vertices.Length];
                for (int j = 0; j < mesh.vertices.Length; ++j)
                {
                    coll.spriteDefinitions[i].positions[j] = meshTransform.TransformPoint(mesh.vertices[j]);
                    coll.spriteDefinitions[i].uvs[j] = new Vector2(v0.x + (v1.x - v0.x) * mesh.uv[j].x, v0.y + (v1.y - v0.y) * mesh.uv[j].y);
                }
                coll.spriteDefinitions[i].indices = new int[mesh.triangles.Length];
                for (int j = 0; j < mesh.triangles.Length; ++j)
                {
                    coll.spriteDefinitions[i].indices[j] = mesh.triangles[j];
                }
				coll.spriteDefinitions[i].material = gen.atlasMaterials[atlasIndex];

                GameObject.DestroyImmediate(instantiated);
            }
            else
            {
				Texture2D thisTextureRef = sourceTextures[i];

				float texHeight = thisTextureRef?thisTextureRef.height:2;
       			float texWidth = thisTextureRef?thisTextureRef.width:2;

				float h = thisTextureRef?thisTextureRef.height:64;
				float w = thisTextureRef?thisTextureRef.width:64;
                h *= thisTexParam.scale.x;
                w *= thisTexParam.scale.y;

				float scaleX = w * scale;
                float scaleY = h * scale;

                Vector3 pos0 = new Vector3(-0.5f * scaleX, 0, -0.5f * scaleY);
                switch (thisTexParam.anchor)
                {
                    case tk2dSpriteCollectionDefinition.Anchor.LowerLeft: pos0 = new Vector3(0, 0, 0); break;
                    case tk2dSpriteCollectionDefinition.Anchor.LowerCenter: pos0 = new Vector3(-0.5f * scaleX, 0, 0); break;
                    case tk2dSpriteCollectionDefinition.Anchor.LowerRight: pos0 = new Vector3(-scaleX, 0, 0); break;

                    case tk2dSpriteCollectionDefinition.Anchor.MiddleLeft: pos0 = new Vector3(0, 0, -0.5f * scaleY); break;
                    case tk2dSpriteCollectionDefinition.Anchor.MiddleCenter: pos0 = new Vector3(-0.5f * scaleX, 0, -0.5f * scaleY); break;
                    case tk2dSpriteCollectionDefinition.Anchor.MiddleRight: pos0 = new Vector3(-scaleX, 0, -0.5f * scaleY); break;

                    case tk2dSpriteCollectionDefinition.Anchor.UpperLeft: pos0 = new Vector3(0, 0, -scaleY); break;
                    case tk2dSpriteCollectionDefinition.Anchor.UpperCenter: pos0 = new Vector3(-0.5f * scaleX, 0, -scaleY); break;
                    case tk2dSpriteCollectionDefinition.Anchor.UpperRight: pos0 = new Vector3(-scaleX, 0, -scaleY); break;

                    case tk2dSpriteCollectionDefinition.Anchor.Custom:
                        {
                            pos0 = new Vector3(-thisTexParam.anchorX * thisTexParam.scale.x * scale, 0, -(h - thisTexParam.anchorY * thisTexParam.scale.y) * scale);
                        }
                        break;
                }

                Vector3 pos1 = pos0 + new Vector3(scaleX, 0, scaleY);

				List<Vector3> positions = new List<Vector3>();
				List<Vector2> uvs = new List<Vector2>();

				// build mesh
				if (_lut.isSplit)
				{
					for (int j = 0; j < spriteLuts.Count; ++j)
					{
						if (spriteLuts[j].source == i)
						{
							_lut = spriteLuts[j];

							int thisAtlasIndex = 0;
							foreach (var p in packers)
							{
								if ((atlasEntry = p.FindEntryWithIndex(_lut.atlasIndex)) != null)
								{
									packer = p;
									break;
								}
								++thisAtlasIndex;
							}

							if (thisAtlasIndex != atlasIndex)
							{
								// This is a serious problem, dicing is not supported when multi atlas output is selected
								Debug.Break();
							}

							fwidth = packer.width;
				    	    fheight = packer.height;

				            tx = atlasEntry.x + padAmount;
							ty = atlasEntry.y + padAmount;
							tw = atlasEntry.w - padAmount * 2;
							th = atlasEntry.h - padAmount * 2;

				            sd_y = packer.height - ty - th;
				            v0 = new Vector2(tx / fwidth + uvOffsetX, 1.0f - (sd_y + th) / fheight + uvOffsetY);
				            v1 = new Vector2((tx + tw) / fwidth - uvOffsetX, 1.0f - sd_y / fheight - uvOffsetY);

							float x0 = _lut.rx / texWidth;
							float y0 = _lut.ry / texHeight;
							float x1 = (_lut.rx + _lut.rw) / texWidth;
							float y1 = (_lut.ry + _lut.rh) / texHeight;

							Vector3 dpos0 = new Vector3(Mathf.Lerp(pos0.x, pos1.x, x0), 0.0f, Mathf.Lerp(pos0.z, pos1.z, y0));
							Vector3 dpos1 = new Vector3(Mathf.Lerp(pos0.x, pos1.x, x1), 0.0f, Mathf.Lerp(pos0.z, pos1.z, y1));

							positions.Add(new Vector3(dpos0.x, dpos0.z, 0));
							positions.Add(new Vector3(dpos1.x, dpos0.z, 0));
							positions.Add(new Vector3(dpos0.x, dpos1.z, 0));
							positions.Add(new Vector3(dpos1.x, dpos1.z, 0));

			                if (atlasEntry.flipped)
			                {
			                    uvs.Add(new Vector2(v0.x,v0.y));
			                    uvs.Add(new Vector2(v0.x,v1.y));
			                    uvs.Add(new Vector2(v1.x,v0.y));
			                    uvs.Add(new Vector2(v1.x,v1.y));
			                }
			                else
			                {
			                    uvs.Add(new Vector2(v0.x,v0.y));
			                    uvs.Add(new Vector2(v1.x,v0.y));
			                    uvs.Add(new Vector2(v0.x,v1.y));
			                    uvs.Add(new Vector2(v1.x,v1.y));
			                }
						}
					}
				}
				else
				{
					float x0 = _lut.rx / texWidth;
					float y0 = _lut.ry / texHeight;
					float x1 = (_lut.rx + _lut.rw) / texWidth;
					float y1 = (_lut.ry + _lut.rh) / texHeight;

					Vector3 dpos0 = new Vector3(Mathf.Lerp(pos0.x, pos1.x, x0), 0.0f, Mathf.Lerp(pos0.z, pos1.z, y0));
					Vector3 dpos1 = new Vector3(Mathf.Lerp(pos0.x, pos1.x, x1), 0.0f, Mathf.Lerp(pos0.z, pos1.z, y1));

					positions.Add(new Vector3(dpos0.x, dpos0.z, 0));
					positions.Add(new Vector3(dpos1.x, dpos0.z, 0));
					positions.Add(new Vector3(dpos0.x, dpos1.z, 0));
					positions.Add(new Vector3(dpos1.x, dpos1.z, 0));

	                if (atlasEntry.flipped)
	                {
	                    uvs.Add(new Vector2(v0.x,v0.y));
	                    uvs.Add(new Vector2(v0.x,v1.y));
	                    uvs.Add(new Vector2(v1.x,v0.y));
	                    uvs.Add(new Vector2(v1.x,v1.y));
	                }
	                else
	                {
	                    uvs.Add(new Vector2(v0.x,v0.y));
	                    uvs.Add(new Vector2(v1.x,v0.y));
	                    uvs.Add(new Vector2(v0.x,v1.y));
	                    uvs.Add(new Vector2(v1.x,v1.y));
	                }
				}

				// build sprite definition
				coll.spriteDefinitions[i].indices = new int[ 6 * (positions.Count / 4) ];
				for (int j = 0; j < positions.Count / 4; ++j)
				{
	                coll.spriteDefinitions[i].indices[j * 6 + 0] = j * 4 + 0;
					coll.spriteDefinitions[i].indices[j * 6 + 1] = j * 4 + 3;
					coll.spriteDefinitions[i].indices[j * 6 + 2] = j * 4 + 1;
					coll.spriteDefinitions[i].indices[j * 6 + 3] = j * 4 + 2;
					coll.spriteDefinitions[i].indices[j * 6 + 4] = j * 4 + 3;
					coll.spriteDefinitions[i].indices[j * 6 + 5] = j * 4 + 0;
				}

				coll.spriteDefinitions[i].positions = new Vector3[positions.Count];
				coll.spriteDefinitions[i].uvs = new Vector2[uvs.Count];
				for (int j = 0; j < positions.Count; ++j)
				{
					coll.spriteDefinitions[i].positions[j] = positions[j];
					coll.spriteDefinitions[i].uvs[j] = uvs[j];
				}

				coll.spriteDefinitions[i].material = gen.atlasMaterials[atlasIndex];
			}

            Vector3 boundsMin = new Vector3(1.0e32f, 1.0e32f, 1.0e32f);
            Vector3 boundsMax = new Vector3(-1.0e32f, -1.0e32f, -1.0e32f);
            foreach (Vector3 v in coll.spriteDefinitions[i].positions)
            {
                boundsMin = Vector3.Min(boundsMin, v);
                boundsMax = Vector3.Max(boundsMax, v);
            }

			coll.spriteDefinitions[i].boundsData = new Vector3[2];
			coll.spriteDefinitions[i].boundsData[0] = (boundsMax + boundsMin) / 2.0f;
			coll.spriteDefinitions[i].boundsData[1] = (boundsMax - boundsMin);
			coll.spriteDefinitions[i].name = gen.textureParams[i].name;
        }
    }

}
