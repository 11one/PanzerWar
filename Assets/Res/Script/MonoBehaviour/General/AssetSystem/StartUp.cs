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

		#if UNITY_IPHONE
		if (check_jailbroken ()) {
			DialogSystem.UpdateLabel ("JailBroken Application Rejected!", onFinish => {
				Application.Quit();
			}, 5);
		}
		#endif
	}
	#if UNITY_IPHONE

	bool check_jailbroken ()
	{
		string[] paths = new string[10] {
			"/Applications/Cydia.app",
			"/private/var/lib/cydia",
			"/private/var/tmp/cydia.log",
			"/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
			"/usr/libexec/sftp-server",
			"/usr/bin/sshd",
			"/usr/sbin/sshd",
			"/Applications/FakeCarrier.app",
			"/Applications/SBSettings.app",
			"/Applications/WinterBoard.app"
		};
		int i = 0;
		bool jailbroken = false;

		for (i = 0; i < paths.Length; i++) {
			if (System.IO.File.Exists (paths [i])) {
				jailbroken = true;
			}            
		}
		return jailbroken;
	}
	#endif
	public void StartUpAction ()
	{
		DontDestroyOnLoad (gameObject);
		StartCoroutine (AssetBundleManager.RequestScene (true, false, "ClientOffline", null, onLoaded => {
			Destroy (gameObject);	
		}));
	}
}
