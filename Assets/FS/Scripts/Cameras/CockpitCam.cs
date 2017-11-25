//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cameras/Cockpit Cam")]
[RequireComponent( typeof(Camera) )]
public class CockpitCam : AircraftCamera 
{
	public string ZoomInputAxis = "";
	public float LookXSpeed = 10.0f;
	public float LookYSpeed = -10.0f;
	public float ZoomSpeed = 100.0f;
	
	public float MinFOV = 1.0f;
	public float MaxFOV = 179.0f;
	
	private Quaternion startOrientation;
	private Vector3 lastMousePosition;
	private float yRotation;
	private float xRotation;
	
	// Use this for initialization
	public void Start () 
	{
		gameObject.GetComponent<Camera>().enabled = false;
		
		startOrientation = gameObject.transform.localRotation;
		lastMousePosition = Input.mousePosition;
		yRotation = 0.0f;
		xRotation = 0.0f;
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if ( CameraActive )
		{
			//Look around.
			if ( Input.GetMouseButton(0) )
			{
				Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
				yRotation += mouseDelta.x * LookXSpeed * Time.deltaTime;
				xRotation += mouseDelta.y * LookYSpeed * Time.deltaTime;
				
				gameObject.transform.localRotation = startOrientation;
				gameObject.transform.Rotate( transform.parent.right, xRotation, Space.World );
				gameObject.transform.Rotate( transform.parent.up, yRotation, Space.World);	
			}
			
			//Zoom.
			if ( ZoomInputAxis != "" )
			{
				gameObject.GetComponent<Camera>().fieldOfView += -Input.GetAxis(ZoomInputAxis) * ZoomSpeed * Time.deltaTime;
			}
			gameObject.GetComponent<Camera>().fieldOfView = Mathf.Clamp( gameObject.GetComponent<Camera>().fieldOfView, MinFOV, MaxFOV );
			lastMousePosition = Input.mousePosition;
			
			//Apply to main camera.
			Camera.main.transform.position = transform.position;
			Camera.main.transform.rotation = transform.rotation;
			
			Camera.main.fieldOfView = gameObject.GetComponent<Camera>().fieldOfView;
			Camera.main.nearClipPlane = gameObject.GetComponent<Camera>().nearClipPlane;
			Camera.main.farClipPlane = gameObject.GetComponent<Camera>().farClipPlane;
		}
	}
	
}
