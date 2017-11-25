using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
	//Vector3 PlaneNormal = new Vector3(0,1,0);
	[HideInInspector]
	public float Velocity = 300;
	[HideInInspector]
	public float Gravity = 9.8f;
	// Use this for initialization
//	void Start () {
//		
//	}
	
	// Update is called once per frame
//	void Update () {
//		List<Vector3> Points = new List<Vector3> ();
//		float angle = FireAngle (transform.forward,new Vector3 (0, 1, 0));
//		for (int i = 0; i < 200; i++) {
//			Vector2 _xy = XY (angle, Velocity, i);
//			//ProcessedXY (new Vector3 (0, -_xy.y, _xy.x),transform.eulerAngles.y, transform.eulerAngles.x);
//			Points.Add (transform.position+ProcessedXY (new Vector3 (0, -_xy.y, _xy.x),transform.eulerAngles.x, transform.eulerAngles.y));
//		}
//		DebugLine (Points);
//
//		Debug.Log (angle);
//	}
	public Vector3 ProcessedXY(Vector3 _p,float _angle,float _dir){
		return Quaternion.AngleAxis(_dir, Vector3.up)* (Quaternion.AngleAxis(_angle, Vector3.right)*_p);
		//return Quaternion.AngleAxis(transform.eulerAngles.x, Vector3.right)* Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up)  * new Vector3 (0, -_xy.y, _xy.x));
	}
	public float FireAngle(Vector3 d,Vector3 n){
		return  90 - Vector3.Angle (d, n);
	}
	public Vector2 XY(float t){
		float V0x = Velocity;
		float V0y = Gravity*t;

		float x = V0x * t;
		float y = 0.5f *V0y* t;
		return new Vector2 (x, y);
	}
	public void DebugLine(List<Vector3> _points){
		Vector3 p = _points [0];
		for (int i = 0; i < _points.Count; i++) {
			Debug.DrawLine (p, _points [i]);
			p = _points [i];
		}
	}
	//X距离得到t
	public float x2t(float x){
		return x / Velocity;
	}
}
