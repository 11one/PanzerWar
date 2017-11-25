//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Dynamics/Aerofoil")]
public class Aerofoil : MonoBehaviour 
{
	public AnimationCurve CL = null;
	public AnimationCurve CD = null;
	public AnimationCurve CM = null;
}
