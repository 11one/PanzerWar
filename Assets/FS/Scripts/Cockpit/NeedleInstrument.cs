//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;


[AddComponentMenu("UnityFS/Cockpit/Needle Instrument")]
public class NeedleInstrument : MonoBehaviour 
{
	public enum InstrumentSource
	{
		AirspeedKnots = 0,
		AltitudeThousandsFeet,
		AltitudeHundredsFeet,
		RateOfClimbFPM,
		EngineRPM,
		Heading,
		Bank,
	}
	
	public InstrumentSource Source = InstrumentSource.AirspeedKnots;
	public float MinValue = 0.0f;
	public float MaxValue = 1.0f;
	public float MinAngleDegrees = 0.0f;
	public float MaxAngleDegrees = 360.0f;
	public Vector3 RotationAxis = new Vector3(0.0f, 0.0f, 1.0f );
	
	private Quaternion InitialRotation = Quaternion.identity;
	private Aircraft Parent = null;

	// Use this for initialization
	void Start () 
	{
		RotationAxis.Normalize();
		
		//Store root rotation.
		InitialRotation = gameObject.transform.localRotation;
		
		//Get aeroplane.
		GameObject root = gameObject.transform.root.gameObject;
		Parent = root.GetComponent<Aircraft>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//Get instrument value.
		float instrumentValue = 0.0f;
		switch (Source)
		{
			case InstrumentSource.AirspeedKnots:
			{
				instrumentValue = Parent.GetAirspeedKnots();	
			}
			break;
			
			case InstrumentSource.AltitudeThousandsFeet:
			{
				instrumentValue = Parent.GetAltitudeThousandsFeet();
				float subtractAmount = Mathf.Floor(instrumentValue/10.0f);
				instrumentValue -= subtractAmount * 10.0f;
			}
			break;
			
			case InstrumentSource.AltitudeHundredsFeet:
			{
				instrumentValue = Parent.GetAltitudeHundredsFeet();
				float subtractAmount = Mathf.Floor(instrumentValue/10.0f);
				instrumentValue -= subtractAmount * 10.0f;
			}
			break;
			
			case InstrumentSource.RateOfClimbFPM:
			{
				instrumentValue = Parent.GetRateOfClimbFPM();	
			}
			break;
			
			case InstrumentSource.EngineRPM:
			{
				instrumentValue = Parent.GetEngineRPM();	
			}
			break;
			
			case InstrumentSource.Heading:
			{
				instrumentValue = Parent.GetHeadingDegrees();	
			}
			break;
			
			case InstrumentSource.Bank:
			{
				instrumentValue = Parent.GetBankDegrees();	
				
				if ( instrumentValue > 180.0f )
				{
					instrumentValue = -(360.0f - instrumentValue);
				}
			
			}
			break;
		}
		
		//Allow force max value to quickly set up instruments when editing them.
		instrumentValue = Mathf.Clamp( instrumentValue, MinValue, MaxValue );
		
		float valueDelta = (instrumentValue-MinValue) / (MaxValue - MinValue);
		valueDelta = Mathf.Clamp( valueDelta, 0.0f, 1.0f );
		 
		float angleDeltaDegrees = MinAngleDegrees + ((MaxAngleDegrees - MinAngleDegrees) * valueDelta);

		gameObject.transform.localRotation = InitialRotation;
		gameObject.transform.Rotate(RotationAxis,angleDeltaDegrees);
	}
}
