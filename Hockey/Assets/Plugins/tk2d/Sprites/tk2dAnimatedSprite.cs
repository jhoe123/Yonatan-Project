using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/tk2dAnimatedSprite")]
public class tk2dAnimatedSprite : tk2dSprite
{
	public tk2dSpriteAnimation anim;
	public int clipId = 0;
	public bool playAutomatically = false;
	
	public static bool g_paused = false;
	public bool paused = false;
	
	public bool createCollider = false;

	tk2dSpriteAnimationClip currentClip = null;
    float clipTime = 0.0f;
	int previousFrame = -1;
	
	public delegate void AnimationCompleteDelegate(tk2dAnimatedSprite sprite, int clipId);
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	public delegate void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum);
	public AnimationEventDelegate animationEventDelegate;
	
	new void Start()
	{
		base.Start();
		
		if (playAutomatically)
			Play(clipId);
	}
	
	public void Play()
	{
		Play(clipId);
	}
	
	public void Play(float clipStartTime)
	{
		Play(clipId, clipStartTime);
	}
	
	public void Play(string name)
	{
		int id = anim?anim.GetClipIdByName(name):-1;
		Play(id);
	}
	
	public void Play(string name, float clipStartTime)
	{
		int id = anim?anim.GetClipIdByName(name):-1;
		Play(id, clipStartTime);
	}
	
	public tk2dSpriteAnimationClip CurrentClip
	{
		get { return currentClip; }
	}
	
	public void Stop()
	{
		currentClip = null;
	}
	
	public bool isPlaying()
	{
		return currentClip != null;
	}
	
	protected override bool NeedBoxCollider()
	{
		return createCollider;
	}
	
	public int GetClipIdByName(string name)
	{
		return anim?anim.GetClipIdByName(name):-1;
	}
	
	public void Play(int id)
	{
		Play(id, 0.0f);
	}
	
	public void Play(int id, float clipStartTime)
	{
		clipId = id;
		if (id >= 0 && anim && id < anim.clips.Length)
		{
			currentClip = anim.clips[id];

			// Simply swap, no animation is played
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || currentClip.frames == null)
			{
				SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
				
				if (currentClip.frames[0].triggerEvent)
				{
					if (animationEventDelegate != null)
						animationEventDelegate( this, currentClip, currentClip.frames[0], 0 );
				}
				currentClip = null;
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int rnd = Random.Range(0, currentClip.frames.Length - 1);
				var currentFrame = currentClip.frames[rnd];
				clipTime = rnd * currentClip.fps;
				
				SwitchCollectionAndSprite(currentFrame.spriteCollection, currentFrame.spriteId);
				if (currentFrame.triggerEvent)
				{
					if (animationEventDelegate != null)
						animationEventDelegate( this, currentClip, currentFrame, 0 );
				}
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame)
				{
					currentClip = null;
					previousFrame = -1;
				}
			}
			else
			{
				clipTime = clipStartTime;
				previousFrame = -1;
				
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && clipTime >= currentClip.fps * currentClip.frames.Length)
				{
					// force to the last frame
					clipTime = currentClip.fps * (currentClip.frames.Length - 0.1f);
				}
			}
		}
		else
		{
			OnCompleteAnimation();
			currentClip = null;
		}
	}
	
	public void Pause()
	{
		paused = true;
	}
	
	public void Resume()
	{
		paused = false;
	}
	
	void OnCompleteAnimation()
	{
		previousFrame = -1;
		if (animationCompleteDelegate != null)
			animationCompleteDelegate(this, clipId);
	}
	
	void SetFrame(int currFrame)
	{
		if (previousFrame != currFrame)
		{
			SwitchCollectionAndSprite( currentClip.frames[currFrame].spriteCollection, currentClip.frames[currFrame].spriteId );
			if (currentClip.frames[currFrame].triggerEvent)
			{
				if (animationEventDelegate != null)
					animationEventDelegate( this, currentClip, currentClip.frames[currFrame], currFrame );
			}
			previousFrame = currFrame;
		}
	}
	
	void Update () 
	{
#if UNITY_EDITOR
		// Don't play animations when not in play mode
		if (!Application.isPlaying)
			return;
#endif
		
		if (g_paused || paused)
			return;
		
		if (currentClip != null && currentClip.frames != null)
		{
			clipTime += Time.deltaTime * currentClip.fps;
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int currFrame = (int)clipTime % currentClip.frames.Length;
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection)
			{
				int currFrame = (int)clipTime;
				if (currFrame >= currentClip.loopStart)
				{
					currFrame = currentClip.loopStart + ((currFrame - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart));
				}
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.PingPong)
			{
				int currFrame = (int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2);
				if (currFrame >= currentClip.frames.Length)
				{
					int i = currFrame - currentClip.frames.Length;
					currFrame = currentClip.frames.Length - 2 - i;
				}
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
			{
				int currFrame = (int)clipTime;
				if (currFrame >= currentClip.frames.Length)
				{
					currentClip = null;
					OnCompleteAnimation();
				}
				else
				{
					SetFrame(currFrame);
				}
				
			}
		}
	}
}
