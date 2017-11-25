//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;


[AddComponentMenu("UnityFS/Dynamics/Wing")]
[RequireComponent( typeof(BoxCollider) )]
public class Wing : AircraftAttachment 
{
	public Aerofoil Aerofoil = null;
	
	public int SectionCount = 10;
	public float WingTipWidthZeroToOne = 1.0f;
	public float WingTipSweep = 0.0f;
	public float WingTipAngle = 0.0f;
	
	public float CDOverride = 0.045f;
	
	[HideInInspector]
	public float WingArea = 0.0f;
	[HideInInspector]
	public float AngleOfAttack = 0.0f;
	
	private BoxCollider WingBoxCollider = null;
	private Vector3 WingRootLeadingEdge = Vector3.zero;
	private Vector3 WingRootTrailingEdge = Vector3.zero;
	private Vector3 WingTipLeadingEdge = Vector3.zero;
	private Vector3 WingTipTrailingEdge = Vector3.zero;
	private Vector3 RootLiftPosition = Vector3.zero;
	private Vector3 TipLiftPosition = Vector3.zero;
	private float LiftLineChordPosition = 0.75f;
	
	private Rigidbody Parent = null;
	private Aircraft ParentAircraft = null;
	private ControlSurface AttachedControlSurface = null;
	private PropWash AttachedPropWash = null;
	private GroundEffect AttachedGroundEffect = null;
	
	// Use this for initialization
	public void Start () 
	{
		WingBoxCollider = (BoxCollider)gameObject.GetComponent<Collider>();
		Parent = transform.root.gameObject.GetComponent<Rigidbody>();
		ParentAircraft = transform.root.gameObject.GetComponent<Aircraft>();
		AttachedControlSurface = gameObject.GetComponent<ControlSurface>();
		AttachedPropWash = gameObject.GetComponent<PropWash>();
		AttachedGroundEffect = gameObject.GetComponent<GroundEffect>();
	}
	
	public void FixedUpdate()
	{
		float debugLineScale = 1.0f / 30.0f;
				
		//Calculate position of wing points in worldspace this frame.
		UpdateWingGeometry();
		
		
		//Per section update.
		for ( int i=0; i<SectionCount; i++ )
		{		
			//R A-----------------B (Leading edge)
			//O |                 |  
			//O |                 |
			//T D-----------------C (Trailing edge

			
			//Find points a,b,c & d for this chunk of wing.
			Vector3 a = WingRootLeadingEdge + ( (WingTipLeadingEdge - WingRootLeadingEdge) * (float)i/(float)SectionCount );
			Vector3 b = WingRootLeadingEdge + ( (WingTipLeadingEdge - WingRootLeadingEdge) * (float)(i+1)/(float)SectionCount );
			Vector3 c = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge) * (float)(i+1)/(float)SectionCount );
			Vector3 d = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge) * (float)i/(float)SectionCount );
			
			//Draw premodified wing.
