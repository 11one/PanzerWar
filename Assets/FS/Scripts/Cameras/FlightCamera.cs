using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCamera : MonoBehaviour {
	public Transform target;
	float distance = 15.0f;
	public float x;
	public float y;
	public float z;

	public float yMinLimit = -15;
	public float yMaxLimit = 40.0f;

	public bool UserInView = false;

	bool IsMobile = false;
	Vector3 FollowPosition;

	void Awake(){
		if (!cInput.MobileEnableMonitor)
			if (Application.platform == RuntimePlatform.Android ||Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.IPhonePlayer)
				IsMobile = true;
	}
	void Start(){
		z = -distance;
		FollowPosition = target.position;
	}

	void FixedUpdate () {
		UserInView = false;
		if (IsMobile) {
			if (Input.touchCount>0) {
				Vector2 Delta = Input.GetTouch (0).deltaPosition;
				x += Delta.x * 60 * Time.fixedDeltaTime;
				y += Delta.y * 60 * Time.fixedDeltaTime;
				UserInView = true;
			}
		} else {
			if (Input.GetKey (KeyCode.Mouse0)) {
				x += Input.GetAxis ("Mouse X") * 60 * Time.fixedDeltaTime;
				y += Input.GetAxis ("Mouse Y") * 60 * Time.fixedDeltaTime;
				UserInView = true;
			}
		}

		y = ClampAngle(y,yMinLimit,yMaxLimit);
		z = Mathf.Clamp(z, -15.0f, 1);
		goRight();
	}
	float ClampAngle(float angle,float min,float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle,min,max);
	}
	Quaternion rotation;
	public void goRight(){
		if (!UserInView) {
			x = Mathf.Lerp (x, 0, Time.fixedDeltaTime*5);
			y = Mathf.Lerp (y, 0, Time.fixedDeltaTime*5);
			FollowPosition = Vector3.Lerp (FollowPosition, target.position, Time.fixedDeltaTime * 15);
		} else {
			FollowPosition = target.position;
		}

		rotation =Quaternion.Lerp(rotation,Quaternion.Euler(y+target.eulerAngles.x, x+target.eulerAngles.y, target.eulerAngles.z),Time.deltaTime*5);
		Vector3 position = rotation* new Vector3(0.0f, 0.0f, z)+ FollowPosition;
		this.transform.position = position;
		this.transform.rotation = rotation;
	}
}
