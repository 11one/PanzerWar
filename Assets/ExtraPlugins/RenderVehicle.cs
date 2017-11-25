using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using System.IO;

public class RenderVehicle : MonoBehaviour {
    public GameObject[] AssetsToRender;

	void Start () {
		StartCoroutine(ScreenShot());
	}


	IEnumerator ScreenShot(){
        foreach (GameObject vehicle in AssetsToRender) {
            GameObject  tempVehicle = Instantiate(vehicle);
            Rect lRect = new Rect(0f, 0f, Screen.width, Screen.height);
            yield return new WaitForEndOfFrame();
            Texture2D capturedImage = zzTransparencyCapture.capture(lRect);
            byte[] byt = capturedImage.EncodeToPNG();
            File.WriteAllBytes("Others/Renderering/Common/" + vehicle.name + ".png", byt);
            Destroy(tempVehicle);
		}
	}

}
#endif