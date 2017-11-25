/*
                   _ooOoo_
                  o8888888o
                  88" . "88
                  (| -_- |)
                  O\  =  /O
               ____/`---'\____
             .'  \\|     |//  `.
            /  \\|||  :  |||//  \
           /  _||||| -:- |||||-  \
           |   | \\\  -  /// |   |
           | \_|  ''\---/''  |   |
           \  .-\__  `-`  ___/-. /
         ___`. .'  /--.--\  `. . __
      ."" '<  `.___\_<|>_/___.'  >'"".
     | | :  `- \`.;`\ _ /`;.`/ - ` : | |
     \  \ `-.   \_ __\ /__ _/   .-` /  /
======`-.____`-.___\_____/___.-`____.-'======
                   `=---='
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         佛祖保佑       永无BUG
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerState : BasePlayerState {
    public PlayerStateParameter playerStateParameter;

	public UserUI userUI = new UserUI();

	public InstanceNetType netType;
	
	[HideInInspector]
	public TankInitSystem tankInitSystem;

	public GameObject Turret,TankScript;
	public Object[] DiedDestoryObjects;

	int MaxHealth;

	[HideInInspector]
	public PlayerTracksController PTC;

	ParticleSystem[] EngineEffect;

	public Transform[] T_EngineEffects;

	Transform TankUIRoot;


	[HideInInspector]
	/// <summary>
	/// Mine is set in Start(); When in Net if it is owned by player return true in single mode it always returns true
	/// </summary>
	public bool Mine ;
	[HideInInspector]
	public bool IsMobile=true,isEnemy_B=false,IsOnline =false;

	public bool showPlayerUI =false;
	public int ViewRange =250;
	public float TrackFixTime = 8;
	#region 防止重复计算死亡
	bool SendDeadMesssageToTankFire = false;
	#endregion
	bool GodMode = true;

	[HideInInspector]
	public bool BotInSingleMode =false;


	[HideInInspector]
	public EngineType engineType;

	public List<GameObject> TankFires = new List<GameObject>();




	public enum UIEvent{
		Repair,
		Capture,
		Restore,
		Message
	}

	public IEnumerator Init(){
		PTC = GetComponent<PlayerTracksController> ();
		MaxHealth = Health;
		#region 多人模式
		if(netType==InstanceNetType.GameNetworkClient)
		{
			Mine = true;
		}
		#endregion
		#region 单人模式
		else if(netType == InstanceNetType.GameNetWorkOffline){
			Mine = true;
		}
		#endregion
		Invoke("SwtichToNormalMode",5);

		//InitAIObstacle ();

		if (netType == InstanceNetType.GameNetworkMaster) {
			PTC.SetWheels (true,false,false);
			InitExtraTankFires(false);
			transform.parent.GetComponentInChildren<TankFire> ().InitSystem ();
		}
		if (netType == InstanceNetType.GameNetworkOthers) {
			PTC.SetWheels (false,false,true);
			InitExtraTankFires(false);
			TankScript.GetComponent<TankFire>().InitSystem ();
		}
		yield return new WaitForEndOfFrame();


		if (netType == InstanceNetType.GameNetworkClient||netType == InstanceNetType.GameNetWorkOffline) {
			MainCamera =this.transform.root.GetComponentInChildren<Camera>();
			PTC.SetWheels (true,true,true);

			StartCoroutine(InGameVoiceSoundManager.Self.Init (TankName,(InGameVoiceSoundManagerReturnValue)=>{
                InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Start_Battle,InGameVoiceSoundManager.InGameVoiceLanguage(playerStateParameter.nationType));
			}));


			#region UI初始化
			StartCoroutine(InitTankUISystem(ReturnValue=>{
				TankUISystemInit();
				InitExtraTankFires(true);
				UpdateHealthBar();

				TankScript.GetComponent<TankFire>().isPlayerControl = true;
				TankScript.GetComponent<TankFire>().InitSystem ();
				TankScript.GetComponent<TankFire>().OnAmmoOut ();
				TankScript.GetComponent<TankFire> ().ReSetAmmoSprits ();
			}));
			#endregion

			StartCoroutine (CheckIfPlayerRollOver ());

		}

	}
	void InitExtraTankFires(bool LocalPlayer){
		if (TankFires.Count == 0)
			return;
		for (int i = 0; i < TankFires.Count; i++) {
			TankFires [i].GetComponent<TankFire> ().ExtraTurret = true;
			if (LocalPlayer) {
				TankFires [i].GetComponent<TankFire> ().isPlayerControl = true;
			}
			TankFires [i].GetComponent<TankFire> ().InitSystem ();
			TankFires [i].GetComponent<TankFire> ().OnAmmoOut();
		}
	}
	void UpdateHealthBar(){
		if (userUI.HealthUI) {
			userUI.HealthUI.text = Health.ToString ();
		}
	}
	void Update(){
		if (netType == InstanceNetType.GameNetworkClient||netType == InstanceNetType.GameNetworkOthers||netType == InstanceNetType.GameNetWorkOffline) {
			if (Mine) {
				ScreenShot ();
				SwtichShowUI ();


						
				//Health = Mathf.Clamp (Health, -1, 5000);




			}

	
		}
	}

	public static Camera MainCamera;


	public void UpdataGameNote(string Note,bool ForceOpen =false){
		if (Mine&&(uGUI_QualitySetting.B_inBattleMessage||ForceOpen)) {
			CancelInvoke ("CloseGameNote");
			UpdateEventMessage (UIEvent.Message, Note);
			Invoke ("CloseGameNote", 1.2f);
		}
	}

	public void CloseGameNote(){
		ActiveEventMessage (UIEvent.Message, false);
	}
	public void UpdateDamageBar(int Damages,int Destroys){
		if (Damages > 1000) {
			userUI.DamageToolBar.text = ((double)Damages / 1000).ToString ("0.00") + "k";
		} else {
			userUI.DamageToolBar.text = Damages.ToString();
		}
		userUI.DestroyToolBar.text = Destroys.ToString ();
	}
	void TankUISystemInit(){
		if (transform.root.GetComponentInChildren<Canvas> ()) {
			TankUIRoot = transform.root.GetComponentInChildren<Canvas> ().transform;
		}
	
		userUI = new UserUI();
		userUI.HealthUI = TankUIRoot.Find("Middle/Health").GetComponent<Text>();
		TankFire TF = TankScript.GetComponent<TankFire> ();
		TF.AmmoInfos = new Text[] {
			TankUIRoot.Find("Buttom/AP/AmmoCount").GetComponent<Text>(),
			TankUIRoot.Find("Buttom/HE/AmmoCount").GetComponent<Text>(),
			TankUIRoot.Find("Buttom/APCR/AmmoCount").GetComponent<Text>(),
		};
		TF.AmmoTextures = new Image[]{
			TankUIRoot.Find("Buttom/AP/AmmoImage").GetComponent<Image>(),
			TankUIRoot.Find("Buttom/HE/AmmoImage").GetComponent<Image>(),
			TankUIRoot.Find("Buttom/APCR/AmmoImage").GetComponent<Image>(),

		};

		TankUIRoot.Find("Buttom/AP/AmmoImage").GetComponent<Button>().onClick.AddListener (TankScript.GetComponent<TankFire>().SelectAp);		
		TankUIRoot.Find("Buttom/HE/AmmoImage").GetComponent<Button>().onClick.AddListener (TankScript.GetComponent<TankFire>().SelectHE);		
		TankUIRoot.Find("Buttom/APCR/AmmoImage").GetComponent<Button>().onClick.AddListener (TankScript.GetComponent<TankFire>().SelectApcr);		

		TF.ReloadTimeCountDownInfo = TankUIRoot.Find("Middle/Reloading").GetComponent<Text>();
		//TF.BattleLog = TankUIRoot.FindChild("Buttom/Data/Damage").GetComponent<Text>();
		transform.root.GetComponentInChildren<VehicleCamera>().FireCross_Texture = TankUIRoot.Find("FireCross").GetComponent<Image>();
		transform.root.GetComponentInChildren<VehicleCamera>().CrossHair = TankUIRoot.Find("Middle/CrossHair").GetComponent<Image>();
		transform.root.GetComponentInChildren<VehicleCamera>().FinalPostion = TankUIRoot.Find("FireCross/FinalPostion").GetComponent<Image>();
		transform.root.GetComponentInChildren<VehicleCamera>().SPGFinalPosition = TankUIRoot.Find("SPGFinal").GetComponent<Image>();

		//transform.root.GetComponentInChildren<TankCamera>().CompactMirror = TankUIRoot.FindChild("FireCross/CompactMirror").GetComponent<Image>();

		TankUIRoot.Find("FireCross").gameObject.SetActive(false);

		userUI.TeamA = TankUIRoot.Find("Middle/TeamList/TeamA").GetComponent<Text>();
		userUI.TeamB = TankUIRoot.Find("Middle/TeamList/TeamB").GetComponent<Text>();

		userUI.FireState = TankUIRoot.Find ("Middle/FireState").GetComponent<Text> ();


		TF.FireStateInfo = userUI.FireState;
		userUI.DamageToolBar = TankUIRoot.Find ("DamageTool/Data/Damage").GetComponent<Text> ();
		userUI.DestroyToolBar = TankUIRoot.Find ("DamageTool/Data/Destroy").GetComponent<Text> ();
		userUI.ReloadBar = TankUIRoot.Find ("Middle/ReloadBar").GetComponent<Image> ();
		userUI.RightBarFill = TankUIRoot.Find ("Middle/RightBarFill").GetComponent<Image> ();

		userUI.EventCapture = TankUIRoot.Find ("EventState/Capture");
		userUI.CaptureProgress = userUI.EventCapture.Find("Progress").GetComponent<Text>();

		userUI.EventRestore = TankUIRoot.Find ("EventState/Restore");
		userUI.RestoreProgress = userUI.EventRestore.Find("Progress").GetComponent<Text>();

		userUI.EventRepair = TankUIRoot.Find ("EventState/Repair");
		userUI.RepairProgress = userUI.EventRepair.Find("Progress").GetComponent<Text>();

		userUI.Message  = TankUIRoot.Find ("EventState/Message");
		userUI.MessageContent = userUI.Message.Find("Content").GetComponent<Text>();

//		TankUI.Turret = TankUIRoot.FindChild ("SmallTank/TankBody/Turret").GetComponent<Image> ();
//		TankUI.TankBody = TankUIRoot.FindChild ("SmallTank/TankBody/").GetComponent<Image> ();
//		TankUI.DamageBar = TankUIRoot.FindChild ("SmallTank/HealthBar/DamageBar").GetComponent<Image> ();
//		TankUI.MiniMapCamera = TankUIRoot.FindChild ("MiniMapCamera").GetComponent<Camera> ();

		TF.MainGunFireLoading = TankUIRoot.Find ("Middle/Android/Fire/Reload").GetComponent<Image> ();
			

		//------------------------------- 手机平台 坦克NGUI  初始化
		if (IsMobile){
				userUI.MG = TankUIRoot.Find("Middle/Android/MachineGun").GetComponent<EventTrigger>();
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Forward").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.Forward,EventTriggerType.PointerDown));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Forward").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.CancelForward,EventTriggerType.PointerUp));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Left").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.Left,EventTriggerType.PointerDown));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Left").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.CancelLeft,EventTriggerType.PointerUp));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Right").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.Right,EventTriggerType.PointerDown));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Right").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.CancelRight,EventTriggerType.PointerUp));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Back").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.Back,EventTriggerType.PointerDown));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/Back").GetComponent<EventTrigger>().triggers.Add(MyEntry(PTC.CancelBack,EventTriggerType.PointerUp));
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/AddRPM").GetComponent<Button>().onClick.AddListener(PTC.AddRPM);
				TankUIRoot.Find("Middle/Android/KeyBoardControllor/DownRPM").GetComponent<Button>().onClick.AddListener(PTC.DownRPM);
				//Debug.Log("EventTrigger For KeyBoardControllor Done"); 
				TankUIRoot.Find("Middle/Android/Fire").gameObject.AddComponent<EventTrigger>();

				VehicleCamera tankCamera = transform.GetComponentInChildren<VehicleCamera>();


				TankUIRoot.Find("Middle/Android/OpenFireCross").GetComponent<Button>().onClick.AddListener(tankCamera.OpenFireCross);
				//Debug.Log("EventTrigger For OpenFireCross Done"); 

				//单机模式开启射击辅助
//				if(!IsOnline||PlayerPrefs.GetString("AllowAutoAim")=="true")
//					TankUIRoot.FindChild("Android/ToggleAutoAim").GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(TankScript.GetComponent<TankFire>(),"SwtichAim"));
//				else 
//					TankUIRoot.FindChild("Android/ToggleAutoAim").gameObject.SetActive(false);

				TankUIRoot.Find("Middle/Android/Fire").GetComponent<EventTrigger>().triggers.Add(MyEntry(TF.Fire,EventTriggerType.PointerClick));

				TF.FireButton = TankUIRoot.Find("Middle/Android/Fire").GetComponent<Button>();

				//TankUIRoot.FindChild("Middle/Android/Fire").GetComponent<EventTrigger>().triggers.Add(MyEntry(TF.Fire,EventTriggerType.PointerEnter));

				foreach (GameObject extraTankFire in TankFires) {
					TankUIRoot.Find("Middle/Android/Fire").GetComponent<EventTrigger>().triggers.Add(MyEntry(extraTankFire.GetComponent<TankFire>().Fire,EventTriggerType.PointerClick));
					//TankUIRoot.FindChild("Middle/Android/Fire").GetComponent<EventTrigger>().triggers.Add(MyEntry(extraTankFire.GetComponent<TankFire>().Fire,EventTriggerType.PointerEnter));
				}

            if(playerStateParameter.vehicleType == VehicleType.SPG){
                TankUIRoot.Find("Middle/Android/AutoAim").GetComponent<Button>().onClick.AddListener(TF.SetSPGAimCircleTarget);

            }
            else{
                TankUIRoot.Find("Middle/Android/AutoAim").GetComponent<Button>().onClick.AddListener(TF.SwtichAim);

            }

				//Debug.Log("EventTrigger For OpenFireCross Fire"); 

				userUI.CameraJoyStick = TankUIRoot.Find("Middle/Android/CameraJoyStick/").GetComponent<Joystick>();

//				FingersScript.Instance.PassThroughObjects = new List<GameObject>();
//				FingersScript.Instance.PassThroughObjects.Add(TankUIRoot.FindChild("Middle/Android/OpenFireCross").gameObject);
//				FingersScript.Instance.PassThroughObjects.Add(TankUIRoot.FindChild("Middle/Android/Fire").gameObject);
//				FingersScript.Instance.PassThroughObjects.Add(TankUIRoot.FindChild("Middle/Android/AutoAim").gameObject);
//				FingersScript.Instance.PassThroughObjects.Add(TankUI.MG.gameObject);

				if (uGUI_ControllorSetting.B_EnableCameraJoyStick) {
					userUI.CameraJoyStick.OnJoystickMovement += tankCamera.On_JoystickMove;
					userUI.CameraJoyStick.OnStartJoystickMovement += tankCamera.On_JoystickMove;
					userUI.CameraJoyStick.OnEndJoystickMovement += tankCamera.On_JoystickStop;
				} else {
					userUI.CameraJoyStick.gameObject.SetActive (false);
				}
		}
		else
			TankUIRoot.Find ("Middle/Android/").gameObject.SetActive(false);

		TankUIRoot.SetParent(null);

		GetComponent<PlayerTracksController>().InitUISystemForPTC(TankUIRoot);
	} 
	public void UpdateEventMessage(UIEvent _UIEvent,string Progress){
		ActiveEventMessage (_UIEvent, true);
		switch (_UIEvent) {
			case UIEvent.Capture:
				userUI.CaptureProgress.text = Progress;
				break;
			case UIEvent.Repair:
				userUI.RepairProgress.text = Progress;
				break;
			case UIEvent.Restore:
				userUI.RestoreProgress.text = Progress;
				break;
			case UIEvent.Message:
				userUI.MessageContent.text = Progress;
				break;
		}
	}
	public void ActiveEventMessage(UIEvent _UIEvent,bool _isActive){
		switch (_UIEvent) {
			case UIEvent.Capture:
				userUI.EventCapture.gameObject.SetActive (_isActive);
				break;
			case UIEvent.Repair:
				userUI.EventRepair.gameObject.SetActive (_isActive);
				break;
			case UIEvent.Restore:
				userUI.EventRestore.gameObject.SetActive (_isActive);
				break;
			case UIEvent.Message:
				userUI.Message.gameObject.SetActive (_isActive);
				break;
		}
	}
	#region 被击中音效
	public override void  OnNotBreakDown(){
		//base.OnNotBreakDown ();

        if (GameDataManager.OfflineMode) {
			OnServerOnNotBreakDown ();
		}
	}

	void OnServerOnNotBreakDown(){
		PoolManager.CreateObject ("ap_armor_not_pierce_main",transform.position,transform.eulerAngles);
	}

	public override void OnRicochet(){
		//base.OnRicochet ();

        if (GameDataManager.OfflineMode) {
			OnServerOnRicochet ();
		}
	}
	void OnServerOnRicochet(){
		PoolManager.CreateObject ("apcr_ricochet_main",transform.position,transform.eulerAngles);
	}
	#endregion

	#region 接收来自HitBox的信息
	public override void ApplyHitBoxDamage(int Damage,float Degree,BaseFireSystem Owner,HitBox _hitBox){
		base.ApplyHitBoxDamage (Damage,Degree,Owner,_hitBox);

		Debug.Log("ApplyHitBoxDamage");


		
//		GameObject owner = (GameObject)HitInfos[1];
//
		if (GodMode) {
			_hitBox.BasePlayerStateGodModeCallBack ();
			return;
		}
		if (GetComponent<Identity> ().identity == Owner.transform.root.GetComponentInChildren<Identity> ().identity) {
			_hitBox.BasePlayerStateHitFriendCallBack ();
			return;
		} else {
			_hitBox.BasePlayerStateHittedCallBack (Damage,Health);
		}

  
		Health -= Damage;

		if (Health <= 0) {
			OnDeadActionCalled ();
		}

		if (Health <= 0&&!SendDeadMesssageToTankFire) {
			SendDeadMesssageToTankFire = true;
			Owner.KillTank(TankName);
			Owner.AddDamage(Damage,TankName);
		}

		if(SendDeadMesssageToTankFire==false){
			Owner.AddDamage(Damage,TankName);
		}
		RepairTankCountDown = 0;
		CaptureCountDown = 0;

		UpdateHealthBar ();
	}
	#endregion






	bool OnEffect =false;
	int hitTime = 1;
	void OutCollsionEffect(){
		OnEffect = false;
	}
	void OnCollisionEnter(Collision cl){
		if(IsOnline)
			if(Mine==false)
				return;
		
		if(!OnEffect)
			if(cl.contacts.Length>0){
				for (int i =0;i<cl.contacts.Length;i++)
					if(i<hitTime)
						Destroy(Instantiate (Resources.Load ("HitEffect"), cl.contacts[i].point, transform.rotation),0.6f);
					
				Invoke("OutCollsionEffect",0.6f);
			}
	}
	bool InUpdateAmmo = false;
	public IEnumerator UpdateAmmo(GameObject AmmoReloadPlace){
		if (InUpdateAmmo)
			yield break;

		InUpdateAmmo = true;

		int CountDown = 5;
		for(CountDown =0;CountDown<=5;CountDown++){
			if(Vector3.Distance(this.transform.position,AmmoReloadPlace.transform.position)>25){
				InUpdateAmmo = false;
				#if ClientCode
				ActiveEventMessage (UIEvent.Restore, false);
				#endif
				yield break;	
			}
			#if ClientCode
			UpdateEventMessage(UIEvent.Restore,string.Format("{0}/5",CountDown.ToString()));
			#endif
			yield return new WaitForSeconds (1);
		}
		InUpdateAmmo = false;
		#if ClientCode
		ActiveEventMessage (UIEvent.Restore, false);
 
		#endif

		#if ServerSide
		transform.root.FindChild("TankScript").GetComponent<TankFire>().UpdateAmmo();
		GetComponent<PhotonView> ().RPC ("CallClientonUpdateAmmo", Util.FindPhotonPlayer (tankInitSystem.PlayerID));
		#endif
	}


	double LastFixTankTime=0;

	bool InReparingTank= false;
	public int RepairTankCountDown = 15;

	public IEnumerator RepairTank(GameObject TankRepairPlace){
		if (InReparingTank)
			yield break;
		
		InReparingTank = true;

		double CurrentTime = 0;

        if (GameDataManager.OfflineMode) {
			CurrentTime = (double)Time.time;
		} 
		if (CurrentTime - LastFixTankTime < 120) {
			//UpdataGameNote(string.Format(uGUI_Localsize.GetContent("FixVehicleWait"),(CurrentTime- LastFixTankTime).ToString()));
			yield break;
		}

		for(RepairTankCountDown =0;RepairTankCountDown<=15;RepairTankCountDown++){
			if(Vector3.Distance(this.transform.position,TankRepairPlace.transform.position)>25){
				InReparingTank = false;
				#if ClientCode
				ActiveEventMessage (UIEvent.Repair, false);
				#endif
				yield break;	
			}
			#if ClientCode
			UpdateEventMessage(UIEvent.Repair,string.Format("{0}/15",RepairTankCountDown.ToString()));
			#endif
			yield return new WaitForSeconds (1);
		}
		InReparingTank = false;
		LastFixTankTime = CurrentTime;

		#if ClientCode
		ActiveEventMessage (UIEvent.Repair, false);
        if (GameDataManager.OfflineMode) {
			CallClientonRepairTank();
		}
		#endif
		#if ServerSide
		Health = MaxHealth;
		GetComponent<PhotonView> ().RPC ("CallClientonRepairTank", Util.FindPhotonPlayer (tankInitSystem.PlayerID));
		#endif
	}
	void CallClientonRepairTank(){
		Health = MaxHealth;
	}

	bool InCapture = false;

	public int CaptureCountDown = 20;

	public IEnumerator Capture(GameObject CapturePlace){
		if(InCapture)
			yield break;
		InCapture = true;

		for(CaptureCountDown =0;CaptureCountDown<=20;CaptureCountDown++){
			if(Vector3.Distance(this.transform.position,CapturePlace.transform.position)>25){
				InCapture = false;
				#if ClientCode
				ActiveEventMessage (UIEvent.Capture, false);
				#endif
				yield break;	
			}
			yield return new WaitForSeconds (1);
			#if ClientCode
			UpdateEventMessage(UIEvent.Capture,string.Format("{0}/20",CaptureCountDown.ToString()));
			#endif
		}
		InCapture = false;

		#if ClientCode
		ActiveEventMessage (UIEvent.Capture, false);
		#else 
		if (GetComponent<Identity> ().identity == PunTeams.Team.red) {
			GameNetwork.Instance.TeamAScore += 10*GameNetwork.Instance.BattleRank;
			GameNetwork.Instance.TeamBScore -=  10*GameNetwork.Instance.BattleRank;
		} else {
			GameNetwork.Instance.TeamBScore +=  10*GameNetwork.Instance.BattleRank;
			GameNetwork.Instance.TeamAScore -=  10*GameNetwork.Instance.BattleRank;
		}

		GameNetwork.Instance.UpdateClient ();
		#endif
		StartCoroutine (Capture (CapturePlace));
	}

	bool ShowUIRoot= true;
	public static ArrayList UIs = new ArrayList();
	void SwtichShowUI(){
		if(Input.GetKeyDown(KeyCode.V))
		if(ShowUIRoot){
			foreach (Canvas temp in GameObject.FindObjectsOfType<Canvas>()){
				temp.gameObject.SetActive(false);

				if(UIs.Contains(temp.gameObject) == false){
					UIs.Add(temp.gameObject);
				}
			}

			UpdataGameNote("关闭UI!");

			ShowUIRoot = false;

		}
		else {
			foreach (GameObject temp in UIs){
				if(temp){
					temp.SetActive(true);
				}
			}

			UpdataGameNote("显示UI!");

			ShowUIRoot = true;
		}

	}
	void ScreenShot(){
		if(Input.GetKeyDown(KeyCode.Home)||Input.GetKeyDown(KeyCode.F9)){
			ScreenCapture.CaptureScreenshot(PlayerPrefs.GetInt("CaptureScreenshot").ToString()+".png");
			PlayerPrefs.SetInt("CaptureScreenshot",PlayerPrefs.GetInt("CaptureScreenshot")+1);
			UpdataGameNote("Save:"+PlayerPrefs.GetInt("CaptureScreenshot").ToString()+".png");
		}
	}


	void SwtichToNormalMode(){
		GodMode = false;
	}


	public IEnumerator InitTankEffects(){
		ResourceRequest ResourceTask = null;
		Object asset = null;
		#region Set up TrackEffect /Dead / engine Particle engine sound.
//		ResourceTask = Resources.LoadAsync("TrackEffect");
//		yield return ResourceTask;
//		asset = ResourceTask.asset;
//		LeftTrackEffect = ((GameObject)Instantiate(asset)).GetComponent<ParticleSystem>();
//		LeftTrackEffect.transform.SetParent(this.transform.FindChild("LeftTrackEffect"));
//		LeftTrackEffect.transform.localPosition = Vector3.zero;
//		LeftTrackEffect.transform.eulerAngles = transform.eulerAngles+new Vector3(0,180,0);
//
//		RightTrackEffect = ((GameObject)Instantiate(asset)).GetComponent<ParticleSystem>();
//		RightTrackEffect.transform.SetParent(this.transform.FindChild("RightTrackEffect"));
//		RightTrackEffect.transform.localPosition = Vector3.zero;
//		RightTrackEffect.transform.eulerAngles = transform.eulerAngles+new Vector3(0,180,0);
//
		ResourceTask = Resources.LoadAsync("EngineSmoke");
		yield return ResourceTask;
		asset = ResourceTask.asset;
//
		EngineEffect = new ParticleSystem[T_EngineEffects.Length];
		for(int i=0;i<EngineEffect.Length;i++){
			EngineEffect[i] = ((GameObject)Instantiate(asset)).GetComponent<ParticleSystem>();
			EngineEffect[i].transform.SetParent(T_EngineEffects[i]);
			EngineEffect[i].transform.localPosition = Vector3.zero;
			EngineEffect[i].transform.localEulerAngles = Vector3.zero;
		}
			
		#endregion
		yield break;

	}
	public static  EventTrigger.Entry MyEntry(UnityAction<BaseEventData> callback,EventTriggerType EventTriggertype){
			EventTrigger.Entry entry = new EventTrigger.Entry();     
			entry.eventID = EventTriggertype;   
			entry.callback = new EventTrigger.TriggerEvent();                                       //设置回调函数		
			entry.callback.AddListener(callback);   
		return entry;
	}

	IEnumerator InitTankUISystem(System.Action<bool> onFinish){
		ResourceRequest resourceRequest = Resources.LoadAsync ("TankUISystem");
		yield return resourceRequest;

		GameObject UISystemObject = (GameObject)Instantiate(resourceRequest.asset,Vector3.zero,Quaternion.identity);

		tankInitSystem.dontDestroyManager.Add (UISystemObject);

		UISystemObject.transform.parent =this.transform.root;
		UISystemObject.name = "UI Root";
		onFinish (true);
	}
	void OnDeadActionCalled(){
		Debug.Log ("OnDeadActionCalled");

		if (netType != InstanceNetType.GameNetworkMaster) {
			PTC.enableUserInput = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

			if (netType == InstanceNetType.GameNetworkClient||netType == InstanceNetType.GameNetWorkOffline) {
				PTC.enableUserInput =false;
				PTC.UpdateWheels(0,0);
				PTC.enabled =false;


				Destroy(TankUIRoot.gameObject);

				//GameNetwork.Instance.OnClientPlayerDead();
				//transform.root.GetComponentInChildren<Camera>().gameObject.AddComponent<Grayscale>().shader = Shader.Find("Hidden/Grayscale Effect");

                InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Vehicle_Destroyed,InGameVoiceSoundManager.InGameVoiceLanguage(playerStateParameter.nationType));

                PoolManager.CreateObject("DestroyEffect", transform.position, transform.eulerAngles);


			}

			if(netType == InstanceNetType.GameNetworkOthers){
				GetComponent<Rigidbody>().isKinematic =true;
			}

			#region 炮塔飞天
			if(Turret.GetComponent<MeshRenderer>() !=null){
				if(Turret.GetComponent<Rigidbody>()==null)
					Turret.AddComponent<Rigidbody>();

				Turret.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity*0.1f;
				Turret.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up*-500,Turret.transform.position);
				Turret.GetComponent<Rigidbody>().drag = 0.1f;
				Turret.GetComponent<Rigidbody>().mass =1000;
				Turret.GetComponent<Rigidbody>().useGravity =true;
			}
			#endregion



		} 
		else {
			//GameNetwork.Instance.OnPlayerDead(Util.FindPhotonPlayer (tankInitSystem.PlayerID),tankInitSystem.VehicleName);

			PTC.SwitchRPM = 0;
			PTC.UpdateWheels(0,0);
			PTC.enabled =false;

			#region 爆炸特效
			PoolManagerSpawnModule.CreateObject("DestroyEffect",transform.position,transform.eulerAngles);
			#endregion
		}
		
		foreach(Object temp in DiedDestoryObjects){
			if(temp){
				foreach (HitBox tempHB in ((GameObject)temp).GetComponentsInChildren<HitBox>()){
					if(tempHB){
						tempHB.enabled =false;
					}
				}
			}
		}

		transform.tag = "Untagged";
        if (TankScript.GetComponent<TankFire>().tankFireParameter.HasMachineGun){
			TankScript.GetComponent<TankFire>().GetComponent<MachineGun>().CancelInvoke ();
		}
		TankScript.GetComponent<TankFire>().NowTime =0;
		TankScript.GetComponent<TankFire>().enabled =false;
		TankScript.GetComponent<TurretController>().enabled =false;

		Destroy (GetComponent<Identity> ());

        GameEventManager.onPlayerVehicleDestroyed();

        if (GameDataManager.OfflineMode){
			Destroy(transform.root.gameObject,5f);

		}

	}



	void ApplyOnHitSound(float Damage){
		if (Damage <= MaxHealth * 0.1f) {
			PoolManager.CreateObject ("ap_critical_hit_small",transform.position,transform.eulerAngles);
		} else if (Damage > MaxHealth * 0.1f && Damage < Health * 0.5f) {
			PoolManager.CreateObject ("ap_critical_hit_medium",transform.position,transform.eulerAngles);
		} else {
			PoolManager.CreateObject ("ap_critical_hit_huge",transform.position,transform.eulerAngles);
		}
	}

	public void GetOwnerInfo(){
		GetComponent<Identity> ().SetOwnerInfo (tankInitSystem.PlayerName, tankInitSystem.VehicleName,Health);
	}



	public void UpdateOtherPlayerHealthBar(float fillAmount){
		if (Health > 0&&userUI.RightBarFill !=null) {
			userUI.RightBarFill.fillAmount = fillAmount;
		}
	}
	int PlayerRollOverTime = 0;

	public IEnumerator CheckIfPlayerRollOver(){
		while (true) {
			if (PTC.WheelDamper <0.2f) {
				PlayerRollOverTime += 1;
			} 
			else {
				PlayerRollOverTime = 0;
			}

			if (PlayerRollOverTime > 2) {
				UpdataGameNote (string.Format(uGUI_Localsize.GetContent ("VehicleRoll"),PlayerRollOverTime.ToString()),true);
				if (PlayerRollOverTime > 30) {
                    if (GameDataManager.OfflineMode) {
                        OnDeadActionCalled();
						//CalledRemoteDeathEvent ();
					} else {
						//photonView.RPC ("CalledRemoteDeathEvent", PhotonNetwork.masterClient);
					}

				}
			}
			yield return new WaitForSeconds (1);
		}
	}
}
