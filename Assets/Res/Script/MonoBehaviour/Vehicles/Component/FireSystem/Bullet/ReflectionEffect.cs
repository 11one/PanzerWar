using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionEffect : MonoBehaviour {
	void Update () {
		transform.Translate (Vector3.forward*350*Time.deltaTime);
	}
}
