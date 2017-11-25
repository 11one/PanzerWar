using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class StaticResourcesReferences : MonoBehaviour {
	public static StaticResourcesReferences Instance;

	public AudioMixer GlobalAudioMixer;

	public PhysicMaterial VehiclePhysicMaterial;




	void Start () {
		if (Instance != null)
			Destroy (gameObject);
		
		Instance = this;
		DontDestroyOnLoad (gameObject);
	}
}
