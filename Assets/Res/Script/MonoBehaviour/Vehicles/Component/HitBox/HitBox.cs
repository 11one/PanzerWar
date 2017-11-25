using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class HitBox : MonoBehaviour {
    public HitBoxType hitBoxType;

    [System.Serializable]
    public enum HitBoxType {
        Simple,
        IntermittentArmor,
    }

    [HideInInspector]
    public GameObject Owner;
    [HideInInspector]
    public GameObject CenterBound;

    List<Vector3> ReactiveArmor = new List<Vector3>();
    public float ReactiveArmorDistance = 0.2f;


    public int Armor = 90;

    BasePlayerState basePlayerState;

    void Awake() {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider) {
            GameObject Center = new GameObject("Center");
            Center.transform.SetParent(this.transform);
            Center.transform.position = meshCollider.bounds.center;
            CenterBound = Center;
        }
        gameObject.layer = LayerMask.NameToLayer("HitBox");
        gameObject.tag = "HitBox";

    }

    public void SetTarget(BasePlayerState _basePlayerState) {
        basePlayerState = _basePlayerState;
    }
    //+++++++++++++++++++++++++++++++++++++——————————————————————Apply Damage Functions——————————————————————————————————++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void ApplyDamage(int Penetration, int Damage, float Degree, TankFire Owner, BulletType bulletType, Vector3 FirePostion, Vector3 HitPoint, int Caliber) {
        Degree *= Mathf.Deg2Rad;

        int RealArmor = Mathf.RoundToInt((float)Armor / Mathf.Cos(Degree));
        if (hitBoxType == HitBoxType.IntermittentArmor) {
            RaycastHit HitBoxRay;
            Vector3 Dir = HitPoint - FirePostion;

            if (Physics.Raycast(HitPoint, Dir, out HitBoxRay, 5, 1 << LayerMask.NameToLayer("HitBox"))) {
                HitBox AttachArmor = HitBoxRay.collider.GetComponent<HitBox>();
                float AttachArmorDegree = Vector3.Angle(Dir * -1, HitBoxRay.normal) * Mathf.Deg2Rad;
                RealArmor += Mathf.RoundToInt(AttachArmor.Armor / Mathf.Cos(AttachArmorDegree));
            }
            else {
                OnOnlyHitIntermittentArmor(Owner);
                return;
            }
        }
        bool isPenetrated = (Penetration > RealArmor && Degree < 85) || Caliber > (Armor * 3);


        switch (bulletType) {
            case BulletType.ApAmmo:
            case BulletType.APCRAmmo:
                if (isPenetrated) {
                    OnApplyHitBoxDamage(Owner, Damage, Degree);
                }
                else {
                    OnNotBreakDown(Owner);
                }
                break;
            case BulletType.HeAmmo:
                if (isPenetrated) {

                }
                else {
                    Debug.Log((float)Damage / 2f);

                    Damage = Mathf.RoundToInt(Mathf.Clamp(((float)Damage / 2f) - Armor, 0, Damage));
                }
                OnApplyHitBoxDamage(Owner, Damage, Degree);
                break;
        }
    }

    void OnApplyHitBoxDamage(TankFire Owner, int Damage, float Degree) {
        if (!CheckIsActive()) {
            return;
        }

        basePlayerState.ApplyHitBoxDamage(Damage, Degree, Owner, this);

    }
    public void BasePlayerStateHittedCallBack(int Damage, int Health) {
        PutInfoToHitInfo(transform.position, Quaternion.identity, string.Format("{0}/{1}", Damage.ToString(), Health.ToString()));
    }
    public void BasePlayerStateGodModeCallBack() {
        PutInfoToHitInfo(transform.position, Quaternion.identity, "InGodMode");
    }
    public void BasePlayerStateHitFriendCallBack() {
        PutInfoToHitInfo(transform.position, Quaternion.identity, "HitFriend");
    }

    void OnOnlyHitIntermittentArmor(TankFire Owner) {
        if (!CheckIsActive()) {
            return;
        }
        PutInfoToHitInfo(transform.position, Quaternion.identity, "Only Hit Space Armor");
        basePlayerState.OnNotBreakDown();
        Owner.NotBreakDown(Armor);
    }

    void OnNotBreakDown(TankFire Owner) {
        if (!CheckIsActive()) {
            return;
        }
        PutInfoToHitInfo(transform.position, Quaternion.identity, "Not Pierced");
        basePlayerState.OnNotBreakDown();
        Owner.NotBreakDown(Armor);
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    public void ApplyRicochet(TankFire Owner) {
        if (!CheckIsActive()) {
            return;
        }

        PutInfoToHitInfo(transform.position, Quaternion.identity, "Ricochet");
        //Target.SendMessage ("OnRicochet",SendMessageOptions.DontRequireReceiver);
        basePlayerState.OnRicochet();
        Owner.Ricochet();
    }
    public bool CheckIsActive() {
        if (!enabled) {
            //PutInfoToHitInfo (transform.position, Quaternion.identity, "Destroyed");
            return false;
        }
        else {
            return true;
        }
    }
    void SimpleHit(int Penetration, int Damage, float Degree, GameObject Owner, BulletType bulletType, Vector3 FirePostion, Vector3 HitPoint, int Caliber) {
        //
        //		if (hitBoxType == HitBoxType.IntermittentArmor) {
        //			RaycastHit HitBoxRay;
        //			Vector3 Dir = HitPoint - FirePostion;
        //			if (Physics.Raycast (HitPoint, Dir, out HitBoxRay, 5, 1 << LayerMask.NameToLayer ("HitBox"))) {
        //				Debug.Log (HitBoxRay.collider.name);
        //				HitBox AttachArmor = HitBoxRay.collider.GetComponent<HitBox> ();
        //				float AttachArmorDegree = Vector3.Angle (Dir * -1, HitBoxRay.normal);
        //				RealArmor = Mathf.RoundToInt ((Armor) / Mathf.Cos ((Degree) * (3.15f / 180))) + Mathf.RoundToInt ((AttachArmor.Armor) / Mathf.Cos ((AttachArmorDegree) * (3.15f / 180)));
        //			} else {
        //				OnOnlyHitIntermittentArmor(Target,Owner);
        //				return;
        //			}
        //		} else {
        //		 	RealArmor = Mathf.RoundToInt(Armor/Mathf.Cos((Degree)*(3.15f/180)));
        //		}
        //
        //		Debug.Log (RealArmor);
        //
        //		if (Degree < 75||Caliber>Armor*3) {
        //			#region 未跳弹
        //			#region 击穿
        //			if (Penetration >RealArmor||(Caliber>Armor*3&&hitBoxType != HitBoxType.IntermittentArmor)) {
        //				if(hitBoxType == HitBoxType.Reactive){
        //					if(ReactiveArmorWork(HitPoint)){
        //						ReactiveArmor.Add(HitPoint);
        //						Damage=Mathf.RoundToInt(Damage*0.05f);
        //					}
        //				}
        //				if(hitBoxType == HitBoxType.Composite){
        //					Damage =Mathf.RoundToInt(Damage*0.25f*(Caliber/100));
        //				}
        //				OnApplyHitBoxDamage(Target,Owner,Damage,RealArmor,Degree);
        //			}
        //			#endregion
        //			#region 未击穿
        //			else {
        //				if(bulletType == BulletScript.BulletType.HeAmmo){
        //					Damage = Mathf.RoundToInt(Mathf.Clamp(Damage/2f - RealArmor,0,Damage));
        //					OnApplyHitBoxDamage(Target,Owner,Damage,RealArmor,Degree);
        //				}else {
        //					OnNotBreakDown(Target,Owner);
        //				}
        //			}
        //			#endregion
        //			#endregion
        //		} 
        //		#region 跳弹
        //		else {
        //			if(bulletType == BulletScript.BulletType.HeAmmo){
        //				Damage = Mathf.RoundToInt(Mathf.Clamp(Damage/2f - RealArmor,0,Damage));
        //				OnApplyHitBoxDamage(Target,Owner,Damage,RealArmor,Degree);
        //			}else {
        //				OnRicochet(Target,Owner);
        //			}
        //		}
        //		#endregion
    }

    bool ReactiveArmorWork(Vector3 HitPostion) {
        bool HasHitted = false;
        foreach (Vector3 HittedArea in ReactiveArmor) {
            if (Vector3.Distance(HittedArea, HitPostion) < ReactiveArmorDistance) {
                HasHitted = true;
            }
        }
        return HasHitted;
    }

    public static void PutInfoToHitInfo(Vector3 v, Quaternion q, string Info) {
        GameObject mObject = new GameObject("HitInfo");
        mObject.AddComponent<HitBoxInfoOut>();
        mObject.GetComponent<HitBoxInfoOut>().Target = v;
        mObject.GetComponent<HitBoxInfoOut>().Value = Info;
    }

    void OnDrawGizmosSelected() {
        if (GetComponent<MeshCollider>())
            Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh, transform.position, transform.rotation, transform.lossyScale);

    }

    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "HitBoxIcon.png");
    }

    #region 车库显示

    MeshRenderer visualHitBoxMeshModel;
    GameObject ArmorInfo;
    bool NeedShow = false;

    public IEnumerator ShowArmorInfo() {
        ResourceRequest resourceRequest;
        resourceRequest = Resources.LoadAsync("GarageHitBoxInfo");
        yield return resourceRequest;
        ArmorInfo = (GameObject)Instantiate(resourceRequest.asset, transform.position, transform.rotation);
        ArmorInfo.transform.SetParent(GameObject.Find("UI Root").transform);
        ArmorInfo.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        ArmorInfo.GetComponent<RectTransform>().localEulerAngles = new Vector3(1, 1, 1);
        ArmorInfo.transform.Find("Info/Armor").GetComponent<Text>().text = Armor.ToString();
        ArmorInfo.transform.Find("Info/ArmorType").GetComponent<Text>().text = hitBoxType.ToString();
        ArmorInfo.SetActive(false);
        ArmorInfo.transform.SetAsFirstSibling();
        InitHitBox();
    }

    void OnMouseOver() {
        if (NeedShow == false)
            return;
        visualHitBoxMeshModel.enabled = true;

        if (ArmorInfo)
            ArmorInfo.SetActive(true);
    }

    void OnMouseExit() {
        if (NeedShow == false)
            return;

        visualHitBoxMeshModel.enabled = false;
        if (ArmorInfo)
            ArmorInfo.SetActive(false);
    }

    void OnDisable() {
        if (NeedShow == false)
            return;

        if (visualHitBoxMeshModel)
            visualHitBoxMeshModel.enabled = false;

        if (ArmorInfo)
            ArmorInfo.SetActive(false);
    }

    void Update() {
        if (ArmorInfo) {
            ArmorInfo.transform.position = Camera.main.WorldToScreenPoint(CenterBound.transform.position);
        }
    }

    void InitHitBox() {
        NeedShow = true;

        visualHitBoxMeshModel = GetComponent<MeshRenderer>();

        if (visualHitBoxMeshModel) {
            visualHitBoxMeshModel.material = Resources.Load("HitBoxM") as Material;
            visualHitBoxMeshModel.enabled = false;
        }
        else {
            Destroy(this);
        }
    }

    #endregion
}
