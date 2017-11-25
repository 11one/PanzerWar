using UnityEngine;
using System.Collections;

public class cInputDemoSetup : MonoBehaviour {
	void Start() {
		// initialize cInput
		cInput.Init();

		// cINPUT SETUP
		cInput.SetKey("Forward",Keys.W);
		cInput.SetKey("Back",Keys.S);
		cInput.SetKey("Left",Keys.A);
		cInput.SetKey("Right",Keys.D);
		cInput.SetKey ("Brake", Keys.Space);

		cInput.SetKey("MainGunFire",Keys.Mouse0);
		cInput.SetKey("MachineGun",Keys.Q);
		cInput.SetKey("OpenFireCross",Keys.E);

		cInput.SetKey("IncreaseRPM",Keys.R);
		cInput.SetKey("DecreaseRPM",Keys.F);

		cInput.SetKey ("CameraLeft", Keys.MouseLeft);
		cInput.SetKey ("CameraRight", Keys.MouseRight);
		cInput.SetKey ("CameraUp", Keys.MouseUp);
		cInput.SetKey ("CameraDown", Keys.MouseDown);

		cInput.SetKey ("MouseScrollWheelUp", Keys.MouseWheelUp);
		cInput.SetKey ("MouseScrollWheelDown", Keys.MouseWheelDown);
		cInput.SetKey ("ToogleCamera", Keys.LeftShift);

		cInput.SetKey ("SelectAmmo1", Keys.Alpha1);
		cInput.SetKey ("SelectAmmo2", Keys.Alpha2);
		cInput.SetKey ("SelectAmmo3", Keys.Alpha3);

		cInput.SetKey ("AutoAim", Keys.X);


		cInput.SetKey ("TurnLeft", Keys.Q);
		cInput.SetKey ("TurnRight", Keys.E);

		cInput.SetKey("Ignition",Keys.I);
		cInput.SetKey ("Landing", Keys.G);
		cInput.SetKey ("Plus", Keys.LeftControl);
		cInput.SetKey ("Minus", Keys.LeftAlt);

		cInput.SetAxis("Throttle","Minus","Plus");

		cInput.SetAxis("Mouse ScrollWheel","MouseScrollWheelUp","MouseScrollWheelDown");
		cInput.SetAxis("CameraZoom","MouseScrollWheelUp","MouseScrollWheelDown");

		cInput.SetAxis("Tank Horizontal Movement","Left","Right");
		cInput.SetAxis("Tank Vertical Movement","Forward","Back");
		cInput.SetAxis ("Camera Horizontal", "CameraLeft", "CameraRight",10.5f,6.5f);
		cInput.SetAxis("Camera Vertical","CameraUp","CameraDown",10.5f,6.5f);
		cInput.SetAxis ("Roll", "Left","Right");
		cInput.SetAxis ("Flag", "Left","Right");
		cInput.SetAxis ("Pitch", "Back","Forward");
		cInput.SetAxis ("Yaw", "TurnLeft", "TurnRight");
		//cInput.SetAxis("Horizontal Movement","Mouse Right","Mouse Left", 4.5f);
	}


}
