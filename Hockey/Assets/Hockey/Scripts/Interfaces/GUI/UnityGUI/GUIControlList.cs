using UnityEngine;
using System.Collections;

/*
 * WZIG Implementation that renders the controls as list view.
 * Note. But this is slow due to mesh setups etc. For performance use GUIListRenderer instead.
 * */
public class GUIControlList : GUIScrollView {
	
	public IGUIUnity mLastAdded;					//the last added control
	IGUIUnity mTmp;
	
	//add a control on the view
	//@param 1: control to be instantiated. can pass the mLastAdded
	//@param 2: the alignment
	public IGUIUnity AddControl( IGUIUnity pType, Vector3 pAlignment )
	{
		IGUIUnity newC = (GameObject.Instantiate( pType.gameObject, pType.transform.position + pAlignment, pType.transform.rotation ) as GameObject).GetComponent<IGUIUnity>();
		newC.transform.parent = transform;
		newC.transform.localScale = pType.transform.localScale;
		StartCoroutine( "InternalCall", newC);
		return newC;
	}

	IEnumerator InternalCall( IGUIUnity pToBeAdded)
	{
		yield return new WaitForSeconds (0.02f);
		AddChild( pToBeAdded);
	}
	
	public override void AddChild (GUIObject pObject)
	{
		base.AddChild (pObject);
		if( pObject as IGUIUnity != null )
			mLastAdded = pObject as IGUIUnity;
	}
	
}
