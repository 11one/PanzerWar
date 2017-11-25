//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using System.Collections;

[AddComponentMenu("UnityFS/External/Stall Warner")]
[RequireComponent(typeof(Wing))]
public class StallWarner : MonoBehaviour 
{
	public AudioClip StallWarnerClip = null;
	public float AngleOfAttackToTrigger = 12.0f;
	
	private Wing AttachedWing = null;
	private AudioSource StallWarnerSource = null;
	private Rigidbody Parent = null;
	// Use this for initialization
	void Start () 
	{
		Parent = transform.root.gameObject.GetComponent<Rigidbody>();
		
		AttachedWing = GetComponent<Wing>();
		
		if ( null != StallWarnerClip )
		{
			StallWarnerSource = gameObject.AddComponent<AudioSource>();
			StallWarnerSource.clip = StallWarnerClip;
			StallWarnerSource.volume = 1.0f;
			StallWarnerSource.loop = true;
			StallWarnerSource.dopplerLevel = 0.0f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( null != AttachedWing )
		{
			if ( AttachedWing.AngleOfAttack >= AngleOfAttackToTrigger )
			{
				if ( !StallWarnerSource.isPlaying )
				{
					if ( Parent.velocity.magnitude > 0.1f ) //only play if we are moving
					{
						StallWarnerSource.Play();
					}
				}
			}
			else
			{
				if ( StallWarnerSource.isPlaying )
					StallWarnerSource.Stop();
			}
		}
	}
}
