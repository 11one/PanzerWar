using UnityEngine;
using System.Collections;

public class FireSmoke : MonoBehaviour {
	float Counter =0;
	void OnEnable(){
		Counter = 0;
	}
	void Update(){
		Counter += Time.deltaTime;
		if (Counter < 2) {
			GetComponent<ParticleSystem> ().Emit (1);
		}
	}
}
