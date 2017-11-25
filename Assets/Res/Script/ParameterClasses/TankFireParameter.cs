using UnityEngine;

[System.Serializable]
public class TankFireParameter
{
	public float ReloadTime = 6;

	public int FireRecoil = 250, AmmoCount = 30;

	public bool HasMachineGun = false;

    public GameObject bulletType;

    public RuntimeAnimatorController GymAnimation;

	public TankFire.MuzzleFire muzzleFire;

	public TankFire.FireState fireState = TankFire.FireState.Simple;

	public TankFire.AdvanceFireClass advanceFireClass;
}