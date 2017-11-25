//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/External/Wheel Audio")]
[RequireComponent(typeof(WheelCollider))]
public class WheelAudio : MonoBehaviour 
{
	public AudioClip WheelRollClip = null;
	public float RPMForMaxVolume = 1000.0f;
	public float MaxVolume = 0.25f;
		
	private AudioSource WheelRoll = null;
	private WheelCollider Wheel = null;
	
	// Use this for initialization
	void Start () 
	{
		if ( null != WheelRollClip )
		{
			WheelRoll = gameObject.AddComponent<AudioSource>();
			WheelRoll.clip = WheelRollClip;
			WheelRoll.loop = true;
			WheelRoll.volume = 0.0f;
			WheelRoll.Play();
			WheelRoll.dopplerLevel = 0.0f;
		}
		
		Wheel = gameObject.GetComponent<WheelCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( null != Wheel )
		{
			if ( Wheel.enabled && Wheel.isGrounded )
			{
				float volume = (Wheel.rpm / RPMForMaxVolume) * MaxVolume;
				volume = Mathf.Clamp( volume, 0.0f, 1.0f );
				WheelRoll.volume = volume;
			}
			else
			{
				WheelRoll.volume = 0.0f;
			}
		}
	}
}
