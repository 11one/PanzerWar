//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UnityFS/Aircraft")]
[RequireComponent(typeof(Rigidbody))]
public class Aircraft : MonoBehaviour 
{
	public bool AircraftEnabledAtStart = true;
	public bool OverrideInertiaTensor = false;
	public Vector3 InertiaTensor = new Vector3( 0.0f, 0.0f, 0.0f );
	public float RollwiseDamping = 1.0f;
	
	[HideInInspector]
	public InputController ChangeCameraController = new InputController();
	
	private bool AircraftEnabled = false;
	private int CurrentCameraIndex = 0;
	private AircraftAttachment[] AircraftAttachments = null;
	private AircraftCamera[] AircraftCameras = null;
	private Engine[] AircraftEngines = null;
	
	private GUIText AirspeedText = null;
	private GUIText AltitudeText = null;
	private GUIText RateOfClimbText = null;

	// Use this for initialization
	public virtual void Start () 
	{
		//Register a list of all attached parts and cameras..
		AircraftAttachments = GetComponentsInChildren<AircraftAttachment>();
		AircraftCameras = GetComponentsInChildren<AircraftCamera>();
		AircraftEngines	 = GetComponentsInChildren<Engine>();
		
		//Enable control if requested at start.
		if ( AircraftEnabledAtStart )
		{
			AircraftEnabled = true;
			EnableControl( true );
		}
		
		//Override inertia tensor if so desired.
		if ( OverrideInertiaTensor )
		{
			GetComponent<Rigidbody>().inertiaTensor = InertiaTensor;
		}
		
		//Clamp rollwise damping.
		RollwiseDamping  = Mathf.Clamp( RollwiseDamping, 0.0f, 1.0f );
		
		//Grab gui text for onscreen gui if it exists.
		GameObject Airspeed = GameObject.Find( "GUIAirspeed" );
		if ( null != Airspeed )
		{
			AirspeedText = Airspeed.GetComponent<GUIText>();
		}
		
		GameObject Altitude = GameObject.Find( "GUIAltitude" );
		if ( null != Altitude )
		{
			AltitudeText = Altitude.GetComponent<GUIText>();
		}
		
		GameObject RateOfClimb = GameObject.Find( "GUIRateOfClimb" );
		if ( null != RateOfClimb )
		{
			RateOfClimbText = RateOfClimb.GetComponent<GUIText>();
		}
		
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		if ( AircraftEnabled )
		{
			//Listen for input to swap cameras..
			if ( ChangeCameraController.GetButtonPressed() )
			{
				int previousCameraIndex = CurrentCameraIndex;
				
				CurrentCameraIndex++;
				if ( CurrentCameraIndex >= AircraftCameras.Length )
				{
					CurrentCameraIndex = 0;
				}
				
				AircraftCameras[previousCameraIndex].SetCameraActive(false);
				AircraftCameras[CurrentCameraIndex].SetCameraActive(true);
			}
			
			if ( AircraftEnabled )
			{
				if ( null !=AirspeedText )
				{
					AirspeedText.text = "Airspeed:" + ((int)GetAirspeedKnots()).ToString() + "kts";
				}
			
				if ( null !=AltitudeText )
				{
					AltitudeText.text = "Altitude:" + ((int)GetAltitude()).ToString() + "ft";
				}
		
				if ( null !=RateOfClimbText )
				{
					RateOfClimbText.text = "RateOfClimb:" + ((int)GetRateOfClimbFPM()).ToString() + "fpm";
				}
			}
		}
	}
	
	public void EnableControl( bool enable )
	{
		//Set all parts enabled.
		if ( null != AircraftAttachments )
		{
			foreach ( AircraftAttachment a in AircraftAttachments )
			{
				a.SetControllable( enable );
			}
		}
		
		//Enable start camera.
		CurrentCameraIndex = 0;
		if ( null != AircraftCameras )
		{
			for ( int i=0; i<AircraftCameras.Length; i++ )
			{
				//Always disable all cameras.
				AircraftCameras[i].SetCameraActive( false );
				
				if ( i == CurrentCameraIndex )
				{
					AircraftCameras[i].SetCameraActive( enable );
				}
			}
		}
	}
	
	public float GetAirspeedKnots()
	{
		return gameObject.GetComponent<Rigidbody>().velocity.magnitude * Conversions.MetersPerSecondToKnots;
	}
	
	public float GetAltitudeThousandsFeet()
	{
		return (gameObject.transform.position.y * Conversions.MetersToFeet)/1000.0f;
	}
	
	public float GetAltitudeHundredsFeet()
	{
		return (gameObject.transform.position.y * Conversions.MetersToFeet)/100.0f;
	}
	
	public float GetAltitude()
	{
		return (gameObject.transform.position.y * Conversions.MetersToFeet);
	}
	
	public float GetHeadingDegrees()
	{
		return gameObject.transform.eulerAngles.y;
	}
	
	public float GetBankDegrees()
	{
		return gameObject.transform.localEulerAngles.z;
	}
	
	public float GetRateOfClimbFPM()
	{
		float yRate = gameObject.GetComponent<Rigidbody>().velocity.y;
		yRate *= Conversions.MetersToFeet;
		yRate *= 60.0f;
		return yRate;
	}
	
	public float GetEngineRPM( int engineIndex )
	{
		float rpm = 0.0f;
		if ( null != AircraftEngines )
		{
			engineIndex = Mathf.Clamp( engineIndex, 0, AircraftEngines.Length -1 );
			
			if ( null != AircraftEngines[engineIndex] )
			{
				rpm = AircraftEngines[engineIndex].GetRPM();
			}
		}
		return rpm;
	}
	
	public float GetEngineRPM()
	{
		return GetEngineRPM(0);
	}
	
	
}
	

public class Conversions
{
	public static float MetersPerSecondToKnots = 1.94f;
	public static float MetersToFeet = 3.28f;
}