//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Base/AircraftAttachment")]
public class AircraftAttachment : MonoBehaviour 
{
	// Base class for all UnityFS attachable objects.
	protected bool Controllable = false;
	
	public void SetControllable( bool enable )
	{
		Controllable = enable;
	}
}
