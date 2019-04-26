// ===============================
// Original author : J. Giezendanner
// SPECIAL NOTES   : 
// ===============================
// Summary         : Global variable manager
// ===============================
// Change History:
// J.Giezendanner: 
//==================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVarManager : MonoBehaviour
{
	public static GlobalVarManager gs;

	public GlobalVarSO activeGlobalVar;

	// Use this for initialization
	void Awake()
	{
		if (gs == null)
		{
			DontDestroyOnLoad(this.gameObject);
			gs = this;
		}
		else if (gs != this)
			Destroy(gameObject);
	}

	public void SetActiveGlobalVar(GlobalVarSO activeGlobalVar){
		this.activeGlobalVar = activeGlobalVar;
	}
}
