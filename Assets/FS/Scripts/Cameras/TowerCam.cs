//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cameras/Tower Cam")]
[RequireComponent( typeof(Camera) )]
public class TowerCam : AircraftCamera 
{
	public string ZoomInputAxis = "";
	public float ZoomSpeed = 100.0f;
	public float MinFOV = 1.0f;
	public float MaxFOV = 179.0f;
	
	private Aircraft TargetAeroplane = null;
	private Vector3 IntitialWorldPosition = Vector3.zero;
	
	// Use this for initialization
	public void Start () 
	{
		TargetAeroplane = transform.root.gameObject.GetComponent<Aircraft>();
		IntitialWorldPosition = transform.position;
	}
	
	// Update is called once per frame
	public void Update () 
	{

		if ( CameraActive && (null != TargetAeroplane) )
		{
			//Reset back to correct world position.
			transform.position = IntitialWorldPosition;
			
			//Look at target.
			transform.LookAt( TargetAeroplane.transform.position, Vector3.up );
		
			//Do zoom.
			if ( ZoomInputAxis != "" )
			{
				gameObject.GetComponent<Camera>().fieldOfView += -Input.GetAxis("CameraZoom") * ZoomSpeed * Time.deltaTime;
			}
			gameObject.GetComponent<Camera>().fieldOfView = Mathf.Clamp( gameObject.GetComponent<Camera>().fieldOfView, MinFOV, MaxFOV );
			
			//Apply to main camera.
			Camera.main.transform.position = transform.position;
			Camera.main.transform.rotation = transform.rotation;
			
			Camera.main.fieldOfView = gameObject.GetComponent<Camera>().fieldOfView;
			Camera.main.nearClipPlane = gameObject.GetComponent<Camera>().nearClipPlane;
			Camera.main.farClipPlane = gameObject.GetComponent<Camera>().farClipPlane;
			
		}
	}
}
