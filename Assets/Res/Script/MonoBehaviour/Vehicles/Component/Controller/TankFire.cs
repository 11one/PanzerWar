using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;
using UnityEngine.EventSystems;



[AddComponentMenu("TankWar/Scripts/TankFire")]
public class TankFire : BaseFireSystem {
    public enum NationType {
        CN,
        RU,
        GER,
        US,
        UK,
        JP,
        FR,
        CZ
    }

    public enum FireState {
        Simple,
        Advance,
    }

    public enum MuzzleFire {
        MuzzleFire_Small,
        MuzzleFire_Middle,
        MuzzleFire_Large
    }

    [System.Serializable]
    public class AdvanceFireClass {
        public bool LargeFinish = false;
        public float LargeNowTime, LargeReloadTime, SmallNowTime, SmallReloadTime;
        public int MaxSmallAmmo, SmallAmmoLeft;
    }

    public TankFireParameter tankFireParameter;

    public TankInitSystem tankInitSystem;

    private bool HasInitedSystem = false;


    public Transform MainBody;

    public Transform FFPoint;

    public Transform FireEffectPoint;

    public Transform FireRecoilPoint;

    public Transform MachineGunFFPoint;

    //UI
    public Text ReloadTimeCountDownInfo, BattleLog, FireStateInfo;

    public Text[] AmmoInfos;

    public Image[] AmmoTextures;

    public Image MainGunFireLoading;

    public Button FireButton;

    public int SelectAmmo = 0;

    public float NowTime;

    public int[] BulletCountList = new int[3];

    public TurretController turretController;

    public Animator GunDym;


    public bool isAutoCaclulateGravity = false;


    public NationType nationType;



    public InstanceNetType netType;

    public static int Damages, KillTanks, FriendFireCount;

    bool IsVibrate;

    public bool AutoAim;

    public bool isBotControl = false;

    public bool isPlayerControl = false;


    bool isMaster = false;

    public bool ExtraTurret = false;

    PlayerState playerState;

    BasePlayerState basePlayerState;

    public MachineGun machineGun;

    //Aim
    public bool UseGravity = true;

    ProjectileManager projectile = new ProjectileManager();

    float BulletSpeed = 0, BulletSpeedCurrent = 0, BulletGravityCurrent = 0;

    public Vector3 AmmoFinalPostion = Vector3.zero;

    public float fireAngle = 0, gunAngle = 0, damperPerHundredMeters;

    public bool HasFinalPostion = false;

    public Vector3 EndPoint;

    float distance = 0;

    GameObject SPGIndicate;

    private VehicleCamera vehicleCamera;





    public void SetAiMode() {
        isBotControl = true;
    }

    // Load Ammo Properties
    public void LoadAmmo() {
        BulletScript bulletScript = tankFireParameter.bulletType.GetComponent<BulletScript>();
        BulletSpeed = bulletScript.Speed;
        BulletSpeedCurrent = BulletSpeed;

        StartCoroutine(bulletScript.InitWeaponSound());

        BulletGravityCurrent = bulletScript.GravityDamper;

        projectile.Gravity = BulletGravityCurrent;
        projectile.Velocity = BulletSpeedCurrent;

        if (tankInitSystem.PSParameter.vehicleType == VehicleType.SPG) {
            SPGIndicate = Instantiate(Resources.Load("Effects/SPGIndicate")) as GameObject;
        }
    }


