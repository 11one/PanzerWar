//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//
	
using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Dynamics/Ground Effect")]
[RequireComponent (typeof(Wing))]
public class GroundEffect : AircraftAttachment 
{
	public AnimationCurve CLHeightVsChord = null;
	public AnimationCurve CDHeightVsSpan = null;
	public Vector3 RayCastAxis = new Vector3( 0.0f, -1.0f, 0.0f );
	public LayerMask RayCastLayers = (LayerMask)1; //Default layermask.
	public float Wingspan = 10;
	
	// Use this for initialization
	public void Start () 
	{
		RayCastAxis.Normalize();
	}
	
	public void GetGroundEffectCoefficients( Vector3 PointA, Vector3 PointB, Vector3 PointC, Vector3 PointD, out float clMultiplier, out float cdMultiplier )
	{
		clMultiplier = 1.0f;
		cdMultiplier = 1.0f;
		
		//R A-----------------B (Leading edge)
		//O |                 |  
		//O |                 |
		//T D-----------------C (Trailing edge
		
		//Get the center of the wing we will use this as our raycast position.
		Vector3 rootChordWiseCenter = PointD + ( ( PointA - PointD ) * 0.5f );
		Vector3 tipChordWiseCenter = PointC + ( ( PointB - PointC) * 0.5f );
		Vector3 center = rootChordWiseCenter + ( ( tipChordWiseCenter - rootChordWiseCenter ) * 0.5f );
		
		//Get the average chord length for the section.
		float averageChord = ((PointA - PointD).magnitude + (PointB - PointC).magnitude) * 0.5f;
		
		//Do a raycast along the center.
		float rayDistance = Wingspan;
		Vector3 castDirection = transform.rotation * RayCastAxis;
		Ray ray = new Ray( center, castDirection );
		
		Debug.DrawLine( center, center + (castDirection*rayDistance), Color.white );
		
		RaycastHit hitInfo = new RaycastHit();
		if ( Physics.Raycast( ray, out hitInfo, rayDistance, RayCastLayers ) )
		{
			//We have hit something check that the normal is inline with 
			float castDirectionDotNormal = Vector3.Dot( -castDirection, hitInfo.normal );
			float castDirectionDotNormalClamped = Mathf.Clamp( castDirectionDotNormal, 0.0f, 1.0f );
			
			float heightVsChord = hitInfo.distance / averageChord;
			heightVsChord = Mathf.Clamp( heightVsChord, 0.0f, 1.0f );
			
			//float heightVsSpan = hitInfo.distance / wingSectionSpan;
			float heightVsSpan = hitInfo.distance / Wingspan;
				
			heightVsSpan = Mathf.Clamp( heightVsSpan, 0.0f, 1.0f );
		
			//Use the normal to blend in the effect of ground effect. Closer it is to the cast direction
			//the more effect we want.
			float clLookUp = 1.0f - ( (1.0f - heightVsChord) * castDirectionDotNormalClamped );
			float cdLookUp = 1.0f - ( (1.0f - heightVsSpan) * castDirectionDotNormalClamped );
			
			Debug.DrawLine( hitInfo.point, hitInfo.point + (hitInfo.normal*clLookUp), Color.green );
			Debug.DrawLine( hitInfo.point, hitInfo.point + (hitInfo.normal*cdLookUp), Color.red );
			
			clMultiplier = CLHeightVsChord.Evaluate( clLookUp );
			cdMultiplier = CDHeightVsSpan.Evaluate( cdLookUp );
		}			
	}
}
