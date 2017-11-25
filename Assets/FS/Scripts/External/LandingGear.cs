//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/External/Landing Gear")]
[RequireComponent(typeof(Aircraft))]
public class LandingGear : MonoBehaviour 
{
	
	public AudioClip ToggleLandingGearClip = null;
	public GameObject ToggleLandingGearAnimationGameObject = null;
	public string ToggleLandingGearAnimationName = "";	
	public float GearDownDrag = 0.05f;
	public float GearUpDrag = 0.04f;
	
	[HideInInspector]
	public InputController LandingGearController = new InputController();
	
	private WheelCollider[] AircraftWheels = null;
	private AudioSource ToggleLandingGear = null;
	private bool GearDown = true;
	private bool CycleFinished = false;
	
	// Use this for initialization
	void Start () 
	{
		AircraftWheels = GetComponentsInChildren<WheelCollider>();
	
		//Add audio source for gear raise and lower if added.
		if ( null != ToggleLandingGearClip )
		{
			ToggleLandingGear = gameObject.AddComponent<AudioSource>();
			ToggleLandingGear.clip = ToggleLandingGearClip;
			ToggleLandingGear.volume = 1.0f;
			ToggleLandingGear.playOnAwake = false;
			ToggleLandingGear.loop = false;
			ToggleLandingGear.dopplerLevel = 0.0f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Listen for input for landing gear.
		if ( LandingGearController.GetButtonPressed() )
		{	
			//Play landing gear audio.
			if ( null != ToggleLandingGear )
			{
				ToggleLandingGear.Play();
			}
			
			//Toggle gear animation.
			if ( ("" != ToggleLandingGearAnimationName) && (null!=ToggleLandingGearAnimationGameObject) )
			{
				//Play landing gear animation.
				Animation animation = ToggleLandingGearAnimationGameObject.GetComponent<Animation>();
				if ( animation )
				{		
					animation[ToggleLandingGearAnimationName].wrapMode = WrapMode.ClampForever;
					animation[ToggleLandingGearAnimationName].speed = GearDown ? 1.0f : -1.0f;

					if ( CycleFinished )
					{
						animation[ToggleLandingGearAnimationName].normalizedTime = GearDown ? 0.0f : 1.0f;
					}
					animation.Play(ToggleLandingGearAnimationName);
			
				}
			}

			
			//Change gear down state.
			GearDown = !GearDown;
			
			//Set drag.
			gameObject.GetComponent<Rigidbody>().drag = GearDown? GearDownDrag : GearUpDrag;
			
			//Enable/disable wheel colliders based on new gear state.
			if ( null != AircraftWheels )
			{
				foreach ( WheelCollider w in AircraftWheels )
				{
					if ( null != w )
					{
						w.enabled = GearDown;
					}
				}
			}
			
			CycleFinished = false;
			
		}
		
		//Toggle gear animation.
		if ( ("" != ToggleLandingGearAnimationName) && (null!=ToggleLandingGearAnimationGameObject) )
		{
			//Play landing gear animation.
			Animation animation = ToggleLandingGearAnimationGameObject.GetComponent<Animation>();
			if ( animation )
			{	
				if ( animation[ToggleLandingGearAnimationName].normalizedTime < 0.0f ||  animation[ToggleLandingGearAnimationName].normalizedTime > 1.0f )
				{
					animation.Stop( ToggleLandingGearAnimationName);
					if ( null != ToggleLandingGear )
					{
						ToggleLandingGear.Stop();
						ToggleLandingGear.time = 0.0f;
					}
					
					CycleFinished = true;
				}
			}
		}
	
	}
}
