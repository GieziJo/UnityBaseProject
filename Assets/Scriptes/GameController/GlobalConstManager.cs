// ===============================
// Original author : J. Giezendanner
// SPECIAL NOTES   : 
// ===============================
// Summary         : Constants manager, regroup constants for easy manipulation
// ===============================
// Change History:
// J.Giezendanner: 
//==================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstManager : MonoBehaviour
{
	public static GlobalConstManager gs;

	public GlobalConstSO globalConst;

	// Use this for initialization
	void Awake()
	{
		gs = this;
	}

	public void SetActiveGlobalVar(GlobalConstSO activeGlobalConst){
		this.globalConst = activeGlobalConst;
	}
}
