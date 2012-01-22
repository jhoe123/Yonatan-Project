using System.Collections.Generic;
using UnityEngine;

public class tk2dIndex : MonoBehaviour
{
	[SerializeField] List<tk2dSpriteCollectionData> spriteCollectionData = new List<tk2dSpriteCollectionData>();
	[SerializeField] List<tk2dSpriteAnimation> spriteAnimations = new List<tk2dSpriteAnimation>();
	[SerializeField] List<tk2dFont> fonts = new List<tk2dFont>();
	
	public tk2dSpriteCollectionData[] GetSpriteCollectionData()
	{
		spriteCollectionData.RemoveAll(item => item == null);
		return spriteCollectionData.ToArray();
	}
	
	public void AddSpriteCollectionData(tk2dSpriteCollectionData sc)
	{
		spriteCollectionData.RemoveAll(item => item == null);
		foreach (var v in spriteCollectionData) if (v == sc) return;
		spriteCollectionData.Add(sc);
	}

	public tk2dSpriteAnimation[] GetSpriteAnimations()
	{
		spriteAnimations.RemoveAll(item => item == null);
		return spriteAnimations.ToArray();
	}
	
	public void AddSpriteAnimation(tk2dSpriteAnimation sc)
	{
		spriteAnimations.RemoveAll(item => item == null);
		foreach (var v in spriteAnimations) if (v == sc) return;
		spriteAnimations.Add(sc);
	}

	public tk2dFont[] GetFonts()
	{
		fonts.RemoveAll(item => item == null);
		return fonts.ToArray();
	}
	
	public void AddFont(tk2dFont sc)
	{
		fonts.RemoveAll(item => item == null);
		foreach (var v in fonts) if (v == sc) return;
		fonts.Add(sc);
	}
}

