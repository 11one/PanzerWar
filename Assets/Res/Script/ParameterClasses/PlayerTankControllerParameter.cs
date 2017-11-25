using UnityEngine;

[System.Serializable]
public class PlayerTankControllerParameter
{
	public GameObject TankWheelCollider;
	public Transform CenterOfGravity;
	[HideInInspector]
	public float SideWaysFrictionExtremumFactor = 1.2f, SideWaysFrictionAsymptoteFactor = 1.2f;
	public int MaxSpeed = 45, MinSpeed = -30, MaxAngularVelocity = 45;
	public float PushSpeed = 1, BackSpeed = 1;
	public int Mass = 4500;
	public float Drag = 0.2f, AirDrag = 0.2f;

	public TankTracksController.HAconfig HAconfigSetting = new TankTracksController.HAconfig ();
	public TankTracksController.VAconfig VAconfigSetting = new TankTracksController.VAconfig ();

    public VehicleEngineSoundData vehicleEngineSoundData;
}