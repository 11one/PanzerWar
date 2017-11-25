using UnityEngine;
using System.Collections;

public enum BulletState {
    Master,
    Client,
    Info
}

public enum BulletType {
    ApAmmo,
    HeAmmo,
    APCRAmmo,
    NetworkAmmo
}

public enum BulletSize {
    Small,
    Middle,
    Large
}

[System.Serializable]
public class AmmoAudio {
    [System.Serializable]
    public enum AmmoFireAudio {
        cannon_75mm_qf75,
        cannon_76mm_m1,
        cannon_76mm_zis5,
        cannon_88mm_pak43,
        cannon_105mm_kwk46,
        cannon_128mm_pak44,
        cannon_130mm_b13,
        cannon_150mm_type38,
        weapon_ussr_20_shvak20_shooting_lastshot,
        cannon_85mm_zis_c53,
        cannon_380mm_stum,
        cannon_20mm_kwk30,
        cannon_100mm_d10t,

    }

    [System.Serializable]
    public enum AmmoExplAudio {
        expl_ammo_mid,
    }

    public AmmoFireAudio ammoFireAudio;
    public AmmoExplAudio ammoExplAudio;
}
public class BulletScript : BulletBase {
    public BulletState bulletState;

    public BulletType bulletType;

    public BulletSize bulletSize;

    public AmmoAudio ammoAudio;

    public string FireExplosionHitOnArmor = "FireExplosionHitOnArmor", FireExplosionHitOnGround = "FireExplosionHitOnGround"; 

    public bool isMachineAmmo = false;

    public bool UseGravity = true; // AI will fire ammo without the affect of gravity
    // Bullet Properties
    [Header("Caliber(mm)")]
    public int Caliber = 1;

    public float Speed = 200;

    public int APDamage = 300, APPenetration = 100;

    public int HeDamage = 0, HePenetration = 0;

    public int ApcrPenration; //Apcr Damage is converted automatically

    public float HeRange = 10;

    public float GravityDamper = 1; // SPG will get different gravity damper  Works like WOT

    [HideInInspector]
    public TankFire fireOwner;

    [HideInInspector]
    public Vector3 fireStartPosition;

    private bool IsHitted = false; // Is Bullet hit something

    private Vector3 P0;

    private float shellNormalizationAngle = 0;

    private GameObject weaponProjectileEffect;

    ProjectileManager projectile = new ProjectileManager(); //Initialize project system so the bullet can move based on the formula

    private float EnergyDamper = 1;

    private float PenetrationDamper = 1;

    public void Start() {
        // Initialize the fire sound.
        if (bulletState == BulletState.Info) {
            StartCoroutine(InitWeaponSound());
            return;
        }
        if (bulletType == BulletType.ApAmmo) {
            shellNormalizationAngle = 3;
        }
        else if (bulletType == BulletType.APCRAmmo) {
            shellNormalizationAngle = 1;
            Speed *= 1.25f;
        }
        else if (bulletType == BulletType.HeAmmo) {
            Speed *= 0.75f;
        }

        PoolManager.CreateObject(ammoAudio.ammoFireAudio.ToString(), transform.position, transform.eulerAngles);


        //Destroy Recycle
        if (isMachineAmmo) {
            StartCoroutine(DelayDestroy(5));
        }
        else {
            StartCoroutine(DelayDestroy(10));
        }

        weaponProjectileEffect = PoolManager.CreateObject("WeaponProjectile", transform.position, transform.eulerAngles);

        //Projectile Manager
        P0 = this.gameObject.transform.position;
        projectile.Velocity = Speed;
        projectile.Gravity = GravityDamper;
        Previous = transform.position;
    }

    Vector3 Previous = Vector3.zero;



    float t = 0;

