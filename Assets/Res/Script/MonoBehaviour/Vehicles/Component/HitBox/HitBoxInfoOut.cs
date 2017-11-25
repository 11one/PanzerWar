using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitBoxInfoOut : MonoBehaviour {
	GameObject HitInfo;
	public Vector3 Target;
	Camera MainCamera;
	public string Value;
	public IEnumerator Start(){
		ResourceRequest ResourceTask = null;
		Object asset = null;
		ResourceTask = Resources.LoadAsync("HitInfo");
		yield return ResourceTask;
		asset = ResourceTask.asset;

		HitInfo =(GameObject)Instantiate((GameObject)asset,Target,Quaternion.identity);
		HitInfo.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
		HitInfo.GetComponentInChildren<Text>().text  = Value;
		HitInfo.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		MainCamera = Camera.main;
		Destroy(HitInfo,3.5f);
		Destroy(this.gameObject,3.5f);
	}

	Vector3 Offset = new Vector3(0,1,0);

	void OnGUI(){
		if(MainCamera == null||HitInfo == null)
			return;

		Offset+= new Vector3(0, 1.5f*Time.deltaTime,0);
		HitInfo.transform.position = MainCamera.WorldToScreenPoint(Target+Offset);
		if(HitInfo.transform.position.z <0){
			HitInfo.SetActive(false);
		}
		else {
			HitInfo.SetActive(true);
		}
	}


}
