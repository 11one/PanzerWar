using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RespawnPointModule{
	public MonoBehaviour CorountineRunObject = null;

	List<Transform> UnAvailableForRespawn = new List<Transform> ();

	public Transform RandomStartPoint (GameObject[] ts, bool AirPoint, out Vector3 heightOffSet)
	{
		Vector3 HeightOffSet = Vector3.zero;
		Transform ResultTransform = null;
		List<Transform> AvailableStartPoints = new List<Transform> ();

		if (AirPoint) {
			HeightOffSet = new Vector3 (0, Random.Range (150, 250), 0);
		}

		heightOffSet = HeightOffSet;

		BasePlayerState[] AllVehicles = GameObject.FindObjectsOfType<BasePlayerState> ();

		foreach (GameObject startpoint in ts) {
			bool Available = true;

			foreach (BasePlayerState vehicle in AllVehicles) {
				if (AirPoint) {
					if (Vector3.Distance (vehicle.transform.position, startpoint.transform.position + HeightOffSet) < 50 || UnAvailableForRespawn.Contains (startpoint.transform)) {
						Available = false;
					}
				} else {
					if (Vector3.Distance (vehicle.transform.position, startpoint.transform.position) < 50 || UnAvailableForRespawn.Contains (startpoint.transform)) {
						Available = false;
					}
				}

			}

			if (UnAvailableForRespawn.Contains (startpoint.transform)) {
				Available = false;
			}
			if (Available)
				AvailableStartPoints.Add (startpoint.transform);
		}
		if (AvailableStartPoints.Count == 0) {
			ResultTransform = ts [UnityEngine.Random.Range (0, ts.Length)].transform;
		} else {
			ResultTransform = AvailableStartPoints [UnityEngine.Random.Range (0, AvailableStartPoints.Count)].transform;
			UnAvailableForRespawn.Add (ResultTransform);
			CorountineRunObject.StartCoroutine(UndoUnAvailableForRespawn (ResultTransform));
		}


		return	ResultTransform;
	}

	IEnumerator UndoUnAvailableForRespawn (Transform t)
	{
		yield return new WaitForSeconds (5);
		UnAvailableForRespawn.Remove (t);
	}
}