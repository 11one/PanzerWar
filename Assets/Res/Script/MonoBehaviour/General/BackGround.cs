using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BackGround : MonoBehaviour {
	public static  void PlaySound(string Voice,int DeadTime){
		GameObject VoiceGameObject = new GameObject (Voice);
		AudioSource au = VoiceGameObject.AddComponent<AudioSource> ();
		au.spread = 360;
		au.volume =1;
		au.minDistance = 1000;
		au.maxDistance =1000;
		au.clip =(AudioClip)Resources.Load("Audio/Sounds/"+Voice);
		au.Play ();
		Destroy(VoiceGameObject,DeadTime);
	}

}
