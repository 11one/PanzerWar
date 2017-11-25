using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using System.IO;

public class RenderVehicle : MonoBehaviour {
	public bool isPortrait,isBattleField;
	public Transform t;
	public Text VehicleName;
	List<GameObject> AssetsToRender;
	GameObject CurrentAsset;
	List<string> VehicleList = new List<string>();

	void Start () {
//		AccountDataManager.InstanceAllHastables ();
		PlayerPrefs.SetString("Language","Chinese");

		uGUI_QualitySetting.Init ();
		//foreach(string vehicle in AccountDataManager.ExcelVehicleData.Keys){
		//	if (AccountDataManager.GetVehicleType (vehicle) != VehicleType.Fighter) {
		//		VehicleList.Add (vehicle);
		//	}
		//}


		StartCoroutine(ScreenShot());
	}

	IEnumerator LoadVehicleResources(string Vehicle){
		ResourceRequest RR = Resources.LoadAsync ("ExtraPackage/TankModel/" + Vehicle + "/" + Vehicle + "_Pre");
		yield return RR;
		GameObject VehicleObj = (GameObject)Instantiate((GameObject)RR.asset ,t.transform.position,t.rotation);
		VehicleObj.GetComponent<TankInitSystem> ()._InstanceNetType = InstanceNetType.GarageTank;
		VehicleObj.GetComponent<TankInitSystem> ().ShowHitBoxInspecter =false;
		VehicleObj.GetComponent<TankInitSystem> ().InitTankInitSystem ();
		CurrentAsset = VehicleObj;
	}

	IEnumerator ScreenShot(){
		foreach (string vehicle in VehicleList) {
			if (CurrentAsset != null)
				Destroy (CurrentAsset);
			
			StartCoroutine(LoadVehicleResources (vehicle));

			yield return new WaitForSeconds (3f);
			if (isBattleField) {
				VehicleName.text = uGUI_Localsize.GetContent(vehicle);
				Texture2D screenShot = new Texture2D(Screen.width,Screen.height, TextureFormat.RGB24,false);  
				yield return new WaitForEndOfFrame();
				screenShot.ReadPixels(new Rect(0f,0f,Screen.width,Screen.height), 0, 0);  
				yield return new WaitForEndOfFrame();
				screenShot.Apply();  
				byte[] byt = screenShot.EncodeToPNG();  
				File.WriteAllBytes("Others/Renderering/BattleField/"+ vehicle+".png",byt);
				yield return new WaitForEndOfFrame();
				System.GC.Collect ();
			} else {
				Rect lRect = new Rect(0f,0f,Screen.width,Screen.height);
				yield return new WaitForEndOfFrame();
				Texture2D capturedImage = zzTransparencyCapture.capture(lRect);
				byte[] byt = capturedImage.EncodeToPNG();

				if (isPortrait) {
					File.WriteAllBytes("Others/Renderering/Portrait/"+ vehicle+".png",byt);
				} else {
					File.WriteAllBytes("Others/Renderering/Common/"+ vehicle+".png",byt);
				}
			}

			List<string> ReleaseAssets = new List<string>();
			foreach (string Key in AssetBundleManager.LoadedAssets.Keys) {
				ReleaseAssets.Add (Key);
			}
			foreach (string key in ReleaseAssets) {
				if (AssetBundleManager.LoadedAssets [key] != null) {
					AssetBundleManager.LoadedAssets [key].Unload (true);
				}
				AssetBundleManager.LoadedAssets.Remove (key);
			}

		}
	}

}
#endif