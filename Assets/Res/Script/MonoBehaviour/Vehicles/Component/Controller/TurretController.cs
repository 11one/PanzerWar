using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{
	public Transform target, OriginTarget, Turret, gun;
	public float turretDegreesPerSecond = 45.0f, gunDegreesPerSecond = 45.0f;
	float maxGunAngle;
	public float maxTurretAngle = 180;
	Quaternion qTurret, qGun, qGunStart;
	Transform trans;
	GameObject GunCenter;
	public float UpMaxDegree = 15, DownMaxDegree = 20;
	[HideInInspector]
	public bool isMobile = false;

	void  Start ()
	{
		GunCenter = new GameObject ("GunCenter");
		GunCenter.transform.parent = Turret.transform.parent;
		GunCenter.transform.localPosition = Turret.localPosition;
		GunCenter.transform.localEulerAngles = Turret.localEulerAngles;

		if (target)
			OriginTarget = target;

		trans = Turret.transform; 
		qGunStart = gun.transform.localRotation;
		maxGunAngle = (UpMaxDegree + DownMaxDegree) / 2;
		qGunStart.x -= (UpMaxDegree - maxGunAngle) * Mathf.Deg2Rad;
	}

	void  Update ()
	{
		if (target == null) {
			target = OriginTarget;
			return;
		}
        Vector3 P0 = target.position;


		float TurretAngle = Vector2.Angle (new Vector2 (GunCenter.transform.forward.x, GunCenter.transform.forward.z), new Vector2 ((P0 - Turret.position).x, (P0 - Turret.position).z));
		float distanceToPlane = Vector3.Dot (trans.up, P0 - trans.position);

		Vector3 planePoint = P0 - trans.up * distanceToPlane;
		Vector3 forwardVector = planePoint - trans.position;
		Vector3 upVector = Turret.transform.up;
		
		qTurret = Quaternion.LookRotation (forwardVector, upVector);


		if (TurretAngle < maxTurretAngle || Vector3.Angle (Turret.transform.forward, GunCenter.transform.forward) < maxTurretAngle) {
			Turret.transform.rotation = Quaternion.RotateTowards (Turret.transform.rotation, qTurret, turretDegreesPerSecond * Time.deltaTime);
		}

		Vector3 v3 = new Vector3 (0, distanceToPlane, (planePoint - Turret.transform.position).magnitude);
		qGun = Quaternion.LookRotation (v3);

		if (Quaternion.Angle (qGunStart, qGun) <= maxGunAngle) {
			gun.localRotation = Quaternion.RotateTowards (gun.localRotation, qGun, gunDegreesPerSecond * Time.deltaTime);
		} else {
			if (Quaternion.Angle (qGunStart, gun.localRotation) <= maxGunAngle) {
				gun.localRotation = Quaternion.RotateTowards (gun.localRotation, qGun, gunDegreesPerSecond * Time.deltaTime);
			}
		}
//		else {
//			if (Vector3.Dot (Quaternion.Euler(0,0,-maxGunAngle)*(target.transform.position + PostionOffSet - gun.transform.position), GunCenter.transform.up) * Vector3.Dot (Quaternion.Euler(0,0,maxGunAngle)*gun.forward, GunCenter.transform.up) < 0) {
//				gun.localRotation = Quaternion.RotateTowards (gun.localRotation, qGun, gunDegreesPerSecond * Time.deltaTime);
//			}
//			else if (Vector3.Dot (Quaternion.Euler(0,0,maxGunAngle)*(target.transform.position + PostionOffSet - gun.transform.position), GunCenter.transform.up) * Vector3.Dot (Quaternion.Euler(0,0,-maxGunAngle)*gun.forward, GunCenter.transform.up) < 0) {
//				gun.localRotation = Quaternion.RotateTowards (gun.localRotation, qGun, gunDegreesPerSecond * Time.deltaTime);
//			}
//		}
	}
}
