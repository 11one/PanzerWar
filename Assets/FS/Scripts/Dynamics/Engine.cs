//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Dynamics/Engine")]
public class Engine : AircraftAttachment 
{
	public enum EngineState
	{
		Off,
		Starting,
		Running
	}
	
	public Transform AnimatedPropellerPivot = null;
	public Vector3 AnimatedPropellerPivotRotateAxis = Vector3.forward;
	public GameObject SlowPropeller = null;
	public GameObject FastPropeller = null;
	public float RPMToUseFastProp = 300.0f;
	
	public float IdleRPM = 400.0f;
	public float MaxRPM = 2800.0f;
	public float ForceAtMaxRPM = 4000.0f;
	public AnimationCurve PercentageForceAppliedVSAirspeedKTS = null;
	public float RPMToAddPerKTOfSpeed = 10.0f;
	
	public float RPMLerpSpeed = 1.5f;

	public AudioClip EngineStartClip = null;
	public AudioClip EngineRunClip = null;
	public float PitchAtIdleRPM = 0.5f;
	public float PitchAtMaxRPM = 1.0f;
	
	public EngineState CurrentEngineState = EngineState.Off;
	
	[HideInInspector]
	public Vector3 Thrust = Vector3.zero;
	
	[HideInInspector]
	public InputController ThrottleController = new InputController();
	public InputController EngineStartController = new InputController();
	
	private float CurrentRPM = 0.0f;	
	private Rigidbody Parent = null;
	private float DesiredRPM = 0.0f;
	
	private AudioSource EngineStart = null;
	private AudioSource EngineRun = null;
	private float EngineRunVolume = 0.0f;
	
	public float GetRPM()
	{
		return CurrentRPM;
	}
	
	public void Start () 
	{
		Parent = transform.root.gameObject.GetComponent<Rigidbody>();
		//Parent.velocity = new Vector3(0, 0, 50);
		if ( null != EngineStartClip )
		{
			EngineStart = gameObject.AddComponent<AudioSource>();
			EngineStart.clip = EngineStartClip;
			EngineStart.loop = false;
			EngineStart.dopplerLevel = 0.0f;
		}
		
		if ( null != EngineRunClip )
		{
			EngineRun = gameObject.AddComponent<AudioSource>();
			EngineRun.clip = EngineRunClip;
			EngineRun.loop = true;
			EngineRun.Play();
			EngineRunVolume = EngineRun.volume;
			EngineRun.dopplerLevel = 0.0f;
		}
	}
	
