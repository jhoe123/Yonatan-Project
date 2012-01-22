using UnityEngine;
using System.Collections;

public class GUIProgressbar_Unity : GUITexture {

	public float mInitialMaxValue = 100;				//the initial max value. the max value can be access by maxValue property
	protected float mMaxValue;							//the max value
	protected float mCurrentValue;
	protected float mMaxWidth = 0;
	
	protected override void Start ()
	{
		mMaxValue = mInitialMaxValue;
		mCurrentValue = mInitialMaxValue;
		base.Start ();
	}
	
	public float maxValue
	{
		get{ return mMaxValue;}
		set
		{
			mMaxValue = value;
			SetupRegion();
			Realign();
		}
	}
	
	public float currentValue
	{
		get{ return mCurrentValue; }
		set
		{
			mCurrentValue = value;
			Realign();
		}
	}
	
	
	public void Realign( Rect pRect )
	{
		mRect = pRect;
		Realign();
	}
	
	//use to realign the progress bar
	public void Realign()
	{
		mRect.width = (mMaxWidth * mCurrentValue)/mMaxValue;	
	}
	
	public override void SetupRegion ()
	{
		base.SetupRegion ();
		if( mMaxWidth <= 0)
			mMaxWidth = mRect.width;
		Realign();
	}
}
