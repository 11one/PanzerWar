//#define ServerSide

using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MachineGun : MonoBehaviour
{
	public float ReloadTime = 0.25f, NowTime;


	public Transform MachineGunFFPoint;

	private GameObject Bullet;

	private float SmokeAfter = 0.6f;
	private float SmokeMax = 2;
	private float SmokeIncrement = 0.35f;
	private SmokePlume MuzzlePlume;

	private GameObject MuzzleFlashObject;
	private float FireTime = 0;

	private float _smoke;
	private GameObject Forward;

	private InstanceNetType netType;
    private TurretController turretController;
	private TankFire tankFire;

	public void  Init (InstanceNetType nettype, TurretController mouseturret, TankFire tankfire)
	{
		if (BaseInitSystem.isBot (nettype)) {
			this.enabled = false;
			return;
		}
		
		turretController = mouseturret;
		netType = nettype;
		tankFire = tankfire;
		if(!BaseInitSystem.isBot (nettype)){
			if (transform.root.GetComponentInChildren<PlayerState> ().IsMobile == true &&transform.root.GetComponentInChildren<PlayerState> ().Mine) {
				transform.root.GetComponentInChildren<PlayerState> ().userUI.MG.triggers.Add (PlayerState.MyEntry (SwtichStartFire, UnityEngine.EventSystems.EventTriggerType.BeginDrag));
				transform.root.GetComponentInChildren<PlayerState> ().userUI.MG.triggers.Add (PlayerState.MyEntry (SwtichStartFire, UnityEngine.EventSystems.EventTriggerType.Drag));
				transform.root.GetComponentInChildren<PlayerState> ().userUI.MG.triggers.Add (PlayerState.MyEntry (SwtichEndFire, UnityEngine.EventSystems.EventTriggerType.PointerUp));
			}
		}


		Bullet = (GameObject)Resources.Load ("Ammos/Ammo_13mm");
		GameObject temp = (GameObject)Instantiate (Resources.Load ("SmokePlume"), MachineGunFFPoint.position, MachineGunFFPoint.rotation);
		MuzzlePlume = temp.GetComponent<SmokePlume> ();
		MuzzlePlume.transform.parent = MachineGunFFPoint;
		MuzzleFlashObject = MuzzlePlume.transform.Find ("MuzzleFlashLights").gameObject;
		MuzzleFlashObject.gameObject.SetActive (false);

		Forward = new GameObject ("Forward");
		Forward.transform.SetParent (MachineGunFFPoint.parent);
		Forward.transform.localPosition = Vector3.zero;
		Forward.transform.eulerAngles = MachineGunFFPoint.eulerAngles;

		GameObject AmmoInfo = Instantiate (Bullet);
		AmmoInfo.GetComponent<BulletScript> ().bulletState = BulletState.Info;
		AmmoInfo.transform.position = Vector3.zero;
	}

	public void SwtichStartFire (BaseEventData arg0)
	{
		OnFire ();
	}

	public void SwtichEndFire (BaseEventData arg0)
	{
		OnIdle ();
	}

	bool MachineAvailable = true;

	void Update ()
	{
		if (turretController.target == null)
			return;
		
		NowTime += Time.deltaTime;

		#if ClientCode
		float TurretAngle = Vector3.Angle (Forward.transform.forward, turretController.target.position - Forward.transform.position);
		if (TurretAngle < 25) {
			MachineGunFFPoint.transform.LookAt (turretController.target.transform.position);
			MachineAvailable = true;
		} else {
			MachineAvailable = false;
		}


		MuzzlePlume.Emit = _smoke > SmokeAfter;
		_smoke -= Time.deltaTime;
		if (_smoke > SmokeMax)
			_smoke = SmokeMax;

		if (_smoke < 0)
			_smoke = 0;


		if (cInput.GetKeyDown ("MachineGun")) {
			OnFire ();
		}
		if (cInput.GetKeyUp ("MachineGun")) {
			OnIdle ();
		}

		if (!MachineAvailable) {
			if (LocalmachinegunFireState == MachineGunFireState.OnFiring) {
				OnIdle ();
			}
		}
		#endif

		#if ServerSide
		MachineGunFFPoint.transform.LookAt (mouseTurret.target.transform.position);
		#endif

	}

	public enum MachineGunFireState
	{
		Idle = 0,
		OnFiring = 1
	}

	public MachineGunFireState LocalmachinegunFireState = MachineGunFireState.Idle;


	 void ChangeFireActions (MachineGunFireState machinegunFireState)
	{
		if (machinegunFireState == MachineGunFireState.Idle) {
			CancelInvoke ("FireMethod");
		} else {
			InvokeRepeating ("FireMethod", 0, 0.2f);
		}
	}


	public void OnFire ()
	{
		if (MachineAvailable) {
			if (LocalmachinegunFireState == MachineGunFireState.OnFiring)
				return;
		} else {
			return;
		}

		if (netType == InstanceNetType.GameNetworkClient && MachineAvailable) {
			LocalmachinegunFireState = MachineGunFireState.OnFiring;
		}
		
		if ((netType == InstanceNetType.GameNetworkBotOffline||netType == InstanceNetType.GameNetWorkOffline)&& MachineAvailable) {
			ChangeFireActions (MachineGunFireState.OnFiring);
			LocalmachinegunFireState = MachineGunFireState.OnFiring;
		}
		#if ServerSide
		if(netType == InstanceNetType.GameNetWorkBotMaster&&MachineAvailable){
			OnFireStateChanged(MachineGunFireState.OnFiring);
		}
		#endif
	}

	public void OnIdle ()
	{
		if (netType == InstanceNetType.GameNetworkClient) {
			LocalmachinegunFireState = MachineGunFireState.Idle;
		}
		if ((netType == InstanceNetType.GameNetworkBotOffline||netType == InstanceNetType.GameNetWorkOffline)) {
			ChangeFireActions (MachineGunFireState.Idle);
			LocalmachinegunFireState = MachineGunFireState.Idle;
		}
		#if ServerSide
		if(netType == InstanceNetType.GameNetWorkBotMaster)
			OnFireStateChanged(MachineGunFireState.Idle);
		#endif
	}

	void FireMethod ()
	{
		if (ReloadTime < NowTime) {
			GameObject Ammo = Instantiate (Bullet) as GameObject;
			Ammo.transform.position = MachineGunFFPoint.position;
			Ammo.transform.eulerAngles = MachineGunFFPoint.eulerAngles;

			BulletScript AmmoInfo = Ammo.GetComponent<BulletScript> ();
			AmmoInfo.isMachineAmmo = true;
			AmmoInfo.fireOwner = tankFire;

			if (netType == InstanceNetType.GameNetworkMaster) {
				NowTime = 0;
				AmmoInfo.bulletState = BulletState.Master;
			} else {
				MuzzleFlashObject.SetActive (true);
				Invoke ("LightsOff", 0.05f);
				_smoke += SmokeIncrement;
				FireTime += 0.05f;
                if (GameDataManager.OfflineMode) {
					AmmoInfo.bulletState = BulletState.Master;
				} else {
					AmmoInfo.bulletState = 
                        BulletState.Client;
				}
			}
		}
	}

	private void LightsOff ()
	{
		MuzzleFlashObject.SetActive (false);
	}

}
