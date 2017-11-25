using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using System.Reflection;
#endif
public class TankInitSystem : BaseInitSystem
{


	public GameObject TankTransform, FFPoint, TankPrefabs, InstanceMesh, TurretTransform, MainHitBox, TurretHitBox, GunTransform, EffectStart;
	public Vector3 CameraOffSet = Vector3.zero;
	public Vector3 StartPointOffSet = Vector3.zero;




	public PlayerTankControllerParameter PTCParameter;

	public TankFireParameter TFParameter;

	public bool ExtraTF = false;

	public MultiTurrets[] multiTurrets;

	public MouseTurretParameter MTParameter;

	public PlayerStateParameter PSParameter;

	public bool ShowHitBoxInspecter = true;




    //	public Transform[] AdvanceLeftBones, AdvanceRightBones;
    //	public Transform trackPrefab;


    bool HasInited = false;


	bool isMobile = false;

	public VehicleDontDestroyManagerModule dontDestroyManager;

    public VehicleComponentsReferenceManager referenceManager;

	public void InitTankInitSystem ()
	{
		Debug.Log ("InitTankInitSystem");
		if (_InstanceNetType == InstanceNetType.None) {
			Debug.Log ("Return");
			return;
		}
		if (!HasInited) {
			InitComponent ();
			HasInited = true;
		}
	}

	protected void  InitComponent ()
	{
		if (!cInput.MobileEnableMonitor) {
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.IPhonePlayer)
				isMobile = true;
		}


		//加载载具数据AsssetBundle
		AssetRequestTask vehicleDataRequestTask = new AssetRequestTask() {
			onAssetLoaded=(vehicleTextDataLoaded) => {
				//读取数据
				VehicleTextData vehicleTextData = (VehicleTextData)vehicleTextDataLoaded;
				PTCParameter = vehicleTextData.PTCParameter;
				TFParameter = vehicleTextData.TFParameter;
				ExtraTF = vehicleTextData.ExtraTF;
				multiTurrets = vehicleTextData.multiTurrets;
				MTParameter = vehicleTextData.MTParameter;
				PSParameter = vehicleTextData.PSParameter;

				string AssetBundleVariant = "extramesh";

				#if ClientCode
				AssetBundleVariant = "clientextramesh";
				#else
				AssetBundleVariant = "masterextramesh";
				#endif

				//<-加载载具预制体 AsssetBundle->
				AssetRequestTask vehicleModelRequestTask = new AssetRequestTask() {
					onAssetLoaded= (myReturnValue) => {
						isAssetBundleLoaded = true;
						AsyncLoadingScript((GameObject)myReturnValue);
					}
				};
				vehicleModelRequestTask.SetAssetBundleName(VehicleName + "_pre", AssetBundleVariant);

				AssetBundleManager.LoadAssetFromAssetBundle(vehicleModelRequestTask);

				//<-加载载具预制体 AsssetBundle->

			}
		};
		vehicleDataRequestTask.SetAssetBundleName(VehicleName, "data");

		AssetBundleManager.LoadAssetFromAssetBundle(vehicleDataRequestTask);

			
			


