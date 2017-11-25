//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cockpit/Attitude Indicator")]
public class AttitudeIndicator : MonoBehaviour 
{
	
	public void Update () 
	{
		Vector3 forward = transform.root.forward;
		forward.y = 0.0f;
		forward.Normalize();
		transform.LookAt( transform.position + forward );
		transform.localEulerAngles = new Vector3( -transform.localEulerAngles.x,
													transform.localEulerAngles.y,
													transform.localEulerAngles.z);
	}
}
