using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFromAnimation : MonoBehaviour
{
	public void playFromAnimation(AudioClipName audioClipName){
		AudioManager.sm.Play(audioClipName);
	}
}
