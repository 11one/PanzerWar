//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Dynamics/Prop Wash")]
[RequireComponent (typeof(Wing))]
public class PropWash : AircraftAttachment 
{
	public bool[] AffectedSections = null;
	public Engine PropWashSource = null;
	public float PropWashStrength = 0.01f;

	
	public Vector3 GetPropWash()
	{
		Vector3 propWash = Vector3.zero;
		
		if ( null != PropWashSource )
		{
			propWash = PropWashSource.Thrust;
		}
		
		return propWash;
	}

	public void OnDrawGizmos() 
	{
		ClampEditorValues();
    }
	
	private void ClampEditorValues()
	{
		Wing wing = gameObject.GetComponent<Wing>();
		if (null!= wing )
		{
			if ( (null==AffectedSections) || (wing.SectionCount != AffectedSections.Length) )
			{
				AffectedSections = new bool[wing.SectionCount];
			}
		}
	}
}

