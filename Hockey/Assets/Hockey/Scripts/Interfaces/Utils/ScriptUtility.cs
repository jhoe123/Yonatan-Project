using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;

public class ScriptUtility{
	
	//use to get the delegate from the methodname
	//@param 1: the method name
	//@param 2: the type of delegate to get
	//@param 3: the object instance that has the method to search
	//@return: return the delegate if found on the given target object while null if not found
	public static System.Delegate GetDelegateFromMethod( string pMethodName, System.Type pDelegateType, object pTargetClass)
	{
		return System.Delegate.CreateDelegate( pDelegateType, pTargetClass, pMethodName, false, false);	
	}
}
