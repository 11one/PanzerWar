using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAmmo : MonoBehaviour {
	float Velocity = 30;
	float Gravity = 10f;

	public float Angle;
	float t =0;
	Vector3 p0;
	// Use this for initialization
	void Start () {
		p0 = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		t += Time.fixedTime;
		Vector2 _xy = XY (Angle, Velocity, t);

		transform.position = p0 + new Vector3 (0, _xy.y, _xy.x); 
	}
	Vector2 XY(float _angle,float V0,float t){
		float V0x = V0 * Mathf.Cos (_angle * Mathf.Deg2Rad);
		float V0y = V0 * Mathf.Sin (_angle * Mathf.Deg2Rad) - Gravity*t;

		float x = V0x * t;
		float y = V0y * t +0.5f * Gravity*t*t;
		return new Vector2 (x, y);
	}
}