	public void Update () 
	{
		if ( Parent )
		{
			switch ( CurrentEngineState )
			{
				case EngineState.Off:
				{
					UpdateOff();
				}
				break;
				
				case EngineState.Starting:
				{
					UpdateStarting();
				}
				break;
				
				case EngineState.Running:
				{
					UpdateRunning();
				}
				break;
			}
			
			//Lerp current rpm to desired rpm.
			CurrentRPM = Mathf.Lerp( CurrentRPM, DesiredRPM, RPMLerpSpeed * Time.deltaTime );
			
			//Update audio based on RPM. (Doesn't matter if it's not playing i.e engine off )
			if ( null != EngineRun )
			{
				float velocity = Parent.velocity.magnitude;
				float velocityKTS = velocity * 1.943844492f;
					
				float CurrentPitchRPM = CurrentRPM + (velocityKTS * RPMToAddPerKTOfSpeed);
				
				float rpmOffset = (CurrentPitchRPM - IdleRPM) / (MaxRPM - IdleRPM );
				
				float enginePitch = PitchAtIdleRPM + ( (PitchAtMaxRPM-PitchAtIdleRPM) * rpmOffset );
				EngineRun.pitch = enginePitch;
				
				//If below idle fade out engine run sound..
				if ( CurrentRPM < IdleRPM )
				{
					EngineRun.volume = EngineRunVolume * ( CurrentRPM / IdleRPM );
					
					if ( CurrentRPM < (IdleRPM * 0.1f) )
					{
						EngineRun.volume = 0.0f;
					}
				}
				else
				{
					EngineRun.volume = EngineRunVolume;
				}
				
			}
			
			//Set the correct propeller visibility.
			if ( SlowPropeller && FastPropeller )
			{
				if ( CurrentRPM > RPMToUseFastProp )
				{
					SlowPropeller.GetComponent<Renderer>().enabled = false;
					FastPropeller.GetComponent<Renderer>().enabled = true;
				}
				else
				{
					SlowPropeller.GetComponent<Renderer>().enabled = true;
					FastPropeller.GetComponent<Renderer>().enabled = false;
				}
			}
			
			//Rotate the propeller hub.
			if ( null != AnimatedPropellerPivot )
			{
				float rotationThisFrame = (( CurrentRPM * 360.0f ) / 60.0f ) * Time.deltaTime;
				AnimatedPropellerPivot.transform.Rotate( AnimatedPropellerPivotRotateAxis,rotationThisFrame );
			}
		}
	}
	
	
	public void FixedUpdate()
	{
		if ( Parent )
		{
			float forceMultiplier = (CurrentRPM-IdleRPM) / (MaxRPM-IdleRPM);
			forceMultiplier = Mathf.Clamp( forceMultiplier, 0.0f, 1.0f );
			
			float velocity = Parent.velocity.magnitude;
			float velocityKTS = velocity * 1.943844492f;
			float thrustPercent = PercentageForceAppliedVSAirspeedKTS.Evaluate( velocityKTS ) * 0.01f; //Convert to zero to one.
			
			Thrust = (transform.forward * ( ForceAtMaxRPM * forceMultiplier )) * thrustPercent;
			Parent.AddForceAtPosition(Thrust, transform.position, ForceMode.Force);
		}
	}
	 
	private void UpdateOff()
	{
		if ( EngineStart.isPlaying )
		{
			EngineStart.Stop();
		}
			
		if( Controllable )
		{
			//Listen for engine start and trigger start sound..
			if ( EngineStartController.GetButton() )
			{
				EngineStart.Play();
				CurrentEngineState = EngineState.Starting;
			}
		}
		
		//Spin down blades.
		DesiredRPM = 0.0f;
	}
	
	private void UpdateStarting()
	{
		//If start is held througout start procedure run engine else stop.
		if ( Controllable && EngineStartController.GetButton() )
		{
			if ( !EngineStart.isPlaying )
			{
				CurrentEngineState = EngineState.Running;
			}
		}
		else
		{
			EngineStart.Stop();
			CurrentEngineState = EngineState.Off;
		}
		
		//Spin up blades to idle.
		DesiredRPM = IdleRPM;
	}
	
	private void UpdateRunning()
	{
		if ( EngineStart.isPlaying )
		{
			EngineStart.Stop();
		}
		
		if( Controllable )
		{
			//Control throttle.
			float input = ThrottleController.GetAxisInput();
			input = Mathf.Clamp( input, 0.0f, 1.0f );
			DesiredRPM = IdleRPM + ( ( MaxRPM - IdleRPM ) * input );
			
			//Start stop engine.
			if ( EngineStartController.GetButtonPressed() )
			{
				//EngineRun.Stop();
				CurrentEngineState = EngineState.Off;
			}
		}
	}
	
	
	
	public void OnDrawGizmos() 
	{
		//Draw icon.
		Gizmos.DrawIcon (transform.position, "prop.png", true);
		
		if ( null != PercentageForceAppliedVSAirspeedKTS )
		{
			//Add two keys at the start and end..
			if ( PercentageForceAppliedVSAirspeedKTS.keys.Length < 2 )
			{
				 PercentageForceAppliedVSAirspeedKTS.AddKey( 0.0f, 100.0f );
				 PercentageForceAppliedVSAirspeedKTS.AddKey( 300.0f, 100.0f );
			}
		}
	}
}
