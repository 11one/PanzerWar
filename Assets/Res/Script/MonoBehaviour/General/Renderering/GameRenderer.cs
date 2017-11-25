using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.UI;

public class GameRenderer : MonoBehaviour {
	public static bool Instanced = false;
	public static GameObject Instance;
	// Use this for initialization
	void Start () {
		if (!Instanced) {
			Instanced = true;
		} else {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		Instance = gameObject;
	}

	public static void RegistCamera(Camera camera){
		RenderTexture gameRenderTexture =new RenderTexture( PlayerPrefs.GetInt ("Width"), PlayerPrefs.GetInt ("Height"), 24 );
		camera.targetTexture = gameRenderTexture;
		Instance.GetComponentInChildren<RawImage>().texture = gameRenderTexture;
	}
}
