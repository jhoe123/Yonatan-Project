using UnityEngine;
using System.Collections;

public delegate void UpdateDelegate( float pCurrentTime );

///////////////////////////CONTROLLER DELEGATES/////////////////////////////
public delegate void SceneLoadDelegate( Scene pScene, float pLoadTime);

//////////////////////////INPUT DELEGATES//////////////////////////////
public delegate void MouseMoveDelegate( Vector3 pPos);
public delegate void MouseCallbackDelegate( InputType pType, int pMouseindex, bool pIsPressed, Vector3 pMousePosition);
public delegate void KeyboardCallbackDelegate( KeyCode pKeyCode);
public delegate void MouseScrollDelegate( float mSpeed);
public delegate void AcceleratorDelegate( Vector3 pCurrentAcceleration );

/////////////////////////GUI DELEGATES////////////////////////////////
public delegate void GUIDelegate( eStateList pList, GUIObject pGUI);

/////////////////////////ACTOR DELEGATE///////////////////////////////
public delegate void SetStateDelegate( eStateList pState, Object[] pParams);

/// ////////////////////EFFECT DELEGATE//////////////////////////////////
public delegate void LerpAnimatorDelegate( LerpAnimator pAnimator);
