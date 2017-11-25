using UnityEngine;
using System.Collections;

public class WeaponSound : MonoBehaviour {
	public AudioClip CannonShot,CannonShot_Far;
	public AnimationCurve CannonShot_ACurve = AnimationCurve.Linear(0,1,5,0),CannonShot_Far_ACurve = AnimationCurve.Linear(5,1,200,0);
	void Start(){
		Init ();
	}
	void Init(){
		AudioSource Near = gameObject.AddComponent<AudioSource> ();
		Near.clip = CannonShot;
		Near.minDistance = 0;
		Near.maxDistance = 25;
		Near.spatialBlend = 1;
		Near.rolloffMode = AudioRolloffMode.Custom;
		Near.SetCustomCurve (AudioSourceCurveType.CustomRolloff, CannonShot_ACurve);
		AudioSource Far = gameObject.AddComponent<AudioSource> ();
		Far.clip = CannonShot_Far;
		Far.minDistance = 15;
		Far.maxDistance = 200;
		Far.spatialBlend = 1;
		Far.rolloffMode = AudioRolloffMode.Custom;
		Far.SetCustomCurve (AudioSourceCurveType.CustomRolloff, CannonShot_Far_ACurve);
		Near.Play ();
		Far.Play ();
	}

}
