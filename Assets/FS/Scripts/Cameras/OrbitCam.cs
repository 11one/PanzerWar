//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Cameras/Orbit Cam")]
[RequireComponent( typeof(Camera) )]
public class OrbitCam : AircraftCamera 
{
	public float CameraZOffset = 10.0f;
	public float CameraYOffset = 2.0f;
	
	private bool FirstClick = false;
	private Vector3 MouseStart;
	private float CameraAngle = 180.0f;
	
	private Aircraft TargetAeroplane = null;
	
	// Use this for initialization
	public void Start () 
	{
		gameObject.GetComponent<Camera>().enabled = false;
		
		TargetAeroplane = transform.root.gameObject.GetComponent<Aircraft>();
	}
	
	// Update is called once per frame
	public void Update () 
	{	
		if ( (CameraActive) && (null!=TargetAeroplane) )
		{
			if ( Input.GetMouseButton(0))
			{
				if ( FirstClick )
				{
					MouseStart = Input.mousePosition;
					FirstClick = false;
				}
				CameraAngle += (Input.mousePosition - MouseStart).x * Time.deltaTime;
			}
			else
			{
				FirstClick = true;
			}
				
			Vector3 zAxis = TargetAeroplane.transform.forward;
			zAxis.y = 0.0f;
			zAxis.Normalize();
			zAxis = Quaternion.Euler(0, CameraAngle, 0) * zAxis;
			
			Vector3 cameraPosition = TargetAeroplane.transform.position;
			cameraPosition += zAxis * CameraZOffset;
			cameraPosition += new Vector3(0.0f, 1.0f, 0.0f ) * CameraYOffset;
			
			Vector3 cameraTarget = TargetAeroplane.transform.position;
			
			//Apply to main camera.
			Camera.main.transform.position = cameraPosition;
			Camera.main.transform.LookAt( cameraTarget );
			
			Camera.main.fieldOfView = gameObject.GetComponent<Camera>().fieldOfView;
			Camera.main.nearClipPlane = gameObject.GetComponent<Camera>().nearClipPlane;
			Camera.main.farClipPlane = gameObject.GetComponent<Camera>().farClipPlane;
		}
	}
}
