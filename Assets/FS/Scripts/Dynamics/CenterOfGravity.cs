//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/Dynamics/Center Of Gravity")]
public class CenterOfGravity : AircraftAttachment 
{
	private GameObject Parent = null;

	// Use this for initialization
	public void Start () 
	{
		//Get the topmost parent
		Parent = gameObject.transform.root.gameObject;
	}
	
	// Update is called once per frame
	public void Update () 
	{		
		//Update center of mass.
		Rigidbody rigidBody = Parent.GetComponent<Rigidbody>();
		if ( null != rigidBody )
		{
			rigidBody.centerOfMass = gameObject.transform.localPosition;
		}
		
		//Debug draw.
		Debug.DrawLine( gameObject.transform.position - ( gameObject.transform.up * 1.0f ), gameObject.transform.position + ( gameObject.transform.up * 1.0f ), Color.blue );
		Debug.DrawLine( gameObject.transform.position - ( gameObject.transform.right * 1.0f ), gameObject.transform.position + ( gameObject.transform.right * 1.0f ), Color.blue );
	}
	
	public void OnDrawGizmos() 
	{
		//Draw sphere at cg position.
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (transform.position, 0.1f);
    }
}

