using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VehicleCamera : MonoBehaviour
{
    public enum CameraMode {
        TankCamera,
        AirCamera
    }

	public Transform target;

    public Transform mainCameraFollowTarget, mainCameraGunner;

    private float desireDistance=-15;

	private float x;
	private float y;
    private float z = 10.0f;

    private const float maxFOV = 60;

    private const float minFov = 5;


	private float yMinLimit = -45, yMaxLimit = 90;
    	
    private float xSpeed = 120.0f;
	private float ySpeed = 60.0f;
	private int CameraType = 0;

	public bool IsOpenFireCross = false;

	public bool isVehicleSPG = false;

	public bool IsMobile = false;

	public Image FireCross_Texture, CrossHair, FinalPostion,SPGFinalPosition,SPGIndicatePoint; //CompactMirror;

    public bool isGarageModeCamera = false;


    private ArrayList CullObjectsMeshs = new ArrayList();
	private ArrayList CullObjectsMeshSkins = new ArrayList();



	public CameraMode cameraMode;
	Camera MyCamera;
	public Vector3 CameraOffSet;

	float InFireZoom = 60;

	public void CameraChange()
	{
		CameraType++;
		switch (CameraType) {
			case 0:
				desireDistance = -15;
			break;
			case 1:
				desireDistance = -7.5f;
			break;
			case 2:
				desireDistance = 0;
			break;
			case 3:
				CameraType = -1;
				CameraChange();
			break;
		}
	}

	public bool inSPGMode = false;

	public void OpenFireCross()
	{
		if (cameraMode == CameraMode.TankCamera) {
			if (isVehicleSPG) {
				inSPGMode = !inSPGMode;
				if (inSPGMode) {
					xSpeed = uGUI_ControllorSetting.F_MoveSpeedInFireCross * 0.25f;
					ySpeed = uGUI_ControllorSetting.F_MoveSpeedInFireCross * 0.25f;
                    target = mainCameraGunner.transform;
                    target.transform.localPosition += new Vector3(0, 25, 0);

                    GetComponent<Camera>().fieldOfView = InFireZoom;
					IsOpenFireCross = true;


				} else {
					xSpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;
					ySpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;
					target.transform.localPosition += new Vector3(0, -25, 0);
                    target = mainCameraFollowTarget.transform;

                    IsOpenFireCross = false;

				}
				return;
			}

			GameEvent.PlayerInCrossFire = !IsOpenFireCross;

			#region 开镜行为
			if (!IsOpenFireCross) {
				IsOpenFireCross = true;

				//MyCamera.cullingMask = 
				if (isVehicleSPG) {
					//target.transform.localPosition += new Vector3(0,,0);
				} else {
					if (!uGUI_ControllorSetting.B_EnableSaveZoom)
						InFireZoom = 45;

					foreach (MeshRenderer cullObject in CullObjectsMeshs) {
						cullObject.enabled = false;
					}

                    target = mainCameraGunner.transform;

                    transform.localPosition = Vector3.zero;
                    transform.localEulerAngles = Vector3.zero;

                    GetComponent<Camera>().fieldOfView = InFireZoom;


					yMinLimit = -45;
					yMaxLimit = 25;

					Debug.Log(yMaxLimit);

					desireDistance = 0;
					xSpeed = uGUI_ControllorSetting.F_MoveSpeedInFireCross;
					ySpeed = uGUI_ControllorSetting.F_MoveSpeedInFireCross;



					FireCross_Texture.gameObject.SetActive(true);
					CrossHair.gameObject.SetActive(false);

					BackGround.PlaySound("turret_turn", 2);
					MyCamera.cullingMask&=  ~(1 << LayerMask.NameToLayer("TransparentFX"));
				}


			} 
			#endregion
			#region 关镜行为
			else {
				IsOpenFireCross = false;


				if (isVehicleSPG) {
					//target.transform.localPosition += new Vector3(0,-15,0);
				} else {
					foreach (MeshRenderer cullObject in CullObjectsMeshs) {
						cullObject.enabled = true;
					}

					yMinLimit = -45;
					yMaxLimit = 90;
                    target = mainCameraFollowTarget.transform;

					GetComponent<Camera>().fieldOfView = maxFOV;
					xSpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;
					ySpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;
					desireDistance = -10;
					MyCamera.cullingMask = OriginalCullMask;

					FireCross_Texture.gameObject.SetActive(false);
					CrossHair.gameObject.SetActive(true);
				}
			}
			#endregion
		}
	}

	int OriginalCullMask;
	#if ClientCode
	void Start()
	{
		GameEvent.PlayerInCrossFire = false;
		MyCamera = GetComponent<Camera>();
		OriginalCullMask = MyCamera.cullingMask;

		MyCamera.layerCullSpherical = false;

		float[] distances = new float[32];
		distances[LayerMask.NameToLayer("Building")] = 250;
		distances[LayerMask.NameToLayer("Terrian")] = 800;

		MyCamera.layerCullDistances = distances;





		xSpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;
		ySpeed = uGUI_ControllorSetting.F_MoveSpeedNormal;


		if (Application.loadedLevelName == "garage_Cooked") {
			isGarageModeCamera = true;
		}

		if (cameraMode == CameraMode.TankCamera && !isGarageModeCamera) {
			InvokeRepeating("RayShowEnemyInfo", 1, 0.25f);

			foreach (MeshRenderer ms in transform.root.GetComponentInChildren<TurretController> ().Turret.GetComponentsInChildren<MeshRenderer>()) {
				CullObjectsMeshs.Add(ms);
			}

		}

		
		goRight();

		//MobileInput._Instance.OnMovement += HandleMovement;

	}
	#endif
	float Drag = 10f;
	void HandleMobileCamera(Vector2 Delta)
	{
		if (IsOpenFireCross) {
			Delta *= uGUI_ControllorSetting.F_MoveSpeedInFireCross;
		} 
		else {
			Delta *= uGUI_ControllorSetting.F_MoveSpeedNormal;
		}

		if (isGarageModeCamera) {
			Delta += MobileInput.DragForce*Time.deltaTime;
			MobileInput.DragForce = Vector2.Lerp(MobileInput.DragForce, Vector2.zero, Time.deltaTime*2);
		}
		x += Delta.x;
		y += Delta.y;
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		desireDistance = Mathf.Clamp(desireDistance, -10.0f, 0);
		goRight();
	}

	int TouchMoveID = -1;

	#if ClientCode
	void LateUpdate()
	{
		UpdateFinalAmmoPostion();
		//UpdateCompactMirrorScale();
		CheckCollison();

		if (IsMobile) {
			float zoom = 1;
			zoom = MyCamera.fieldOfView / maxFOV;
			#region 车库中
			if (isGarageModeCamera) {
				ZoomCamera();
				if(!inZoom){
					HandleMobileCamera(MobileInput.MoveDelta);
				}

			} 
			#endregion
			#region 战斗中
			else {
				if (uGUI_ControllorSetting.B_EnableCameraJoyStick) {
					#region Handle JoyStick movement
					HandleMobileCamera(new Vector2(CameraJoyStickMoveVector.x * xSpeed, -CameraJoyStickMoveVector.y * ySpeed));
					#endregion
				} else {
                    ZoomCamera();

                    if (IsOpenFireCross || inSPGMode) {
						if(!inZoom){
							HandleMobileCamera(MobileInput.MoveDelta * 0.25f * zoom);
						}
					} else {
						HandleMobileCamera(MobileInput.MoveDelta);
					}
				}
			}
			#endregion

		}
		#region CInput 操控
		if (!IsMobile) {
			float MouseScrollWheel = -cInput.GetAxis("Mouse ScrollWheel");
			bool MouseMoveEnable = true;
			#region 车库
			if (isGarageModeCamera) {
				if (Input.GetKey(KeyCode.Mouse0) && Input.mousePosition.y > Screen.width * 0.2f) {
					desireDistance -= MouseScrollWheel * 100.0f * Time.deltaTime;
				}else {
					MouseMoveEnable =false;
				}
			} 
			#endregion
			else {
				#region 战斗中
				if (cInput.GetKeyDown("OpenFireCross"))
					OpenFireCross();

				if (cInput.GetKeyDown("ToogleCamera"))
					CameraChange();
				
				if (IsOpenFireCross) {
					InFireZoom -= MouseScrollWheel * 100.0f * Time.deltaTime;
                    InFireZoom = Mathf.Clamp(InFireZoom,minFov, 50);
				} else {
					desireDistance += MouseScrollWheel * 100.0f * Time.deltaTime;
				}		
				#endregion
			}
			if(MouseMoveEnable){
				x += Input.GetAxis("Mouse X") * xSpeed * 60 * Time.deltaTime * (MyCamera.fieldOfView / 60);
				y += Input.GetAxis("Mouse Y") * ySpeed * 60 * Time.deltaTime * (MyCamera.fieldOfView / 60);
			}
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			desireDistance = Mathf.Clamp(desireDistance, -10.0f, 1);
			goRight();
		}
		#endregion

		if (IsOpenFireCross || inSPGMode || isGarageModeCamera) {
			GetComponent<Camera>().fieldOfView = InFireZoom;
		} else {
			GetComponent<Camera>().fieldOfView = maxFOV;
		}
	}
	#endif

	float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		
		return  Mathf.Clamp(angle, min, max);
	}





	public void goRight()
	{
		Quaternion rotation = Quaternion.Euler(y, x, 0);
		Vector3 position = rotation * new Vector3(0.0f, 0.0f, z) + target.position;
		this.transform.position = position;
		this.transform.rotation = rotation;
	}
	void CheckCollison()
	{
		RaycastHit hit;

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        Vector3 cameraNextPosition = rotation * new Vector3(0.0f, 0.0f, desireDistance) + target.position;
                                                                         
		if (IsOpenFireCross == false) {
			if (Physics.Raycast(cameraNextPosition, target.position - this.transform.position, out hit, Vector3.Distance(target.position, cameraNextPosition), 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Terrian"))) {
				z = -Mathf.Abs(Vector3.Distance(target.position, hit.point) - 0.5f);
			} else {
                z = desireDistance;
			}
		} else {
            z = 0;
		}

	}

	Identity CurrentID;

	void RayShowEnemyInfo()
	{
		RaycastHit hit;

		int Mask = 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Terrian") | 1 << LayerMask.NameToLayer("HitBox");

		if (Physics.Raycast(transform.position + transform.forward * desireDistance * -1, transform.forward, out hit, 500, Mask)) {
			Identity id = hit.collider.GetComponentInParent<Identity>();
			if (id != null) {
				if (GameEvent.Player != null)
				if (GameEvent.Player.identity != id.identity) {
					if (CurrentID != null) {
						if (id != CurrentID) {
							CurrentID.Hide();
							id.Show();
							CurrentID = id;
						}
					} else {
						id.Show();
						CurrentID = id;
					}
					GameEvent.InstantPlayerState.UpdateOtherPlayerHealthBar(id.CurrentHealth / id.InitHealth);
				}

			} else {
				if (CurrentID != null) {
					CurrentID.Hide();
					CurrentID = null;
					GameEvent.InstantPlayerState.UpdateOtherPlayerHealthBar(0);
				}
				GameEvent.InstantPlayerState.UpdateOtherPlayerHealthBar(0);
			} 
		} else {
			if (CurrentID != null) {
				CurrentID.Hide();
				CurrentID = null;
			}
			if (GameEvent.InstantPlayerState != null) {
				GameEvent.InstantPlayerState.UpdateOtherPlayerHealthBar(0);
			}
		} 

	}

	Vector2 old_Finger01, old_Finger02;
	public bool inZoom =false;
	void ZoomCamera()
	{
		if (Input.touchCount > 1) {
			if (Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Moved && Input.GetTouch(Input.touchCount - 2).phase == TouchPhase.Moved) {
				inZoom = true;
				Vector2 Finger01 = Input.GetTouch(Input.touchCount - 1).position, Finger02 = Input.GetTouch(Input.touchCount - 2).position;
				if (Finger01.x > Screen.width * 0.6f && Finger02.x > Screen.width * 0.6f) {
					if (old_Finger01 != Vector2.zero && old_Finger02 != Vector2.zero) {
						float DeltaDistance = Vector3.Distance(old_Finger01, old_Finger02) - Vector3.Distance(Finger01, Finger02);
						if (IsOpenFireCross || isGarageModeCamera) {
							InFireZoom += (DeltaDistance / 20);
                            InFireZoom = Mathf.Clamp(InFireZoom, minFov, maxFOV);
						} else {
							desireDistance -= (DeltaDistance / 50);
							desireDistance = Mathf.Clamp(desireDistance, -10.0f, 0);
						}
					}
					old_Finger01 = Finger01;
					old_Finger02 = Finger02;
					this.goRight();
					return;
	
				} else {
					inZoom = false;
				}
			} 
			else {
				old_Finger01 = Vector3.zero;
				old_Finger02 = Vector3.zero;
				inZoom = false;

			}
	
		} else {
			inZoom = false;
		}
	}

	Vector2 CameraJoyStickMoveVector = Vector2.zero;

	public void On_JoystickStop(Joystick joystick)
	{
		CameraJoyStickMoveVector = Vector2.zero;
	}

	public void On_JoystickMove(Joystick joystick, Vector2 move)
	{
		CameraJoyStickMoveVector = move;
	}

	void UpdateFinalAmmoPostion()
	{
		if (isVehicleSPG) {
			if (GameEvent.HasFinalPostion) {
				SPGFinalPosition.gameObject.SetActive(true);
				Vector3 ScreenPostion = GameEvent.PlayerCamera.WorldToScreenPoint(GameEvent.PlayerAmmoFinalPostion);
				SPGFinalPosition.transform.position = ScreenPostion;
			} else {
				SPGFinalPosition.gameObject.SetActive(false);
			}
			return;
		}

		if (IsOpenFireCross) {
			if (GameEvent.HasFinalPostion) {
				if (GameEvent.PlayerCamera != null & FinalPostion != null) {
					FinalPostion.gameObject.SetActive(true);
					Vector3 ScreenPostion = GameEvent.PlayerCamera.WorldToScreenPoint(GameEvent.PlayerAmmoFinalPostion);
					FinalPostion.transform.position = ScreenPostion;
				} 
			} else {
				FinalPostion.gameObject.SetActive(false);
			}
		}
	}


		
}