    void FixedUpdate() {
        if (bulletState == BulletState.Info)
            return;

        Vector2 Previous_xy = projectile.XY(t);

        Previous = P0 + projectile.ProcessedXY(new Vector3(0, -Previous_xy.y, Previous_xy.x), transform.eulerAngles.x, transform.eulerAngles.y);

        t += Time.fixedDeltaTime;

        Vector2 _xy = projectile.XY(t);

        transform.position = P0 + projectile.ProcessedXY(new Vector3(0, -_xy.y, _xy.x), transform.eulerAngles.x, transform.eulerAngles.y);


        if (bulletType == BulletType.ApAmmo || bulletType == BulletType.APCRAmmo)
            APAmmoOnHit();
        if (bulletType == BulletType.HeAmmo) {
            HeAmmoOnHit();
        }

        if (weaponProjectileEffect) {
            weaponProjectileEffect.transform.position = transform.position;
        }
    }



    bool RayHitDetect(out RaycastHit rayHit) {
        Vector3 Dir = transform.position - Previous;


        int layerMask = 1 << LayerMask.NameToLayer("HitBox") | 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Terrian");

        bool IsHit = Physics.Raycast(Previous, Dir, out rayHit, Dir.magnitude, layerMask);



        return IsHit;
    }

    void APAmmoOnHit() {
        RaycastHit hit;
        if (IsHitted)
            return;

        if (RayHitDetect(out hit)) {
            float Degree = Vector3.Angle(transform.forward * -1, hit.normal * 1);
            //Normalize 
            Degree = Mathf.Clamp(Degree, 0, Degree - shellNormalizationAngle - (float)Caliber / 100);

            int Penetration = 0;
            switch (bulletType) {
                case BulletType.ApAmmo:
                    Penetration = APPenetration;
                    PenetrationDamper *= 1.2f;
                    break;
                case BulletType.APCRAmmo:
                    Penetration = ApcrPenration;
                    PenetrationDamper *= 2;
                    break;
            }

            float Distance = Vector3.Distance(transform.position, P0);

            Penetration = Mathf.RoundToInt(Random.Range(0.95f, 1.05f) * Penetration * EnergyDamper - Distance * 0.1f * PenetrationDamper);

            int Damage = Mathf.RoundToInt(Random.Range(0.95f, 1.05f) * APDamage * EnergyDamper);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("HitBox")) {
                HitBox _hitBox = hit.collider.GetComponent<HitBox>();
                if (_hitBox == null)
                    return;

                if (Degree > 65) {
                    transform.position = hit.point;
                    transform.LookAt(Vector3.Reflect(transform.forward * 1, hit.normal) + hit.point);
                    EnergyDamper *= 0.4f;
                    _hitBox.ApplyRicochet(fireOwner);
                    Debug.DrawRay(transform.position, hit.normal);
                    if (!GameDataManager.OfflineMode) {
                        CreateFireExplosionHitOnArmor(hit.point, transform.forward, "BulletReflectionEffect");
                    }
                    return;
                }


                _hitBox.ApplyDamage(Penetration, Damage, Degree, fireOwner, bulletType, fireStartPosition, hit.point, Caliber);

                CreateFireExplosionHitOnArmor(hit.point, hit.normal, FireExplosionHitOnArmor);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrian")) {
                CreateFireExplosionHitOnArmor(hit.point, hit.normal, FireExplosionHitOnGround);
                if (!isMachineAmmo) {
                    hit.collider.GetComponent<TerrainManager>().DestroyTerrain(hit.point, 2);
                }
            }
            else {
                CreateFireExplosionHitOnArmor(hit.point, hit.normal, FireExplosionHitOnGround);

                if (hit.collider.transform.GetComponentInParent<Building>() != null) {
                    hit.collider.transform.GetComponentInParent<Building>().ApplyAmmoHit(APDamage);
                }
            }
            IsHitted = true;

            if (!isMachineAmmo) {
                PoolManagerSpawnModule.CreateObject(ammoAudio.ammoExplAudio.ToString(), transform.position, transform.eulerAngles);
            }

            Explode();
 
        }

    }

    void HeAmmoOnHit() {
        if (IsHitted)
            return;

        RaycastHit Rayhit;
        if (RayHitDetect(out Rayhit)) {
            float Degree = Vector3.Angle(transform.forward * -1, Rayhit.normal * 1);
            int Penetration = Mathf.RoundToInt(Random.Range(0.85f, 1.25f) * HePenetration * EnergyDamper);
            int Damage = Mathf.RoundToInt(Random.Range(0.85f, 1.25f) * HeDamage * EnergyDamper);


            if (Rayhit.collider.GetComponent<TerrainManager>())
                Rayhit.collider.GetComponent<TerrainManager>().DestroyTerrain(Rayhit.point, Mathf.Clamp(HeRange, 0, 5));

            if (Rayhit.collider.transform.parent.parent.GetComponent<Building>() != null) {
                Rayhit.collider.transform.parent.parent.GetComponent<Building>().ApplyAmmoHit(HeDamage);
            }


            if (Rayhit.collider.gameObject.layer == LayerMask.NameToLayer("HitBox")) {
                HitBox _hitBox = Rayhit.collider.GetComponent<HitBox>();
                if (_hitBox == null)
                    return;

                _hitBox.ApplyDamage(Penetration, Damage, Degree, fireOwner, bulletType, fireStartPosition, Rayhit.point, Caliber);
                CreateFireExplosionHitOnArmor(Rayhit.point, Rayhit.normal, FireExplosionHitOnArmor);
            }
            else {
                CreateFireExplosionHitOnArmor(Rayhit.point, Rayhit.normal, FireExplosionHitOnGround);

                RaycastHit[] HEAttachedHits = Physics.SphereCastAll(Rayhit.point, HeRange, transform.forward, HeRange, 1 << LayerMask.NameToLayer("HitBox"));

                System.Collections.Generic.List<Transform> Attached = new System.Collections.Generic.List<Transform>();

                foreach (RaycastHit HEAttachedHit in HEAttachedHits) {
                    if (!Attached.Contains(HEAttachedHit.collider.transform.root)) {
                        HitBox _hitBox = HEAttachedHit.collider.GetComponent<HitBox>();
                        if (_hitBox == null)
                            continue;
                        float Distance = (Rayhit.point - HEAttachedHit.collider.transform.position).magnitude;
                        float HEWithDistanceDamage = Damage;
                        HEWithDistanceDamage *= Mathf.Clamp(1f - (Distance / HeRange), 0f, 1f);
                        _hitBox.ApplyDamage(Penetration, Mathf.RoundToInt(HEWithDistanceDamage), Degree, fireOwner, bulletType, fireStartPosition, HEAttachedHit.point, Caliber);
                        Attached.Add(HEAttachedHit.collider.transform.root);
                    }
                }
            }

            IsHitted = true;
            Explode();
        }
    }



    public IEnumerator DelayDestroy(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Explode();
    }

    public void Explode() {
        Destroy(gameObject);
    }


    //create explode effect looking at the normal
    void CreateFireExplosionHitOnArmor(Vector3 Position, Vector3 Normal, string ExplosionName) {
        PoolManagerSpawnModule.CreateObject(ExplosionName, Position, Quaternion.LookRotation(Normal).eulerAngles);
    }

    public IEnumerator InitWeaponSound() {
        //Prevent loading Audio assets twice.
        if (PoolManager.LoadedObjectName.Contains(ammoAudio.ammoFireAudio.ToString()) == false) {
            ResourceRequest resourceRequest = Resources.LoadAsync("Audio/Sounds/res/" + ammoAudio.ammoFireAudio.ToString());
            yield return resourceRequest;


            GameObject ammoSoundRes = (GameObject)Instantiate(resourceRequest.asset);
            if (isMachineAmmo)
                PoolManager.UpdateParams(ammoAudio.ammoFireAudio.ToString(), ammoSoundRes, 20, 2, false);
            else
                PoolManager.UpdateParams(ammoAudio.ammoFireAudio.ToString(), ammoSoundRes, 10, 4, false);
        }
    }
}
