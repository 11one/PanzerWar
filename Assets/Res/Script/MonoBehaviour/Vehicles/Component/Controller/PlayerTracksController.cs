using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class PlayerTracksController :TankTracksController
{
	public TankInitSystem tankInitSystem;

	public bool enableUserInput = true;
	public GameObject Keybutton;
	bool IsMobile = false;
	string ControlType;
	public int SwitchRPM = 0;

	public static Joystick ControlJoyStick;
	public static GameObject etcUI;

	public void AddRPM ()
	{
		SwitchRPM++;
		SwitchRPM = Mathf.Clamp (SwitchRPM, -4, 4);
	}

	public void DownRPM ()
	{
		SwitchRPM--;
		SwitchRPM = Mathf.Clamp (SwitchRPM, -4, 4);
	}

	public override void Start ()
	{
		base.Start ();
	}

	public bool Delegated = false;

	#region 初始化操控UI 与判断是否是 自己坦克

	public void InitUISystemForPTC (Transform UIoj)
	{
		#region 移动平台非强制使用电脑操控
		if (!cInput.MobileEnableMonitor) {
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
				IsMobile = true;
		}
		#endregion
		#region 控制面板逻辑
		if (IsMobile) {
			if (PlayerPrefs.HasKey ("ControllorType"))
				ControlType = PlayerPrefs.GetString ("ControllorType");
			else
				ControlType = "Joystick";

			Keybutton = UIoj.root.Find ("Middle/Android/KeyBoardControllor").gameObject;

			if (ControlType != "Key") {
				Keybutton.SetActive (false);	
				if (etcUI != null) {
					Destroy (etcUI);
					etcUI = null;
					ControlJoyStick = null;
				}

				if (ControlJoyStick == null || etcUI == null) {
					GameObject PTCCanvas = (GameObject)Instantiate (Resources.Load ("PTCCanvas"));

					tankInitSystem.dontDestroyManager.Add(PTCCanvas);

					etcUI = PTCCanvas;
					ControlJoyStick = PTCCanvas.GetComponentInChildren<Joystick> ();
				}
				ControlJoyStick.OnStartJoystickMovement += On_JoystickMoveStart;
				ControlJoyStick.OnJoystickMovement += On_JoystickMove;
				ControlJoyStick.OnEndJoystickMovement += On_JoystickMoveEnd;

				Delegated = true;

				etcUI.transform.SetAsFirstSibling ();
			} else {
				Keybutton.SetActive (true);
			}
		}
		#endregion

	}

	#endregion

	void Update ()
	{
		if (!enableUserInput)
			return;
		
		if (cInput.GetKey ("Brake")) {
			isBrakeing = true;
		} else {
			isBrakeing = false;
		}

		if (cInput.GetKeyDown ("IncreaseRPM"))
			AddRPM ();
		if (cInput.GetKeyDown ("DecreaseRPM"))
			DownRPM ();

		MoveInput ();

		if (IsMobile) {
			#region 防止之前数据干扰
			if (ControlType == "Key") {
				#region 按键操控
				if (!KeyInRotate) {
					steerG = 0;
				}
				#endregion
			} else {
				#region 摇杆
				if (!JostickOnMove) {
					steerG = 0;
				}
				#endregion
			}
			#endregion
		}
		if ((uGUI_ControllorSetting.B_EnableAutoDriveGround || accelG > 0) && enableUserInput && uGUI_ControllorSetting.B_EnableAutoDrive && Mathf.Abs (steerG) < 0.6f) {
			Vector3 LocalDir = transform.InverseTransformVector (GameEvent.PlayerCamera.transform.forward);
			steerG = LocalDir.x;

//			TurretForward = new Vector3 (TurretForward.x, 0, TurretForward.z);
//			TurretForward = TurretForward.normalized;
//
//			if (Vector3.Angle (TurretForward, transform.forward) > 15) {
//				cos = Mathf.Clamp (Vector3.Dot (TurretForward, transform.forward), 0, 1);
//			} else {
//				cos = 1;
//			}
//
//
//			float isRight = Vector3.Dot (transform.right, TurretForward);
//			cos = 1 - cos;
//
//			//Debug.Log (cos);
//
//			if (isRight <= 0) {					
//				cos *= -1;				
//			} 
//			if(accelG>0)
//				steerG = Mathf.Clamp (steerG+cos*accelG, -1, 1);
//			else 
//				steerG = Mathf.Clamp (steerG+cos, -1, 1);
		}

	}



	void MoveInput ()
	{
		if (enableUserInput && IsMobile) {
			if (ControlType == "Key") {
				#region 挂档算法
				if (SwitchRPM != 0) {
					if (SwitchRPM > 0)
						accelG = PushSpeed * (SwitchRPM * 0.25f);
					else
						accelG = BackSpeed * (SwitchRPM * 0.25f);
				}
				#endregion
			}

		}
		if (enableUserInput && !IsMobile) {
			if (cInput.GetKey ("Back")) {
				accelG = -cInput.GetAxis ("Tank Vertical Movement") * BackSpeed;
				steerG = -cInput.GetAxis ("Tank Horizontal Movement");
			} else {
				accelG = -cInput.GetAxis ("Tank Vertical Movement") * PushSpeed;
				steerG = cInput.GetAxis ("Tank Horizontal Movement");
			}
			if (cInput.GetAxis ("Tank Vertical Movement") != 0) {
				SwitchRPM = 0;
			}
			#region 挂档算法
			if (SwitchRPM != 0) {
				if (SwitchRPM > 0)
					accelG = PushSpeed * (SwitchRPM * 0.25f);
				else
					accelG = BackSpeed * (SwitchRPM * 0.25f);
			}
			#endregion
		}		
	}

	void Brake(){
		sidewaysFrictionAsymptoteFactor = 0.15f;
		sidewaysFrictionExtremumFactor = 0.15f;
		isBrakeingAble = true;
	}
	void NotBrake(){
		sidewaysFrictionAsymptoteFactor = 1.2f;
		sidewaysFrictionExtremumFactor = 1.2f;
		isBrakeingAble = false;
	}
	void FixedUpdate ()
	{		
		if (isBrakeing&&CurrentSpeed>25) {
			Brake ();
		} 
		else if(isBrakeing&&isBrakeingAble&&CurrentSpeed>10){
			Brake ();
		}
		else {
			NotBrake ();
		}
		if (isBrakeing) {
			accelG = 0f;
		}
		if (CurrentSpeed > MaxSpeed || CurrentSpeed < MinSpeed) {
			rigidBody.velocity = new Vector3 (rigidBody.velocity.x * 0.95f, rigidBody.velocity.y, rigidBody.velocity.z * 0.95f);
		}

		if (!OnlyReceiveControlActions || enableUserInput) {
			UpdateWheels (accelG, steerG);
			UpdateTrackByAngularVelocity (accelG, steerG, true);
		} 
	}

	public void Forward (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		SwitchRPM = 0;
		accelG = 1 * PushSpeed;
	}

	public void CancelForward (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		accelG = 0;
	}

	public void Back (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		SwitchRPM = 0;
		accelG = -1 * PushSpeed;
	}

	public void	CancelBack (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		accelG = 0;
	}

	bool KeyInRotate = false;

	public void Right (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		steerG = 1 * PushSpeed;
		KeyInRotate = true;
	}

	public void	CancelRight (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		steerG = 0;
		KeyInRotate = false;
	}

	public void Left (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		steerG = -1 * PushSpeed;
		KeyInRotate = true;
	}

	public void	CancelLeft (BaseEventData arg0)
	{
		if (!enableUserInput) {
			return;
		}
		steerG = 0 * PushSpeed;
		KeyInRotate = false;
	}

	void OnDestroy ()
	{
		if (Delegated) {
			Destroy (etcUI);
			ControlJoyStick.OnJoystickMovement -= On_JoystickMove;
			ControlJoyStick.OnEndJoystickMovement -= On_JoystickMoveEnd;
			ControlJoyStick.OnStartJoystickMovement -= On_JoystickMoveStart;
		}
	}

	bool JostickOnMove = false;

	void On_JoystickMoveStart (Joystick joystick, Vector2 move)
	{
		JostickOnMove = true;
		On_JoystickMove (joystick, move);
	}

	void On_JoystickMoveEnd (Joystick joystick)
	{
		JostickOnMove = false;
		accelG = 0;
		steerG = 0;
	}

	void On_JoystickMove (Joystick joystick, Vector2 move)
	{
		if (!enableUserInput)
			return;
		if (!IsMobile)
			return;
		if (ControlType == "Key")
			return;
		
		
		float x = move.x;
		float y = move.y;
		if (Mathf.Abs (x) < 0.25f) {
			x = 0;		
		} 
		else if (Mathf.Abs (x) > 0.9f) {
			if (x > 0) {
				x = 1;
			} else {
				x = -1;
			}
		} 
		else {
			if (x > 0) {
				x -= 0.25f;
			} else {
				x += 0.25f;
			}
			x *= 1.55f;
		}


		if (y > 0 || Mathf.Abs (y) < 0.2f) {
			accelG = y * PushSpeed;
			steerG = x * PushSpeed;
		} else {
			accelG = y * BackSpeed;
			steerG = -x * BackSpeed;
		}

	}

}
