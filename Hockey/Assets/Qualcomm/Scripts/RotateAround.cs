/*==============================================================================
            Copyright (c) 2010-2011 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

// A simple behaviour to rotate the owning game object.
public class RotateAround : MonoBehaviour
{
    #region UNTIY_MONOBEHAVIOUR_METHODS

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Transform parentTransform = transform.parent;
		transform.RotateAround(parentTransform.position, parentTransform.forward, -60 * Time.deltaTime);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS
}
