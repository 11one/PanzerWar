using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class uGUI_ControllorSetting : MonoBehaviour {
	public InputField MoveSpeedNormal,MoveSpeedInFireCross;
	public Dropdown ControlWay;
	public Toggle EnabelMonitor,EnableAutoDrive,EnableCameraJoyStick,EnableAutoDriveGround,EnableSaveZoom;
	public static bool B_EnableAutoDrive =false,B_EnableCameraJoyStick=false,B_EnableAutoDriveGround=false,B_EnableSaveZoom=false;
	public static float F_MoveSpeedNormal=1,F_MoveSpeedInFireCross=1;

	public void OnEnable(){
		Read ();
	}
	public void Save(){
		//PlayerPrefs.SetString ("ControllorType", ControlWay.);
		PlayerPrefs.SetFloat ("MoveSpeedNormal", float.Parse(MoveSpeedNormal.text));
		PlayerPrefs.SetFloat ("MoveSpeedInFireCross",float.Parse(MoveSpeedInFireCross.text));
		PlayerPrefs.SetString ("EnabelMonitor", EnabelMonitor.isOn.ToString ());
		PlayerPrefs.SetString ("EnableAutoDrive", EnableAutoDrive.isOn.ToString ());
		PlayerPrefs.SetString ("EnableCameraJoyStick", EnableCameraJoyStick.isOn.ToString ());
		PlayerPrefs.SetString ("EnableAutoDriveGround", EnableAutoDriveGround.isOn.ToString ());
		PlayerPrefs.SetString ("EnableSaveZoom", EnableSaveZoom.isOn.ToString ());


		cInput.MobileEnableMonitor = EnabelMonitor.isOn;
		SetControlWay (ControlWay.value);
		Read ();
	}
	public void Read(){
		Debug.Log ("Read");

		string ControlType ="";
		if(PlayerPrefs.HasKey("ControllorType"))
			ControlType = PlayerPrefs.GetString("ControllorType");
		else 
			ControlType = "Joystick";
		
		if (ControlType == "Joystick") {
			ControlWay.value = 0;
		} else {
			ControlWay.value = 1;
		}

		if (PlayerPrefs.HasKey ("MoveSpeedNormal")) {
			MoveSpeedNormal.text = PlayerPrefs.GetFloat ("MoveSpeedNormal").ToString();
			F_MoveSpeedNormal = PlayerPrefs.GetFloat ("MoveSpeedNormal");
		}
		if (PlayerPrefs.HasKey ("MoveSpeedInFireCross")) {
			MoveSpeedInFireCross.text = PlayerPrefs.GetFloat ("MoveSpeedInFireCross").ToString();
			F_MoveSpeedInFireCross = PlayerPrefs.GetFloat ("MoveSpeedInFireCross");
		}

	

		if (PlayerPrefs.HasKey ("EnabelMonitor")) {
			if (PlayerPrefs.GetString ("EnabelMonitor") == "True") {
				EnabelMonitor.isOn = true;
			} else {
				EnabelMonitor.isOn = false;
			}
			cInput.MobileEnableMonitor = EnabelMonitor.isOn;
		}


		if (PlayerPrefs.HasKey ("EnableAutoDrive")) {
			if (PlayerPrefs.GetString ("EnableAutoDrive") == "True") {
				EnableAutoDrive.isOn = true;
				B_EnableAutoDrive = true;
			} else {
				EnableAutoDrive.isOn = false;
				B_EnableAutoDrive = false;
			}
		}

		if (PlayerPrefs.HasKey ("EnableCameraJoyStick")) {
			if (PlayerPrefs.GetString ("EnableCameraJoyStick") == "True") {
				EnableCameraJoyStick.isOn = true;
				B_EnableCameraJoyStick = true;
			} else {
				EnableCameraJoyStick.isOn = false;
				B_EnableCameraJoyStick = false;
			}
		}

		if (PlayerPrefs.HasKey ("EnableAutoDriveGround")) {
			if (PlayerPrefs.GetString ("EnableAutoDriveGround") == "True") {
				EnableAutoDriveGround.isOn = true;
				B_EnableAutoDriveGround = true;
			} else {
				EnableAutoDriveGround.isOn = false;
				B_EnableAutoDriveGround = false;
			}
		}
		if (PlayerPrefs.HasKey ("EnableSaveZoom")) {
			if (PlayerPrefs.GetString ("EnableSaveZoom") == "True") {
				EnableSaveZoom.isOn = true;
				B_EnableSaveZoom = true;
			} else {
				EnableSaveZoom.isOn = false;
				B_EnableSaveZoom= false;
			}
		}

	}
	public void SetControlWay(int option){
		switch (option) {
		case 0:
			PlayerPrefs.SetString ("ControllorType", "Joystick");
			break;
		case 1:
			PlayerPrefs.SetString ("ControllorType", "Key");
			break;
		}
	}
}
