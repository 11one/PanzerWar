using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LODManger : MonoBehaviour
{
	MeshFilter[] TankMeshes;
	MeshFilter[] LoDMeshes;
	public Transform LODMeshes, OriginMesh;
	public bool isMine;
	public static bool isFree = true;
	public void SetLOD (bool _isMine)
	{
		LODMeshes = LODMeshes.GetChild (0).transform;
		LODMeshes.gameObject.SetActive (true);
		LODMeshes.SetParent (transform.parent.parent);
		LODMeshes.transform.position = Vector3.zero - new Vector3 (0, 1000, 0);
		isMine = _isMine;
		StartCoroutine (StartReplaceLOD ());
	}

	public IEnumerator StartReplaceLOD ()
	{
		yield return new WaitForSeconds (10);
		if (!isFree) {
			yield return new WaitForSeconds (1);
		}
		isFree = false;

		if (transform.root.GetComponent<TankInitSystem> ().VehicleName == "IS-3"||transform.root.GetComponent<TankInitSystem> ().VehicleName == "Leopard1") {
			Destroy (LODMeshes);
			yield break;
		}

		LoDMeshes = LODMeshes.GetComponentsInChildren<MeshFilter> ();
		TankMeshes = OriginMesh.transform.GetComponentsInChildren<MeshFilter> ();
		List<MeshFilter> myUsedLODList = new List<MeshFilter> ();
		Debug.Log ("LOD");
		foreach (MeshFilter temp in TankMeshes) {
			#region LOD内部循环 通过Mesh 寻找LOD Meshes
			foreach (MeshFilter LodTemp in LoDMeshes) {
				bool WheelLod = false;

				if (temp.transform.parent.parent.name == "TankTransform")
					WheelLod = true;


				if (temp.sharedMesh.name == LodTemp.sharedMesh.name) {
					if (WheelLod)
						if (temp.transform.parent.name != LodTemp.transform.parent.name) {
							continue;
						} 
					
					myUsedLODList.Add (LodTemp);

					float Detail = 0.35f;

					if (WheelLod)
						Detail = 0.15f;

					GameObject LODGroupObj = new GameObject ("LOD", typeof(LODGroup));
					LODGroupObj.transform.parent = temp.transform.parent;
					LODGroupObj.transform.localPosition = new Vector3 ();
					LODGroupObj.transform.localEulerAngles = new Vector3 ();

					LodTemp.name += "_LOD";
					LodTemp.transform.SetParent (temp.transform.parent);
					LodTemp.transform.localPosition = temp.transform.localPosition;
					LodTemp.transform.localEulerAngles = temp.transform.localEulerAngles;


					LOD[] lod = new LOD[2];
					lod [0] = new LOD (Detail, new Renderer[]{ temp.GetComponent<Renderer> () });
					lod [1] = new LOD (0, new Renderer[]{ LodTemp.GetComponent<Renderer> () });
					LODGroupObj.GetComponent<LODGroup> ().SetLODs (lod);
					LODGroupObj.GetComponent<LODGroup>().RecalculateBounds();
				}
			
			}
			#endregion
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		Destroy (LODMeshes.gameObject);
		isFree = true;
		yield break;
	}
}
