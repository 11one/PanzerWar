//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cockpit/Rudder Pedal")]
public class RudderPedal : MonoBehaviour 
{
	public Vector3 TranslateAxis = Vector3.forward;
	public float DeflectionMeters = 0.1f;
	
	[HideInInspector]
	public InputController Controller = new InputController();
	
	private Vector3 InitialPosition = Vector3.zero;
	
	
	// Use this for initialization
	void Start () 
	{
		InitialPosition = transform.localPosition;
		TranslateAxis.Normalize();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float rudderDeflection = Controller.GetAxisInput() * DeflectionMeters;
		transform.localPosition = InitialPosition;
		transform.localPosition += TranslateAxis * rudderDeflection;
	}
}