    public void InitSystem() {
        HasInitedSystem = true;
        playerState = transform.root.GetComponentInChildren<PlayerState>();
        basePlayerState = transform.root.GetComponentInChildren<BasePlayerState>();

        nationType = tankInitSystem.PSParameter.nationType;

        turretController = GetComponent<TurretController>();

        LoadAmmo();


        if (tankFireParameter.HasMachineGun) {
            machineGun = gameObject.AddComponent<MachineGun>();
            machineGun.MachineGunFFPoint = MachineGunFFPoint;
            machineGun.Init(netType, turretController, this);
        }
        else {
            if (!isBotControl) {
                if (playerState.IsMobile && isPlayerControl) {
                    playerState.userUI.MG.gameObject.SetActive(false);
                }
            }
        }

        if (isBotControl)
            return;


        if (netType == InstanceNetType.GameNetworkMaster) {

        }
        else {
            if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
                Damages = 0;
                KillTanks = 0;
                FriendFireCount = 0;
                ReSetAmmoSprits();
                vehicleCamera = transform.root.GetComponentInChildren<VehicleCamera>();
                IsVibrate = PlayerPrefs.GetInt("IsVibrate") == 1;
                if (isAutoCaclulateGravity) {
                    //lineRenderer = gameObject.AddComponent<LineRenderer>();
                    //lineRenderer.numPositions =6;
                }
                //SPGIndicator = Instantiate(Resources.Load("SPGIndicator")) as GameObject;
            }
            if (netType == InstanceNetType.GameNetworkOthers) {

            }
        }
    }



    void AmmoFinalPostionCaculate() {
        if (!HasInitedSystem || ExtraTurret)
            return;

        Vector3 FinalPostion = Vector3.zero;
        Vector3 Offset = Vector3.zero;
        if (FireAngle(BulletSpeedCurrent, out FinalPostion, out EndPoint)) {
            AmmoFinalPostion = FinalPostion;
            HasFinalPostion = true;
        }
        else {
            HasFinalPostion = false;
        }
#if ClientCode


        GameEvent.HasFinalPostion = HasFinalPostion;
        GameEvent.PlayerAmmoFinalPostion = AmmoFinalPostion;
#endif

    }
    void PlayGunAim() {
        GunDym.Play("Gym_Fire");
    }
    void FixedUpdate() {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            AmmoFinalPostionCaculate();
        }
    }

    Vector3 SPGTarget = new Vector3();
    void Update() {
        if (!HasInitedSystem)
            return;
        if (basePlayerState.Health <= 0)
            return;

        //SPGFire();
        if (tankFireParameter.fireState == FireState.Simple)
            SimpleReloadMethod();
        else
            AdvanceReloadMethod();


        if (netType == InstanceNetType.GameNetWorkOffline) {
            if (Input.GetKeyDown(KeyCode.T)) {
                SetSPGAimCircleTarget();
            }

            if (!ExtraTurret) {
                if (isAutoCaclulateGravity) {
                    FireStateInfo.text = fireAngle.ToString("f0") + "°";
                }
                else {
                    FireStateInfo.text = distance.ToString("f0") + "M";
                }

            }
            if (cInput.GetKeyDown("MainGunFire")) {
                if (playerState.IsMobile) {
                    #region 当移动平台支持外界控制事件时候
                    if (cInput.MobileEnableMonitor) {
                        Fire();
                    }
                    #endregion
                }
                else {
                    #region PC等有键盘操控
                    Fire();
                    #endregion
                }
            }

            if (cInput.GetKeyDown("SelectAmmo1"))
                SelectAp();
            if (cInput.GetKeyDown("SelectAmmo2"))
                SelectHE();
            if (cInput.GetKeyDown("SelectAmmo3"))
                SelectApcr();
            if (cInput.GetKeyDown("AutoAim"))
                SwtichAim();

        }

    }

    public void SetSPGAimCircleTarget() {
        RaycastHit AimRayHit;
        if (Physics.Raycast(vehicleCamera.transform.position, vehicleCamera.transform.forward, out AimRayHit, 1000, 1 << LayerMask.NameToLayer("Terrian"))) {
            SPGTarget = AimRayHit.point;
            SPGIndicate.transform.position = AimRayHit.point;
        }
    }
    public bool IsOnLine;

    public void Fire(BaseEventData args = null) {
        if (netType == InstanceNetType.GameNetWorkOffline) {
            FireActionToClient();
        }
    }

    public void FireActionToClient() {
        bool Fired = false;

        if (tankFireParameter.fireState.CompareTo(FireState.Simple) == 0)
            SimpleFireMethod();
        if (tankFireParameter.fireState.CompareTo(FireState.Advance) == 0)
            AdvanceFireMethod();
    }



    void SimpleFireMethod() {
        if (NowTime > tankFireParameter.ReloadTime && BulletCountList[SelectAmmo] > 0) {
            GameObject ammo = (GameObject)Instantiate(tankFireParameter.bulletType, FFPoint.transform.position, FFPoint.transform.rotation);
            ammo.GetComponent<BulletScript>().bulletType = SwtichAmmo(SelectAmmo);
            ammo.GetComponent<BulletScript>().fireOwner = this;
            ammo.GetComponent<BulletScript>().fireStartPosition = FFPoint.position;
            ammo.GetComponent<BulletScript>().UseGravity = UseGravity;
            NowTime = 0;
            BulletCountList[SelectAmmo] -= 1;
            CheckAmmoCount();
            ammo.GetComponent<BulletScript>().bulletState = BulletState.Master;
            MainBody.GetComponent<Rigidbody>().AddForceAtPosition((-MainBody.forward * (MainBody.GetComponent<Rigidbody>().mass * tankFireParameter.FireRecoil)) - (MainBody.up * (MainBody.GetComponent<Rigidbody>().mass * tankFireParameter.FireRecoil / 5)), MainBody.transform.position + MainBody.transform.forward * -5);

            ReSetAmmoSprits();
            ammo.GetComponent<BulletScript>().bulletState = BulletState.Client;
            PoolManager.CreateObject(tankFireParameter.muzzleFire.ToString(), FireEffectPoint.transform.position, FireEffectPoint.transform.eulerAngles);
            StartCoroutine(FireSmokeGenerater());
        }
        else {
            if (isBotControl) {
                return;

            }
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("OnReloading"));

        }
    }

    public IEnumerator FireSmokeGenerater() {
        PlayGunAim();

        GameObject _FireSmoke = PoolManager.CreateObject("FireSmoke", FireEffectPoint.transform.position, FireEffectPoint.transform.eulerAngles);
        if (_FireSmoke == null)
            yield break;

        while (_FireSmoke.activeSelf) {
            _FireSmoke.transform.position = FireEffectPoint.transform.position;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }


    public void SimpleReloadMethod() {
        if (NowTime < tankFireParameter.ReloadTime) {
            DisActiveFireButton();
            NowTime += Time.deltaTime;
            float ReloadTimeCountDown = tankFireParameter.ReloadTime - NowTime;

            ReloadTimeCountDown = Mathf.Clamp(ReloadTimeCountDown, 0, tankFireParameter.ReloadTime);
            if (netType == InstanceNetType.GameNetworkMaster) {

            }
            else if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
                #region 客户端
                if (!ExtraTurret) {
                    ReloadTimeCountDownInfo.text = ReloadTimeCountDown.ToString("f1") + "s";
                    AmmoTextures[SelectAmmo].fillAmount = NowTime / tankFireParameter.ReloadTime;
                    if (playerState.IsMobile) {
                        MainGunFireLoading.fillAmount = NowTime / tankFireParameter.ReloadTime;
                    }
                    AmmoInfos[SelectAmmo].text = BulletCountList[SelectAmmo].ToString();
                    playerState.userUI.ReloadBar.fillAmount = NowTime / tankFireParameter.ReloadTime;
                }
                #endregion
            }
        }
        else {
            ActiveFireButton();
        }
    }


    public void AdvanceFireMethod() {
        if (BulletCountList[SelectAmmo] > 0)
            if (tankFireParameter.advanceFireClass.SmallNowTime > tankFireParameter.advanceFireClass.SmallReloadTime) {
                if (tankFireParameter.advanceFireClass.LargeFinish && tankFireParameter.advanceFireClass.SmallAmmoLeft > 0) {
                    ReSetAmmoSprits();
                    tankFireParameter.advanceFireClass.SmallAmmoLeft -= 1;
                    tankFireParameter.advanceFireClass.SmallNowTime = 0;
                    GameObject ammo = Instantiate(tankFireParameter.bulletType, FFPoint.transform.position, FFPoint.transform.rotation) as GameObject;
                    ammo.GetComponent<BulletScript>().fireOwner = this;
                    ammo.GetComponent<BulletScript>().bulletType = SwtichAmmo(SelectAmmo);
                    ammo.GetComponent<BulletScript>().fireStartPosition = FFPoint.position;
                    ammo.GetComponent<BulletScript>().UseGravity = UseGravity;


                    ammo.GetComponent<BulletScript>().bulletState = BulletState.Master;
                    MainBody.GetComponent<Rigidbody>().AddForceAtPosition((-MainBody.forward * (MainBody.GetComponent<Rigidbody>().mass * tankFireParameter.FireRecoil)) - (MainBody.up * (MainBody.GetComponent<Rigidbody>().mass * tankFireParameter.FireRecoil / 5)), MainBody.transform.position + MainBody.transform.forward * -5);
                    PoolManager.CreateObject(tankFireParameter.muzzleFire.ToString(), FireEffectPoint.transform.position, FireEffectPoint.transform.eulerAngles);
                    StartCoroutine(FireSmokeGenerater());


                    if (!ExtraTurret)
                        AmmoInfos[SelectAmmo].text = BulletCountList[SelectAmmo].ToString();
                }

                BulletCountList[SelectAmmo] -= 1;
                CheckAmmoCount();

                ReSetAmmoSprits();
                PoolManager.CreateObject(tankFireParameter.muzzleFire.ToString(), FireEffectPoint.transform.position, FireEffectPoint.transform.eulerAngles);
                StartCoroutine(FireSmokeGenerater());
            }
            else {
                if (isBotControl)
                    return;
                playerState.UpdataGameNote(uGUI_Localsize.GetContent("OnReloading"));
            }

        if (tankFireParameter.advanceFireClass.SmallAmmoLeft <= 0) {
            tankFireParameter.advanceFireClass.LargeFinish = false;
        }
    }


    void AdvanceReloadMethod() {
        if (tankFireParameter.advanceFireClass.LargeNowTime > tankFireParameter.advanceFireClass.LargeReloadTime) {
            tankFireParameter.advanceFireClass.LargeFinish = true;
            tankFireParameter.advanceFireClass.LargeNowTime = 0;
            if (netType == InstanceNetType.GameNetworkMaster) {

            }
            else if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
                #region 客户端
                if (!ExtraTurret)
                    ReloadTimeCountDownInfo.text = "Reloaded";
                #endregion
            }
        }
        if (!tankFireParameter.advanceFireClass.LargeFinish) {
            tankFireParameter.advanceFireClass.LargeNowTime += Time.deltaTime;
            if (netType == InstanceNetType.GameNetworkMaster) {

            }
            else if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
                #region 客户端
                if (!ExtraTurret) {
                    ReloadTimeCountDownInfo.text = (tankFireParameter.advanceFireClass.LargeReloadTime - tankFireParameter.advanceFireClass.LargeNowTime).ToString("f1") + "s";
                    AmmoTextures[SelectAmmo].fillAmount = tankFireParameter.advanceFireClass.LargeNowTime / tankFireParameter.advanceFireClass.LargeReloadTime;
                    playerState.userUI.ReloadBar.fillAmount = tankFireParameter.advanceFireClass.LargeNowTime / tankFireParameter.advanceFireClass.LargeReloadTime;
                }
                #endregion
            }
        }
        else {
            if (tankFireParameter.advanceFireClass.SmallAmmoLeft <= 0) {
                tankFireParameter.advanceFireClass.SmallAmmoLeft = tankFireParameter.advanceFireClass.MaxSmallAmmo;
            }
            if (tankFireParameter.advanceFireClass.SmallNowTime < tankFireParameter.advanceFireClass.SmallReloadTime) {
                tankFireParameter.advanceFireClass.SmallNowTime += Time.deltaTime;
                if (netType == InstanceNetType.GameNetworkMaster) {

                }
                else if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
                    #region 客户端
                    if (!ExtraTurret) {
                        ReloadTimeCountDownInfo.text = (tankFireParameter.advanceFireClass.SmallReloadTime - tankFireParameter.advanceFireClass.SmallNowTime).ToString("f1");
                        playerState.userUI.ReloadBar.fillAmount = tankFireParameter.advanceFireClass.SmallNowTime / tankFireParameter.advanceFireClass.SmallReloadTime;
                    }
                    #endregion
                }
            }
        }

    }

    void DisActiveFireButton() {
        if (BaseInitSystem.isLocalPlayer(netType))
            if (playerState.IsMobile && !ExtraTurret)
                FireButton.interactable = false;
    }

    void ActiveFireButton() {
        if (BaseInitSystem.isLocalPlayer(netType))
            if (playerState.IsMobile && !ExtraTurret)
                FireButton.interactable = true;
    }

    #region 伤害系统

    public override void AddDamage(int Damage, string HitVehicle) {
        base.AddDamage(Damage, HitVehicle);
        if (GameDataManager.OfflineMode) {
            OnServerAddDamage(Damage);
        }

        ApplyHit(Damage, 0, HitVehicle);
    }

    void OnServerAddDamage(int Damage) {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            Damages += Damage;
            Waroftanks.Music.BattleMusic.PlayBGMinFight();

            //BattleLog.text = uGUI_Localsize.GetContent("CausedDamage")+Damages.ToString()+uGUI_Localsize.GetContent("KillTankInAll")+KillTanks.ToString();		
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("CausedDamage") + Damage.ToString());
            playerState.UpdateDamageBar(Damages, KillTanks);
            InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Armor_Pierced, InGameVoiceSoundManager.InGameVoiceLanguage(nationType));
        }
    }

    #region 未击穿

    public void NotBreakDown(int Armor) {
        if (GameDataManager.OfflineMode) {
            OnServerNotBreakDown(Armor);
        }

    }

    void OnServerNotBreakDown(int Armor) {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            Debug.Log("OnServerNotBreakDown");
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("NotBreakDown") + uGUI_Localsize.GetContent("HitArmor") + Armor.ToString());
            InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Armor_Not_Pierced, InGameVoiceSoundManager.InGameVoiceLanguage(nationType));
        }
    }

    #endregion

    #region 跳弹

    public void Ricochet() {
        if (GameDataManager.OfflineMode) {
            OnServerRicochet();
        }
    }

    void OnServerRicochet() {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("Ricochet"));
            InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Armor_Ricochet, InGameVoiceSoundManager.InGameVoiceLanguage(nationType));
        }
    }

    #endregion

    #region 击毁坦克

    public override void KillTank(string HitVehicle) {
        base.KillTank(HitVehicle);
        if (GameDataManager.OfflineMode) {
            OnServerKillTank();
        }
        else {
            ApplyHit(0, 1, HitVehicle);
        }
    }

    void OnServerKillTank() {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            Waroftanks.Music.BattleMusic.StopBGM();
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("KillTank"));
            KillTanks++;
            playerState.UpdateDamageBar(Damages, KillTanks);
            InGameVoiceSoundManager.PlayVoice(InGameVoiceSoundManager.VoiceType.Enemy_Killed, InGameVoiceSoundManager.InGameVoiceLanguage(nationType));
            //BattleLog.text = uGUI_Localsize.GetContent ("CausedDamage") + Damages.ToString () + uGUI_Localsize.GetContent ("KillTankInAll") + KillTanks.ToString ();			
        }
    }

    #endregion


    #region 击杀友军

    public override void KillFriend(string Vehicle) {
        base.KillFriend(Vehicle);
        if (GameDataManager.OfflineMode) {
            OnServerKillFriend(Vehicle);
        }

    }

    void OnServerKillFriend(string Vehicle) {
        if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline) {
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("FirendFire") + uGUI_Localsize.GetContent(Vehicle));

        }
    }

    #endregion

    #endregion



    //补给弹药
    public void UpdateAmmo() {
        BulletCountList = tankInitSystem.BulletCountList;
        ReSetAmmoSprits();
    }

    public void SelectAp() {
        if (SelectAmmo == 0)
            return;

        if (BulletCountList[0] <= 0) {
            OnAmmoOut();
            return;
        }
        BulletSpeedCurrent = BulletSpeed;
        SelectAmmo = 0;
        NowTime = 0;
        tankFireParameter.advanceFireClass.SmallNowTime = 0;

        playerState.UpdataGameNote(uGUI_Localsize.GetContent("SelectAp"));
        ReSetAmmoSprits();
    }

    public void SelectHE() {
        if (SelectAmmo == 1)
            return;

        BulletSpeedCurrent = BulletSpeed * 0.75f;
        SelectAmmo = 1;
        NowTime = 0;
        tankFireParameter.advanceFireClass.SmallNowTime = 0;

        playerState.UpdataGameNote(uGUI_Localsize.GetContent("SelectAp"));
        ReSetAmmoSprits();
    }

    public void SelectApcr() {
        if (SelectAmmo == 2)
            return;
        
        BulletSpeedCurrent = BulletSpeed * 1.25f;
        SelectAmmo = 2;
        NowTime = 0;
        tankFireParameter.advanceFireClass.SmallNowTime = 0;

        playerState.UpdataGameNote(uGUI_Localsize.GetContent("SelectAp"));
        ReSetAmmoSprits();

    }

    public void OnAmmoOut() {
        if (BulletCountList[0] > 0) {
            SelectAp();
        }
        else if (BulletCountList[1] > 0) {
            SelectHE();
        }
        else {
            SelectApcr();
        }
    }
    private void CheckAmmoCount() {
        if (BulletCountList[SelectAmmo] <= 0) {
            OnAmmoOut();
        }
    }
    public void ReSetAmmoSprits() {
        if (ExtraTurret)
            return;

        for (int i = 0; i < AmmoTextures.Length; i++) {
            AmmoTextures[i].fillAmount = 1;
            AmmoInfos[i].text = BulletCountList[i].ToString();
            if (i == SelectAmmo) {
                AmmoTextures[i].transform.parent.GetComponent<Image>().color = new Color(0.51f, 0.576f, 0.639f, .8f);
            }
            else {
                AmmoTextures[i].transform.parent.GetComponent<Image>().color = new Color(0.51f, 0.576f, 0.639f, .35f);
            }
        }
    }





    public Transform MinDegreeTarget() {
        Transform returnTransform = null;

        ArrayList Targets = new ArrayList();
        foreach (Identity id in GameObject.FindObjectsOfType<Identity>()) {
            if (id.identity != transform.root.GetComponentInChildren<Identity>().identity) {
                Targets.Add(id);
            }
        }
        float Degree = Mathf.Infinity;
        foreach (Identity temp in Targets) {
            if (Vector3.Angle(turretController.Turret.transform.forward, temp.transform.position - turretController.Turret.transform.position) < Degree) {
                RaycastHit hit;
                Vector3 dir = temp.transform.position + Vector3.up - turretController.Turret.position + Vector3.up;
                Physics.Raycast(turretController.Turret.position, dir, out hit, dir.magnitude, 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Terrian"));
                if (hit.collider == null) {
                    Degree = Vector3.Angle(turretController.Turret.transform.forward, temp.transform.position - turretController.Turret.transform.position);
                    returnTransform = temp.transform;
                }
            }
        }
        if (returnTransform == null)
            return null;

        Transform AimTarget;
        AimTarget = returnTransform.Find("AimTarget");
        if (AimTarget == null) {
            HitBox[] HitBoxs = returnTransform.GetComponentsInChildren<HitBox>();
            Vector3 Centers = Vector3.zero;
            for (int i = 0; i < HitBoxs.Length; i++) {
                Centers += HitBoxs[i].CenterBound.transform.position;
            }
            GameObject AutoAimTemp = new GameObject("AutoAimTemp");
            AutoAimTemp.transform.position = Centers / HitBoxs.Length;
            AutoAimTemp.transform.SetParent(returnTransform.transform);
            return AutoAimTemp.transform;
        }
        return AimTarget;

    }

    public void SwtichAim() {
        if (AutoAim) {
            AutoAim = false;
            turretController.target = turretController.OriginTarget;
            playerState.UpdataGameNote(uGUI_Localsize.GetContent("UnuseAutoAim"));

        }
        else {
            Transform Target = MinDegreeTarget();
            if (Target == null) {
                playerState.UpdataGameNote(uGUI_Localsize.GetContent("FindNoTarget"));
            }
            else {
                AutoAim = true;
                turretController.target = Target;
                playerState.UpdataGameNote(uGUI_Localsize.GetContent("UseAutoAim"));
            }
        }
    }

    void ApplyHit(int Hit, int DestroyTankCount, string HitVehicle) {

    }

    public BulletType SwtichAmmo(int AmmoType) {
        if (AmmoType == 0)
            return BulletType.ApAmmo;
        else if (AmmoType == 1)
            return BulletType.HeAmmo;
        else
            return BulletType.APCRAmmo;
    }


    //
    bool FireAngle(float AmmoVelocity, out Vector3 _finalPosition, out Vector3 _endPoint) {
        distance = 0;
        _endPoint = Vector3.zero;
        _finalPosition = Vector3.zero;

        projectile.Velocity = AmmoVelocity;


        RaycastHit AimRayHit = new RaycastHit();
        if (SPGTarget != Vector3.zero) {
            _endPoint = SPGTarget;
        }
        else {
            if (vehicleCamera.inSPGMode) {
                if (!Physics.Raycast(vehicleCamera.transform.position, vehicleCamera.transform.forward, out AimRayHit, 1000))
                    return false;
            }
            else {
                if (!Physics.Raycast(FFPoint.position, FFPoint.forward, out AimRayHit, 1000))
                    return false;
            }
            _endPoint = AimRayHit.point;
        }

        float x = Vector2.Distance(new Vector2(FFPoint.position.x, FFPoint.position.z), new Vector2(_endPoint.x, _endPoint.z));

        distance = x;

        fireAngle = projectile.FireAngle(FFPoint.forward, new Vector3(0, 1, 0));

        if (gunAngle > 180) {
            gunAngle = 360 - gunAngle;
        }
        else {
            gunAngle = -gunAngle;
        }
        float t2EndPoint = projectile.x2t(x);

        Vector2 xy = projectile.XY(t2EndPoint);
        Vector3 LocalXY = projectile.ProcessedXY(new Vector3(0, -xy.y, xy.x), FFPoint.eulerAngles.x, FFPoint.eulerAngles.y);


        _finalPosition = FFPoint.position + LocalXY;

        return true;
    }


}