//			Debug.DrawLine( a, b );
//			Debug.DrawLine( b, c );
//			Debug.DrawLine( c, d );
//			Debug.DrawLine( d, a );
			
			//If we have a control surface attached update the geometry based on control surface inputs.
			if ( null != AttachedControlSurface )
			{
				AttachedControlSurface.ModifyWingGeometry( i, ref a, ref b, ref c, ref d);
			}
			
			//Draw modified wing.
			Debug.DrawLine( a, b );
			Debug.DrawLine( b, c );
			Debug.DrawLine( c, d );
			Debug.DrawLine( d, a );
			
			//Recalculate lift positions..
			Vector3 sectionRootLiftPosition = d + ( ( a - d ) * LiftLineChordPosition );
			Vector3 sectionTipLiftPosition = c + ( ( b - c ) * LiftLineChordPosition );
			
			//Find the aerodynamic center.
			Vector3 aerodynamicCenter = sectionRootLiftPosition + ( ( sectionTipLiftPosition - sectionRootLiftPosition ) * 0.5f );
			
			//Find the chord line.
			Vector3 chordLine = ( a + ((b-a) * 0.5f )) -  ( d + ((c-d) * 0.5f) );
			float chordLength = chordLine.magnitude;
			chordLine.Normalize();
			
			Debug.DrawLine( aerodynamicCenter, aerodynamicCenter + chordLine, Color.blue );
			
			//Get relative wind.
			Vector3 relativeWind = -Parent.velocity;
					
			//Calculate angular to linear velocity of any rotation and add to relative wind.
			Vector3 fromCOMToAerodynamicCenter = aerodynamicCenter -  Parent.worldCenterOfMass;
			Vector3 angularVelocity = Parent.angularVelocity;
			
			Vector3 localRelativeWind = Vector3.Cross( angularVelocity.normalized, fromCOMToAerodynamicCenter.normalized );
			localRelativeWind *= -((angularVelocity.magnitude) * fromCOMToAerodynamicCenter.magnitude);
			
			//Tweak rollwise damping based on parent aircraft if it exists.
			if ( null != ParentAircraft )
			{
				localRelativeWind *= ParentAircraft.RollwiseDamping;
			}
			
			//Apply
			relativeWind += localRelativeWind;
			
			//Propwash
			if ( null != AttachedPropWash )
			{
				if ( (null != AttachedPropWash.PropWashSource) && AttachedPropWash.AffectedSections[i] )
				{
					relativeWind += -AttachedPropWash.PropWashSource.Thrust * AttachedPropWash.PropWashStrength;
				}
			}
			
			//Tweak relative wind so we only consider that which is flowing over the wing.
			Debug.DrawLine( aerodynamicCenter - (relativeWind.normalized), aerodynamicCenter, Color.grey );  
			Vector3 correction = gameObject.transform.right;
			float perpChordDotRelativeWind = Vector3.Dot( correction, relativeWind );
			correction *= perpChordDotRelativeWind;
			relativeWind -= correction;
			Debug.DrawLine( aerodynamicCenter - relativeWind.normalized, aerodynamicCenter, Color.white );  
			
			
			//Find the angle of attack.	
			Vector3 relativeWindNormalized = relativeWind.normalized;
			AngleOfAttack = Vector3.Dot(chordLine, -relativeWindNormalized);
			AngleOfAttack = Mathf.Clamp( AngleOfAttack, -1.0f, 1.0f );
			AngleOfAttack = Mathf.Acos(AngleOfAttack);
			AngleOfAttack *= Mathf.Rad2Deg;
			
			
			Vector3 up = Vector3.Cross( chordLine, (sectionTipLiftPosition-sectionRootLiftPosition).normalized );
			up.Normalize();
			
			if ( transform.localScale.x < 0.0f )
			{
				up=-up;
			}
			
			float yAxisDotRelativeWind = Vector3.Dot( up, relativeWindNormalized );		
			if ( yAxisDotRelativeWind < 0.0f )
			{
				AngleOfAttack = -AngleOfAttack;
			}
			
			float totalLift = 0.0f;
			float totalDrag = 0.0f;
			float cM = 0.0f;
			
			float clGroundEffectMult = 1.0f;
			float cdGroundEffectMult = 1.0f;
			
			if ( null != AttachedGroundEffect )
			{
				AttachedGroundEffect.GetGroundEffectCoefficients( a, b, c, d, out clGroundEffectMult, out cdGroundEffectMult );
			}
			
			if ( null != Aerofoil )
			{
				//Use aerofoil..
				
				//L = cl * a * 0.5f * r * v^2
				float cL = Aerofoil.CL.Evaluate(AngleOfAttack);	
				cL *= clGroundEffectMult;
				
				float area = CalculateArea( a, b, c, d );
				float r = 1.29f;
				float v = relativeWind.magnitude;	
				totalLift = cL * area * 0.5f * r * (v*v);
				
				//D = 0.5f * cd * r * v2 * a;
				float cD = Aerofoil.CD.Evaluate(AngleOfAttack);
				cD *= cdGroundEffectMult;
				
				totalDrag = 0.5f * cD * r * (v*v) * area;
				
				cM = Aerofoil.CM.Evaluate(AngleOfAttack);
			}
			else
			{
				//Fall back to basic l/d equations..
				//L = cl * a * 0.5f * r * v^2
				//Approximate Cl using the following formula - Cl = 2 * pi * angle (in radians)
				float cL = 2.0f * Mathf.PI * (AngleOfAttack * Mathf.Deg2Rad); 
				cL *= clGroundEffectMult;
				
				float area = CalculateArea( a, b, c, d );
				float r = 1.29f;
				float v = relativeWind.magnitude;	
				totalLift = cL * area * 0.5f * r * (v*v);
				
				
				//D = 0.5f * cd * r * v2 * a;
				//Typical aerofoil drag co efficient is .045;
				float cD = CDOverride; //Typical aerofoil drag co efficient
				cD *= cdGroundEffectMult;
				
				totalDrag = 0.5f * cD * r * (v*v) * area;
				
				cM = 0.0f;
			}
			

			//Build Lift vector.
			Vector3 liftForce = Vector3.Cross( gameObject.transform.right, relativeWind );
			liftForce.Normalize();
			liftForce *= totalLift;
			Debug.DrawLine( aerodynamicCenter, aerodynamicCenter + (liftForce*Time.deltaTime*debugLineScale), Color.green );
			
		
			//Build Drag vector.
			Vector3 dragForce = relativeWind;
			dragForce.Normalize();
			dragForce *= totalDrag;
			Debug.DrawLine( aerodynamicCenter, aerodynamicCenter + (dragForce*Time.deltaTime*debugLineScale), Color.red );
			
			//LiftMoment.
			Vector3 liftDragPoint = aerodynamicCenter;
			
			//Find wing pitching moment...
			float wingPitchingMoment = cM * chordLength * ( 0.5f *  1.29f * (relativeWind.magnitude*relativeWind.magnitude) ) * CalculateArea( a, b, c, d );
			Vector3 pitchAxis = Vector3.Cross( chordLine, liftForce.normalized );
			pitchAxis.Normalize();
			pitchAxis *= wingPitchingMoment;
			
			//Apply forces.
			Parent.AddForceAtPosition( liftForce, liftDragPoint, ForceMode.Force );
			Parent.AddForceAtPosition( dragForce, liftDragPoint, ForceMode.Force );		
			Parent.AddTorque(pitchAxis, ForceMode.Force );
		}
		
	}
	
	public void OnDrawGizmos() 
	{
		//Draw icon.
		Gizmos.DrawIcon (transform.position, "wing.png", true);
		
		WingBoxCollider = (BoxCollider)gameObject.GetComponent<Collider>();
		if ( null != WingBoxCollider )
		{
			//Clamp box collider scales.
			WingBoxCollider.size = new Vector3( 1.0f, 0.1f, 1.0f );
			
			UpdateWingGeometry();
			
			//Wing geometry.
			Gizmos.color = Color.blue;
			Gizmos.DrawLine( WingRootLeadingEdge, WingTipLeadingEdge );
			
			Gizmos.color = Color.red;
	        Gizmos.DrawLine( WingTipTrailingEdge, WingRootTrailingEdge );
			
			Gizmos.color = Color.blue;
	        Gizmos.DrawLine( WingRootTrailingEdge, WingRootLeadingEdge );
	        Gizmos.DrawLine( WingTipLeadingEdge, WingTipTrailingEdge );
			
			
			//Sections.
			Gizmos.color = Color.blue;
			for ( int i=0; i<SectionCount; i++ )
			{
				Vector3 sectionStart = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge) * (float)i/(float)SectionCount );
				Vector3 sectionEnd = WingRootLeadingEdge + ( (WingTipLeadingEdge - WingRootLeadingEdge) * (float)i/(float)SectionCount );
				Gizmos.DrawLine( sectionStart, sectionEnd );
			}
			
			//Lift line.
			Gizmos.color = Color.green;
			Gizmos.DrawLine( RootLiftPosition, TipLiftPosition );
			
			//Aileron hinge
			AttachedControlSurface = gameObject.GetComponent<ControlSurface>();
			if ( null != AttachedControlSurface )
			{
				float rootHingeOffset = AttachedControlSurface.RootHingeDistanceFromTrailingEdge;
				float tipHingeOffset = AttachedControlSurface.TipHingeDistanceFromTrailingEdge; 

				Vector3 wingRootAileronHingePos = WingRootTrailingEdge + ( ( WingRootLeadingEdge - WingRootTrailingEdge ) * rootHingeOffset );
				Vector3 wingTipAileronHingePos = WingTipTrailingEdge + ( ( WingTipLeadingEdge - WingTipTrailingEdge ) * tipHingeOffset );
				
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine( wingRootAileronHingePos, wingTipAileronHingePos );
				
				//Control surface - Draw crosses over each control surface section which is affected.
				if ( null != AttachedControlSurface.AffectedSections )
				{
					for ( int i=0; i<AttachedControlSurface.AffectedSections.Length; i++ )
					{
						if ( AttachedControlSurface.AffectedSections[i] == true )
						{
							Vector3 hingeLeft = wingRootAileronHingePos + ( (wingTipAileronHingePos - wingRootAileronHingePos ) * ((float)i / (float)AttachedControlSurface.AffectedSections.Length) );
							Vector3 hingeRight = wingRootAileronHingePos + ( (wingTipAileronHingePos - wingRootAileronHingePos ) * ((float)(i+1) / (float)AttachedControlSurface.AffectedSections.Length) );
							
							Vector3 backLeft = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge ) * ((float)i / (float)AttachedControlSurface.AffectedSections.Length) );
							Vector3 backRight = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge ) * ((float)(i+1) / (float)AttachedControlSurface.AffectedSections.Length) );
							
							Gizmos.DrawLine( hingeLeft, backRight );
							Gizmos.DrawLine( hingeRight, backLeft );
						}
					}
				}
			}
			
			//Prop wash - Draw crosses over each control surface section which is affected.
			AttachedPropWash = gameObject.GetComponent<PropWash>();
			if ( null != AttachedPropWash )
			{
				Gizmos.color = Color.cyan;
				if ( null != AttachedPropWash.AffectedSections )
				{
					for ( int i=0; i<AttachedPropWash.AffectedSections.Length; i++ )
					{
						if ( AttachedPropWash.AffectedSections[i] == true )
						{
							Vector3 frontLeft = WingRootLeadingEdge + ( (WingTipLeadingEdge - WingRootLeadingEdge ) * ((float)i / (float)AttachedPropWash.AffectedSections.Length) );
							Vector3 frontRight = WingRootLeadingEdge + ( (WingTipLeadingEdge - WingRootLeadingEdge ) * ((float)(i+1) / (float)AttachedPropWash.AffectedSections.Length) );
							
							Vector3 backLeft = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge ) * ((float)i / (float)AttachedPropWash.AffectedSections.Length) );
							Vector3 backRight = WingRootTrailingEdge + ( (WingTipTrailingEdge - WingRootTrailingEdge ) * ((float)(i+1) / (float)AttachedPropWash.AffectedSections.Length) );
							
							//Vector3 topCenter = hingeLeft + ( (hingeRight-hingeLeft) * 0.5f );
							//Vector3 bottomCenter = backLeft + ( (backRight-backLeft) * 0.5f );
							///Vector3 leftCenter = backLeft + ( (hingeLeft-backLeft) * 0.5f );
							//Vector3 rightCenter = backRight + ( (hingeRight-backRight) * 0.5f );
							
							Gizmos.DrawLine( frontLeft, backRight );
							Gizmos.DrawLine( frontRight, backLeft );
						}
					}
				}
			}
			
		}
    }
	
	private void UpdateWingGeometry()
	{
		//Calculate root and tip center points.
		Vector3 wingRootCenter = transform.position - ( transform.right * (transform.localScale.x * 0.5f) );
		Vector3 wingTipCenter = transform.position + ( transform.right * (transform.localScale.x * 0.5f) );
		wingTipCenter += transform.forward * WingTipSweep;
				
		//Calculate corners.
		WingRootLeadingEdge = wingRootCenter + ( transform.forward * (transform.localScale.z * 0.5f) );
		WingRootTrailingEdge = wingRootCenter - ( transform.forward * (transform.localScale.z * 0.5f) );
		WingTipLeadingEdge = wingTipCenter + ( transform.forward * ((transform.localScale.z * 0.5f) * WingTipWidthZeroToOne ) );
		WingTipTrailingEdge = wingTipCenter - ( transform.forward * ((transform.localScale.z * 0.5f) * WingTipWidthZeroToOne ) );
		
		
		//Tweak tip corners based on the angle between them.
		Vector3 tipTrailingEdgeToTipLeadingEdge = WingTipLeadingEdge - WingTipTrailingEdge;
		Quaternion rotation = Quaternion.AngleAxis( WingTipAngle, transform.rotation * new Vector3( 1.0f, 0.0f, 0.0f ));
		tipTrailingEdgeToTipLeadingEdge = rotation * tipTrailingEdgeToTipLeadingEdge;
		WingTipTrailingEdge = wingTipCenter - (tipTrailingEdgeToTipLeadingEdge * 0.5f);
		WingTipLeadingEdge = wingTipCenter + (tipTrailingEdgeToTipLeadingEdge * 0.5f);
		
		RootLiftPosition = WingRootTrailingEdge + ( ( WingRootLeadingEdge - WingRootTrailingEdge ) * LiftLineChordPosition );
		TipLiftPosition = WingTipTrailingEdge + ( ( WingTipLeadingEdge - WingTipTrailingEdge ) * LiftLineChordPosition );
		
		//Calculate wing area.
		WingArea = CalculateArea( WingRootLeadingEdge, WingTipLeadingEdge, WingTipTrailingEdge, WingRootTrailingEdge );
	}
	
	private float CalculateArea( Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD )
	{
		float ab = (pointB - pointA).magnitude;
		float bc = (pointC - pointB).magnitude;
		float cd = (pointD - pointC).magnitude;
		float da = (pointA - pointD).magnitude;
		
		float s = ( ab + bc + cd + da ) * 0.5f;
		float squareArea = (s-ab) * (s-bc) * (s-cd) * (s-da);
		float area = Mathf.Sqrt( squareArea );
		
		return area;
	}
}