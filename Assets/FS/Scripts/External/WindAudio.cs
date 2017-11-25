//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/External/Wind Audio")]
[RequireComponent(typeof(Rigidbody))]
public class WindAudio : MonoBehaviour 
{
	public AudioClip WindClip = null;
	public float VelocityForMaxVolume = 100.0f;
	public float MaxVolume = 0.5f;
	
	public float MinPitch = 0.5f;
	public float MaxPitch = 1.5f;
		
	private AudioSource Wind = null;
	private Rigidbody AicraftRigdbody = null;
	
	// Use this for initialization
	void Start () 
	{
		if ( null != WindClip )
		{
			Wind = gameObject.AddComponent<AudioSource>();
			Wind.clip = WindClip;
			Wind.loop = true;
			Wind.volume = 0.0f;
			Wind.Play();
			Wind.dopplerLevel = 0.0f;
		}
		
		AicraftRigdbody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( null != AicraftRigdbody )
		{
			float volume = (AicraftRigdbody.velocity.magnitude / VelocityForMaxVolume) * MaxVolume;
			volume = Mathf.Clamp( volume, 0.0f, 1.0f );
			Wind.volume = volume;
			
			float pitch = MinPitch + ((AicraftRigdbody.velocity.magnitude / VelocityForMaxVolume) * (MaxPitch-MinPitch));
			pitch = Mathf.Clamp( pitch, MinPitch, MaxPitch );
			Wind.pitch = pitch;
		}
	}
}
