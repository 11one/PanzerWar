using UnityEngine;
using System.Collections;
public class MuzzleFlashManager : MonoBehaviour {
	[System.Serializable]
	public class LightProperty{
		public float LightDelay = 0,Intensity = 0,Range = 0,LightTime = 0;
		public Color color = Color.red;
	}
	public LightProperty lightProperty;
	// Use this for initialization

	void Init () {
		if(gameObject.GetComponent<Light>())
			gameObject.GetComponent<Light>().enabled = true;
		else 		
			this.gameObject.AddComponent<Light>();

		Invoke ("ShowLight", lightProperty.LightDelay);
	}

	void OnEnable(){
		Init ();
	}
	void ShowLight(){
		this.gameObject.GetComponent<LensFlare> ().enabled = true;
		GetComponent<Light>().intensity = lightProperty.Intensity;
		GetComponent<Light>().range = lightProperty.Range;
		GetComponent<Light>().color = lightProperty.color;
		Invoke ("CloseLight", lightProperty.LightTime);

	}
	void CloseLight(){
		gameObject.GetComponent<Light> ().enabled = false;
		this.gameObject.GetComponent<LensFlare> ().enabled = false;

	}
}
