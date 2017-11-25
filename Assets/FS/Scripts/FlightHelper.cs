using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightHelper : MonoBehaviour {
	public FlightCamera flightCamera;
	GameObject Helper;

	// Use this for initialization
	void Start () {
		Helper = new GameObject ("Helper");
		Helper.transform.SetParent (transform);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay(transform.position, GetLocalDir());
	}


	public Vector3 GetLocalDir(){
		return transform.InverseTransformDirection(new Vector3(transform.forward.x,0,transform.forward.z));
	}
	public float GetRoll(){
		float _roll = transform.eulerAngles.z;
		if (_roll > 180)
			return _roll - 360;
		else
			return _roll;
	}
}