		//EventManager.TriggerEvent ("CutomizeManager.ApplySelect");


	}

	public void AsyncLoadingScript (GameObject myReturnValue)
	{
		#region 读取模型
		transform.position += StartPointOffSet;
		InstanceMesh = Instantiate (myReturnValue) as GameObject;
		InstanceMesh.transform.SetParent (this.transform);
		InstanceMesh.transform.localPosition = Vector3.zero;
		InstanceMesh.transform.localEulerAngles = Vector3.zero;
		#endregion

		#if UNITY_EDITOR && UNITY_ANDROID
		foreach(MeshRenderer meshRenderer in InstanceMesh.GetComponentsInChildren<MeshRenderer>()){
			meshRenderer.sharedMaterial.shader = Shader.Find("Standard");
		}
		#endif

		#region 设置网络状态
		if (_InstanceNetType != InstanceNetType.GarageTank && !isBot (_InstanceNetType) && _InstanceNetType != InstanceNetType.GameNetWorkOffline) {

		}
		#endregion


		//模块加载
		dontDestroyManager = new VehicleDontDestroyManagerModule();

        referenceManager = InstanceMesh.GetComponent<VehicleComponentsReferenceManager>();


        #region 加载完毕游戏资源加载脚本
        TankPrefabs = gameObject;
		TurretTransform = InstanceMesh.transform.Find ("TurretTransform").gameObject;
		MainHitBox = InstanceMesh.transform.Find ("MainHitBox").gameObject;
		TankTransform = InstanceMesh.transform.Find ("TankTransform").gameObject;
		GunTransform = TurretTransform.transform.Find ("GunTransform").gameObject;
		FFPoint = GunTransform.transform.Find ("FFPoint").gameObject;
		EffectStart = GunTransform.transform.Find ("EffectStart").gameObject;
		TurretHitBox = TurretTransform.transform.Find ("TurretHitBox").gameObject;



		GameObject TankScript = null, TankCrossHair = null, MainCamera = null;
		Rigidbody TankPhysic = null;
		TurretController mt = null;
//		SyncGroundVehicle _syncGroundVehicle = null;
		PlayerTracksController PTC = null;
		PlayerState PS = null;
		PTC = InstanceMesh.AddComponent<PlayerTracksController> ();

		#region 物理效果
		TankPhysic = GetComponentInChildren<Rigidbody> ();
		TankPhysic.mass = PTCParameter.Mass;
		TankPhysic.drag = 0.1f;//PTCParameter.Drag;
		TankPhysic.angularDrag = 2.5f;//PTCParameter.AirDrag;
		TankPhysic.useGravity = false;
		BoxCollider[] collisions = PTC.GetComponentsInChildren<BoxCollider> ();
		foreach (Collider collision in collisions) {
			if (collision.isTrigger != true) {
				collision.material = StaticResourcesReferences.Instance.VehiclePhysicMaterial;
			}
		}

        #endregion
        if (_InstanceNetType != InstanceNetType.GarageTank) {
            if (isLocalPlayer(_InstanceNetType)) {
                TankCrossHair = Instantiate((GameObject)Resources.Load("TankCrossHair"), Vector3.zero, Quaternion.identity) as GameObject;
                MainCamera = Instantiate((GameObject)Resources.Load("MainCamera"), Vector3.zero, Quaternion.identity) as GameObject;
                TankCrossHair.transform.parent = TankPrefabs.transform;
                MainCamera.transform.parent = InstanceMesh.transform;
                MainCamera.transform.name = "MainCamera";
            }
            TankScript = Instantiate((GameObject)Resources.Load("TankScript"), Vector3.zero, Quaternion.identity) as GameObject;

            TankScript.transform.name = "TankScript";
            TankScript.transform.parent = TankPrefabs.transform;
            TankPhysic.useGravity = true;

            #region PlayerState 脚本设置
            if (!isBot(_InstanceNetType)) {
                PS = InstanceMesh.AddComponent<PlayerState>();
                PS.tankInitSystem = this;

                foreach (HitBox HB in GetComponentsInChildren<HitBox>()) {
                    HB.SetTarget(PS);
                }
                PS.playerStateParameter = PSParameter;

                PS.netType = _InstanceNetType;
                PS.Turret = TurretTransform;
                PS.TankScript = TankScript;
                PS.DiedDestoryObjects = new Object[] { MainHitBox, TurretHitBox };
                PS.TankName = VehicleName;
                PS.Health = PSParameter.Health;
                PS.engineType = PSParameter.engineType;

                PS.IsMobile = isMobile;
            }
            #endregion
            if (isBot(_InstanceNetType)) {
                EnemyAiState enemyState = InstanceMesh.AddComponent<EnemyAiState>();
                foreach (HitBox HB in GetComponentsInChildren<HitBox>()) {
                    HB.SetTarget(enemyState);
                }
                enemyState.TankScript = TankScript;
                enemyState.Health = PSParameter.Health;
                enemyState.TankName = VehicleName;
                enemyState.MyTeam = ownerTeam;
            }

            #region TankScript 脚本设置
            mt = TankScript.GetComponent<TurretController>();
            if (isLocalPlayer(_InstanceNetType)) {
                mt.target = MainCamera.transform.Find("CameraTarget");
            }
            mt.gun = GunTransform.transform;
            mt.Turret = TurretTransform.transform;
            mt.DownMaxDegree = MTParameter.DownMaxDegree;
            mt.UpMaxDegree = MTParameter.UpMaxDegree;
            mt.maxTurretAngle = MTParameter.maxTurretAngle;
            mt.gunDegreesPerSecond = MTParameter.gunDegreesPerSecond;
            mt.turretDegreesPerSecond = MTParameter.turretDegreesPerSecond;
            mt.isMobile = isMobile;
            #endregion
            #region 主摄像机设置
            if (isLocalPlayer(_InstanceNetType)) {
                MainCamera.GetComponent<VehicleCamera>().target = referenceManager.MainCameraFollowTarget.transform;
                MainCamera.GetComponent<VehicleCamera>().mainCameraFollowTarget = referenceManager.MainCameraFollowTarget.transform;
                MainCamera.GetComponent<VehicleCamera>().mainCameraGunner = referenceManager.MachineGunFFPoint.transform;
                referenceManager.MachineGunFFPoint.transform.position = referenceManager.FFPoint.transform.position;
                MainCamera.GetComponent<VehicleCamera>().IsMobile = isMobile;
                if (PSParameter.vehicleType == VehicleType.SPG) {
                    MainCamera.GetComponent<VehicleCamera>().isVehicleSPG = true;
                }
            }

            #endregion
            #region 瞄准射线
            if (isLocalPlayer(_InstanceNetType)) {
                RayManager rayManager = TankScript.AddComponent<RayManager>();
                rayManager.Init(FFPoint.transform);
            }
            #endregion
            #region 瞄准镜设置
            if (isLocalPlayer(_InstanceNetType)) {
                TankCrossHair.GetComponent<CrossHair>().MainCamera = MainCamera.GetComponent<Camera>();
                TankCrossHair.GetComponent<CrossHair>().tankCamera = MainCamera.GetComponent<VehicleCamera>();
                TankCrossHair.GetComponent<CrossHair>().FFPoint = EffectStart.transform;
            }
            #endregion
            #region 坦克发射器脚本设置
            TankFire tf = TankScript.GetComponent<TankFire>();
            tf.tankFireParameter = TFParameter;
            tf.tankInitSystem = this;
            tf.MachineGunFFPoint = referenceManager.MachineGunFFPoint.transform;
            tf.FFPoint = referenceManager.FFPoint.transform;
            tf.FireRecoilPoint = referenceManager.FireForceFeedbackPoint.transform;
            tf.FireEffectPoint = referenceManager.EffectStart.transform;
            tf.BulletCountList = BulletCountList;
            tf.GunDym = GunTransform.transform.Find("GunDym").GetComponent<Animator>();
            tf.MainBody = InstanceMesh.transform;
            tf.netType = _InstanceNetType;


            if (PSParameter.vehicleType == VehicleType.SPG) {
                tf.isAutoCaclulateGravity = true;
            }
            #endregion

            #region 引擎音效控制
            EngineSoundModule engineSoundModule = InstanceMesh.AddComponent<EngineSoundModule>();
            engineSoundModule.tankInitSystem = this;
            engineSoundModule.engineData = PTCParameter.vehicleEngineSoundData;
            engineSoundModule.Init(referenceManager.EngineSound.transform);
            #endregion
            GameObject EngineSmoke = Instantiate<GameObject>(Resources.Load<GameObject>("EngineSmoke"));
            EngineSmoke.transform.SetParent(referenceManager.EngineSmoke.transform);
            EngineSmoke.transform.localPosition = Vector3.zero;
            EngineSmoke.transform.localEulerAngles = Vector3.zero;

            //GameObject grounddust = Instantiate(Resources.Load<GameObject>("TrackEffect"));
            //grounddust.transform.SetParent(InstanceMesh..transform);
            //grounddust.transform.localPosition = Vector3.zero;
            //grounddust.transform.localEulerAngles = Vector3.zero;

            VehicleState vehicleState = InstanceMesh.AddComponent<VehicleState>();
            vehicleState.engineSoundModule = engineSoundModule;
            vehicleState.tankTracksController = PTC;
        }

		#region 坦克控制器设置
		PTC.tankInitSystem = this;

		PTC.rightTrackUpperWheels = GetAllChilds (TankTransform.transform.Find ("RightUpperWheel"));
		PTC.leftTrackUpperWheels = GetAllChilds (TankTransform.transform.Find ("LeftUpperWheel"));
		PTC.rightTrackWheels = GetAllChilds (TankTransform.transform.Find ("RightWheel")); 
		PTC.leftTrackWheels = GetAllChilds (TankTransform.transform.Find ("LeftWheel"));
        PTC.leftTrack = referenceManager.LeftTrack;
        PTC.rightTrack = referenceManager.RightTrack;

		//PTC.leftTrackBones = CreateTrack (true, TankTransform.transform, ref PTC);
		//PTC.rightTrackBones = CreateTrack (false, TankTransform.transform, ref PTC);
        PTC.tankTransform = TankTransform.transform;



        PTC.maxAngularVelocity = PTCParameter.MaxAngularVelocity;
		PTC.MaxSpeed = PTCParameter.MaxSpeed;
        PTC.MinSpeed = PTCParameter.MinSpeed;
		PTC.COM = PTCParameter.CenterOfGravity;
		PTC.PushSpeed = PTCParameter.PushSpeed;
		PTC.BackSpeed = PTCParameter.BackSpeed;

		PTC.sidewaysFrictionAsymptoteFactor = PTCParameter.SideWaysFrictionAsymptoteFactor;
		PTC.sidewaysFrictionExtremumFactor = PTCParameter.SideWaysFrictionExtremumFactor;
		PTC.wheelCollider = PTCParameter.TankWheelCollider;

		PTC.wheelsAndBonesAxisSettings = new PlayerTracksController.WheelsAxisSettings ();
		PTC.wheelsAndBonesAxisSettings.bonesPositionAxis = TankTracksController.Axis.Z;
		PTC.wheelsAndBonesAxisSettings.wheelsPositionAxis = TankTracksController.Axis.Z;
		PTC.wheelsAndBonesAxisSettings.wheelsRotationAxis = TankTracksController.Axis.X;

		PTC.accelerationConfiguration = PTCParameter.VAconfigSetting;
		PTC.rotationOnAccelerationConfiguration = PTCParameter.HAconfigSetting;
		PTC.rotationOnStayConfiguration = PTCParameter.HAconfigSetting;


		#endregion

		#region 车库状态
		if (_InstanceNetType == InstanceNetType.GarageTank) {
			PTC.enabled = false;
			TankPhysic.isKinematic = true;

			if (ShowHitBoxInspecter) {
				foreach (HitBox HB in GetComponentsInChildren<HitBox>()) {
					HB.StartCoroutine (HB.ShowArmorInfo ());
				}
			}

			foreach (MeshCollider MC in transform.root.GetComponentsInChildren<MeshCollider>()) {
				if (MC.GetComponent<HitBox> () == null) {
					MC.enabled = false;

				}
			}
			//EventManager.StartListening ("TankInitSystem.InitTankShader", GarageInitTankShaderPreView);
			return;
		}
		#endregion

		if (ExtraTF && !isBot (_InstanceNetType)) {
			for (int i = 0; i < multiTurrets.Length; i++) {
				GameObject TankScriptExtra = Instantiate ((GameObject)Resources.Load ("TankScript"), Vector3.zero, Quaternion.identity) as GameObject;
				TankScriptExtra.transform.name = "TankScriptExtra";
				TankScriptExtra.transform.parent = TankPrefabs.transform;
				TurretController mouseTurretExtra = TankScriptExtra.GetComponent<TurretController> ();
				if (isLocalPlayer (_InstanceNetType)) {
					mouseTurretExtra.target = MainCamera.transform.Find ("CameraTarget");
				}
				Transform ExtraTurret = InstanceMesh.transform.Find (multiTurrets [i].ObjectPath);

				Transform ExtraGun = ExtraTurret.transform.Find ("GunTransform");

				mouseTurretExtra.gun = ExtraGun;
				mouseTurretExtra.Turret = ExtraTurret;
				mouseTurretExtra.DownMaxDegree = multiTurrets [i].MTParameter.DownMaxDegree;
				mouseTurretExtra.UpMaxDegree = multiTurrets [i].MTParameter.UpMaxDegree;
				mouseTurretExtra.maxTurretAngle = multiTurrets [i].MTParameter.maxTurretAngle;
				mouseTurretExtra.gunDegreesPerSecond = multiTurrets [i].MTParameter.gunDegreesPerSecond;
				mouseTurretExtra.turretDegreesPerSecond = multiTurrets [i].MTParameter.turretDegreesPerSecond;
				mouseTurretExtra.isMobile = isMobile;

				TankFire tfExtra = TankScriptExtra.GetComponent<TankFire> ();
				Transform ExtraFFPoint = ExtraGun.Find ("FFPoint");
				Transform ExtraFireEffect = ExtraGun.Find ("FireEffect");

				tfExtra.FFPoint = ExtraFFPoint;
				tfExtra.FireRecoilPoint = FFPoint.transform;
				tfExtra.FireEffectPoint = ExtraFireEffect;
				tfExtra.MainBody = InstanceMesh.transform;
				//tfExtra.FireRecoil = multiTurrets [i].tankFireParameter.FireRecoil;
				//tfExtra.ReloadTime = multiTurrets [i].tankFireParameter.ReloadTime;
				//tfExtra.AmmoCount = multiTurrets [i].tankFireParameter.AmmoCount;
				//tfExtra.HasMahineGun = multiTurrets [i].tankFireParameter.HasMachineGun;
				//tfExtra.muzzleFire = multiTurrets [i].tankFireParameter.MuzzleFire;
				//tfExtra.advanceFireClass = multiTurrets [i].tankFireParameter.advanceFireClass;
				//tfExtra.fireState = multiTurrets [i].tankFireParameter.FireState;
				//tfExtra.netType = InstanceNetType;


			}
		}
		#endregion

		InstanceMesh.AddComponent<Identity> ();
        if(isLocalPlayer(_InstanceNetType)){
            GameEvent.InitPlayer(InstanceMesh.GetComponent<Identity>());
        }
		#region 初始化玩家网络
        if (!GameDataManager.OfflineMode) {
			if (_InstanceNetType == InstanceNetType.GameNetworkOthers) {
				PTC.enableUserInput = false;
				//PTC.AdvanceTrackSystem = false;
				PTC.OnlyReceiveControlActions = true;
			}
			if (_InstanceNetType == InstanceNetType.GameNetworkMaster) {
				PTC.enableUserInput = false;

			}

			if (_InstanceNetType == InstanceNetType.GameNetworkClient) {
				PTC.enableUserInput = true;
				PTC.OnlyReceiveControlActions = true;
			}
			if (isBot (_InstanceNetType)) {
				PTC.enableUserInput = false;
				if (_InstanceNetType == InstanceNetType.GameNetWorkBotClient) {
					PTC.OnlyReceiveControlActions = true;
				} else {
					PTC.OnlyReceiveControlActions = false;
				}
			}

			//_syncGroundVehicle = InstanceMesh.AddComponent<SyncGroundVehicle> ();
			//_syncGroundVehicle.tankInitSystem = this;
			//_syncGroundVehicle.Init (PTC, mt, _InstanceNetType);




		}
		#region 设置离线标识
        if (GameDataManager.OfflineMode) {
			if (_InstanceNetType == InstanceNetType.GameNetWorkOffline) {
				Identity MyIdentity = InstanceMesh.GetComponent<Identity> ();
                MyIdentity.Init (ownerTeam, InstanceNetType.GameNetWorkOffline);
				MyIdentity.SetOwnerInfo ("LocalPlayer", VehicleName, 0);
			}

		}
		#endregion
		#endregion
		if (!isBot (_InstanceNetType)) {
			StartCoroutine (PS.Init ());
		}

        if(GameEventManager.onNewVehicleSpawned!=null)
            GameEventManager.onNewVehicleSpawned(this);

        if (onVehicleLoaded != null)
            onVehicleLoaded();

	}
    private void OnDestroy()
    {
        if (GameEventManager.onNewVehicleDestroyed != null)
            GameEventManager.onNewVehicleDestroyed(this);
    }

    public Transform[] GetAllChilds (Transform t)
	{
		int ChildCount = t.childCount;
		List<Transform> TempeoraryT = new List<Transform> ();
		for (int i = 0; i < ChildCount; i++) {
			TempeoraryT.Add (t.GetChild (i));
		}
		return TempeoraryT.ToArray ();
	}









	public override void OnPhotonInstantiate ()
	{
		base.OnPhotonInstantiate ();
		InitTankInitSystem ();
	}







}
	
