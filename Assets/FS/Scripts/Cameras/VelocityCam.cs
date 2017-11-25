//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cameras/Velocity Cam")]
[RequireComponent( typeof(Camera) )]
public class VelocityCam : AircraftCamera 
{
	public Vector3 StartOffset = new Vector3( 10.0f, 10.0f, 20.0f );
	public float ResetTimeSeconds = 15.0f;
	
	private Aircraft TargetAeroplane = null;
	private Vector3 CurrentPosition = Vector3.zero;
	private Vector3 Velocity = Vector3.zero;
	private float CurrentTime = 0.0f;
	
	// Use this for initialization
	public void Start () 
	{
		gameObject.GetComponent<Camera>().enabled = false;
		TargetAeroplane = transform.root.gameObject.GetComponent<Aircraft>();
	}
	
	protected override void OnCameraEnabled()
	{
		Reposition();
	}
	
	// Update is called once per frame
	public void FixedUpdate () 
	{	
		if ( (CameraActive) && (null!=TargetAeroplane) )
		{
			//Reset view if necessary.
			if ( CurrentTime > ResetTimeSeconds )
			{
				Reposition();
			}
			
			CurrentTime += Time.deltaTime;
			
			//Update camera position.
			CurrentPosition += Velocity * Time.deltaTime;
			
			Vector3 cameraTarget = TargetAeroplane.transform.position;
			
			//Apply to main camera.
			Camera.main.transform.position = CurrentPosition;
			Camera.main.transform.LookAt( cameraTarget );
			
			Camera.main.fieldOfView = gameObject.GetComponent<Camera>().fieldOfView;
			Camera.main.nearClipPlane = gameObject.GetComponent<Camera>().nearClipPlane;
			Camera.main.farClipPlane = gameObject.GetComponent<Camera>().farClipPlane;
		}
	}
	
	private void Reposition()
	{
		if ( null!=TargetAeroplane )
		{
			CurrentPosition = TargetAeroplane.transform.position + (  StartOffset );
			
			Velocity = TargetAeroplane.GetComponent<Rigidbody>().velocity;
			Velocity.y = 0.0f;
			
			CurrentTime = 0.0f;
		}
	}
}
