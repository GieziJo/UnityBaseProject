// ===============================
// Original author : J. Giezendanner
// SPECIAL NOTES   : 
// ===============================
// Summary         : Plays a clip from animation
// ===============================
// Change History:
// J.Giezendanner: 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFromAnimation : MonoBehaviour
{
	public void playFromAnimation(AudioClipName audioClipName){
		AudioManager.sm.Play(audioClipName);
	}
}
