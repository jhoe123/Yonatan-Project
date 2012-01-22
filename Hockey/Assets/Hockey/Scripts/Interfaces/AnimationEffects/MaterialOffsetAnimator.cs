using UnityEngine;
using System.Collections;

public class MaterialOffsetAnimator : Effect {
	
	public Vector2 mIncrementation = new Vector2( 0, 1);
	Material mMat;
	Vector2 mOffset;
	
	protected override void Start ()
	{
		base.Start ();
		mMat = renderer.material;
		mOffset = mMat.mainTextureOffset;
	}
	
	public override bool Play ()
	{
		if( base.Play ())
		{
			GameController.mUpdateRealTimeDelegate += OnUpdate;
			return true;
		}
		else
			return false;
	}
	
	public override void Stop ()
	{
		base.Stop ();
		GameController.mUpdateRealTimeDelegate -= OnUpdate;
	}
	
	void OnUpdate( float pTime )
	{
		mOffset += mIncrementation;
		mMat.mainTextureOffset = mOffset;
	}
}
