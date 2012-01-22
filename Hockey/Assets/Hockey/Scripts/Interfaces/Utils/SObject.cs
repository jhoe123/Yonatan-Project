using UnityEngine;
using System.Collections;

public class SObject : MonoBehaviour {

	protected eStateList mState;
	
	protected virtual void Awake()
	{}
	
	protected virtual void Start()
	{}
	
	//to be called when uninit
	protected virtual void OnUnInit()
	{}
	
	public static void DeleteObject( MonoBehaviour pBehaviour )
	{
		Destroy( pBehaviour);
	}
	
	//use to remove this object to the memory
	public static void DeleteObject( SObject pObject )
	{
		pObject.OnUnInit();
		pObject = null;
		Destroy( pObject ); 
	}
	
	//remove the object onto the memory
	//@param: the object to be remove tot the memory
	public static void DeleteObject( GameObject pObject )
	{
		Component[] components = pObject.GetComponentsInChildren<Component>();
		int l = components.Length;
		for( int i=0; i<l; i++)
		{
			if( components[i] as SObject != null )
				DeleteObject( components[i] as SObject);
		}
		Destroy( pObject);
	}
	
	public eStateList GetState()
	{
		return mState;
	}
	
	public virtual void OnUpdate( float mCurrentTime )
	{
		
	}
}
