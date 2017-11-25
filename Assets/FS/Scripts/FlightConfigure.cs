using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightConfigure : MonoBehaviour
{
	public GameObject PlaneInfo;
	public Engine[] Engines;
	[Header ("WingOuter")]
	public ControlSurface[] VerticalControls;
	[Header ("HStabilator")]
	public ControlSurface[] HorizontalControls;
	[Header ("VStabilator")]
	public ControlSurface[] VStabilizer;
	public FlightHelper flightHelper;
	public float Throttle = 0;


	bool IsMobile = false;

	void Awake ()
	{
		if (!cInput.MobileEnableMonitor)
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.IPhonePlayer)
			IsMobile = true;
	}

	void Start ()
	{
		
	}

	void Update ()
	{
		float _InputPitch = 0, _InputRoll = 0, _InputYaw = 0;
		Throttle += Time.deltaTime * 10;

		Throttle += cInput.GetAxis ("Throttle");
		Throttle = Mathf.Clamp (Throttle, 0, 100);
		foreach (Engine engine in Engines) {
			engine.ThrottleController.SetManualInputMinusOneToOne (Throttle / 100f);
		}
		if (IsMobile) {
			_InputPitch = -Input.acceleration.z * 2;
			_InputRoll = Input.acceleration.x * 2;
		} else {
			_InputRoll = cInput.GetAxis ("Roll");
			_InputPitch = cInput.GetAxis ("Pitch");
			_InputYaw = cInput.GetAxis ("Yaw");
		}



//		Vector3 FlightDir = flightHelper.GetLocalDir();
//		Debug.Log(FlightDir.ToString() + flightHelper.GetRoll().ToString());
//
//		foreach (ControlSurface controlSuface in VStabilizer) {
//			controlSuface.Controller.SetManualInputMinusOneToOne(FlightDir.x * 20 + _InputYaw);
//		}
//
//		foreach (ControlSurface controlSuface in HorizontalControls) {
//			controlSuface.Controller.SetManualInputMinusOneToOne(-FlightDir.y*10+_InputPitch);
//		}
//
//		foreach (ControlSurface controlSuface in VerticalControls) {
//			controlSuface.Controller.SetManualInputMinusOneToOne(FlightDir.x + flightHelper.GetRoll() / 180);
//		}
//
//		if (Mathf.Abs(flightHelper.GetRoll()) > 90) {
//			foreach (ControlSurface controlSuface in VerticalControls) {
//				controlSuface.Controller.SetManualInputMinusOneToOne(flightHelper.GetRoll() / 10 + _InputRoll);
//			}
//		}
//		} else {
//
//		}
		//return;

		foreach (ControlSurface controlSuface in VerticalControls) {
			controlSuface.Controller.SetManualInputMinusOneToOne (_InputRoll);
		}

		foreach (ControlSurface controlSuface in HorizontalControls) {
			controlSuface.Controller.SetManualInputMinusOneToOne (_InputPitch);
		}
		foreach (ControlSurface controlSuface in VStabilizer) {
			controlSuface.Controller.SetManualInputMinusOneToOne (_InputYaw);
		}

		if (flightHelper.flightCamera.UserInView) {
			foreach (ControlSurface controlSuface in VerticalControls) {
				controlSuface.Controller.SetManualInputMinusOneToOne(flightHelper.GetRoll() / 10);
			}
			foreach (ControlSurface controlSuface in HorizontalControls) {
				controlSuface.Controller.SetManualInputMinusOneToOne(-flightHelper.GetLocalDir ().y * 10);
			}
			Debug.Log(flightHelper.GetLocalDir ());
		}

	}
}
