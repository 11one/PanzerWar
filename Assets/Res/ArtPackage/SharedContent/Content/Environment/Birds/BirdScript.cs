using UnityEngine;
using System.Threading;
using System.Collections;

public class BirdScript : MonoBehaviour {
	#if ClientCode
	public GameObject brid,lookat;
	float Distance;
	Transform NowPath;
	AudioSource audioSource;
	void Start(){
		NowPath = FindPath ();
		InvokeRepeating ("DistanceDective",0,5);
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(BridSound());

	}
	Transform FindPath(){
		GameObject [] Paths = GameObject.FindGameObjectsWithTag ("BirdPath");
		return Paths [Random.Range (0, Paths.Length)].transform;
	}
	void Update(){
		BirdMove ();
	}
	private void BirdMove(){
		brid.transform.Translate (0, 0, 10 * Time.deltaTime);
		lookat.transform.LookAt (NowPath);
		brid.transform.rotation = Quaternion.Lerp (brid.transform.rotation, lookat.transform.rotation, 0.001f);
	}
	private void DistanceDective(){
		Distance = Vector3.Distance (brid.transform.position, NowPath.position);
		if (Distance < 20) {
			NowPath = FindPath ();
		}
	}
	IEnumerator BridSound(){
		if(audioSource.isPlaying == false)
			audioSource.Play();
		yield return new WaitForSeconds(Random.Range(0,25));
		StartCoroutine(BridSound());
	}
	#endif
}
