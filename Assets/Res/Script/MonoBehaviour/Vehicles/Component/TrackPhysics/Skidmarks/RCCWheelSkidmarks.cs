using UnityEngine;
using System.Collections;

public class RCCWheelSkidmarks : MonoBehaviour {
	
	
	public PlayerTracksController vehicle;
	private Rigidbody vehicleRigid;

	public int lastSkidmark = -1;

	private float wheelSlipAmountSideways = 1;
	private float wheelSlipAmountForward = 1;
	
	void  Start (){
		vehicle = transform.root.GetComponentInChildren<PlayerTracksController> ();
		vehicleRigid = vehicle.GetComponent<Rigidbody>();
	}
	
	void  FixedUpdate (){
		if (vehicleRigid.velocity.magnitude > 1f) 
			lastSkidmark = RCCSkidmarks.Instance.AddSkidMark(transform.position, transform.up, (wheelSlipAmountSideways / 2f) + (wheelSlipAmountForward / 2.5f), lastSkidmark);
		else
			lastSkidmark = -1;

	}
}