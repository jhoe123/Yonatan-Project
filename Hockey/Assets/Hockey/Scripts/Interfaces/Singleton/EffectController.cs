using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {
	public static EffectController mCurrent;
	static GameObject mThisObject;
	
	void Awake()
	{
		mCurrent = this;
		mThisObject = gameObject;
	}
	
	/// <summary>
	/// Lerp the specified pStart, pTarget, pAnimTime and pDelegate.
	/// </summary>
	/// <param name='pStart'>
	/// start position
	/// </param>
	/// <param name='pTarget'>
	/// target position
	/// </param>
	/// <param name='pAnimTime'>
	/// the animation time
	/// </param>
	/// <param name='pDelegate'>
	/// callback on end
	/// </param>
	public static LerpAnimator Lerp( Transform pTransform, Vector3 pStart, Vector3 pTarget, float pAnimTime, LerpAnimatorDelegate pDelegate )
	{
		LerpAnimator animator = mThisObject.AddComponent( typeof(LerpAnimator) ) as LerpAnimator;
		animator.mTransform = pTransform;
		animator.mOnEndCallback += pDelegate;
		animator.mStartPos = pStart;
		animator.mTargetPos = pTarget;
		animator.mAnimateTime = pAnimTime;
		return animator;
	}
}
