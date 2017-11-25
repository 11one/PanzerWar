using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleDontDestroyManagerModule : MonoBehaviour {
	public List<GameObject> DontDestroyObject;
	public VehicleDontDestroyManagerModule (){
		DontDestroyObject = new List<GameObject> ();
	}
	public void Add(GameObject _gameObject){
		DontDestroyOnLoad (_gameObject);
		DontDestroyObject.Add (_gameObject);
	}

	public void Clean(){
		for (int i = 0; i < DontDestroyObject.Count; i++) {
			Destroy (DontDestroyObject [i]);
		}
	}
}
