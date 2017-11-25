using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DymRodTest : MonoBehaviour {
	public Vector3 Dir;
	public Animator PulldownAnimation;

	// Use this for initialization
	void Start () {
		Debug.DrawRay (transform.position, Dir);
		transform.rotation = Quaternion.LookRotation (Dir);
		//PulldownAnimation.Play ("Down");
		Debug.Break ();
	}
	

}
