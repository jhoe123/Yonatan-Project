using UnityEngine;
using System.Collections;

/*
 * Use to animate the material color
 * */
[RequireComponent( typeof(Renderer) )]
public class MaterialColorAnimator : Effect {
	
	public string mColorName = "_Color";							//the color name on the material to be animated
	public Color mStartColor =  Color.white;						//the start color to be animate
	public Color mEndColor = Color.black;							//the end color to animate
	public bool mIsPingpongLoop = true;								//this creates a smooth loop
	Material mMat;															
	float mElapsed;
	
	//to be called when this script starts
	protected override void Start ()
	{
		base.Start ();
		mMat = renderer.material;
	}
	
	//to be called when this will be starting
	public override bool Play ()
	{
		if( base.Play () )
		{	
			GameController.mUpdateRealTimeDelegate += OnUpdate;
			mElapsed = Time.realtimeSinceStartup;
			return true;
		}else
			return false;
	}
	
	//to be called when this effect will be repeated
	//@param: the total repeatation
	protected override void OnRepeat (int pRepeatCount)
	{
		base.OnRepeat (pRepeatCount);
		if( mIsPingpongLoop )
		{
			Color tmpColor = mStartColor;
			mStartColor = mEndColor;
			mEndColor = tmpColor;
		}
	}
	
	//to stop the effect
	public override void Stop ()
	{
		base.Stop ();
		GameController.mUpdateRealTimeDelegate -= OnUpdate;
	}
	
	protected Color mCurrentColor;
	protected virtual void OnUpdate( float pCurrentTime )
	{
		float time = ( pCurrentTime - mElapsed )/ mStopTime; 
		mCurrentColor = Color.Lerp( mStartColor, mEndColor, time);
		mMat.SetColor( mColorName, mCurrentColor);
	}
}
