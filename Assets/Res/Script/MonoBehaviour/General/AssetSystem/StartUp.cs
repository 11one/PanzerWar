using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class StartUp : MonoBehaviour
{


	void Start ()
	{
		if (PlayerPrefs.HasKey ("CustomScreenSolution")) {
			uGUI_QualitySetting.PhysicsScreen.width = PlayerPrefs.GetInt ("OriginalWidth");
			uGUI_QualitySetting.PhysicsScreen.height = PlayerPrefs.GetInt ("OriginalHeight");
			uGUI_QualitySetting.CurrentSolutionValue = PlayerPrefs.GetInt ("CustomScreenSolution");
			uGUI_QualitySetting.ApplySolution (PlayerPrefs.GetInt("CustomScreenSolution"));
			//Screen.SetResolution (Width, Height, false);

		} else {
			uGUI_QualitySetting.PhysicsScreen.width = Screen.width;
			uGUI_QualitySetting.PhysicsScreen.height = Screen.height;

			PlayerPrefs.SetInt ("OriginalWidth", Screen.width);
			PlayerPrefs.SetInt ("OriginalHeight", Screen.height);

			uGUI_QualitySetting.ApplySolution (2);
		}

	}

	public void StartUpAction ()
	{
		DontDestroyOnLoad (gameObject);
		StartCoroutine (AssetBundleManager.RequestScene (true, false, "ClientOffline", null, onLoaded => {
			Destroy (gameObject);	
		}));
	}
}
