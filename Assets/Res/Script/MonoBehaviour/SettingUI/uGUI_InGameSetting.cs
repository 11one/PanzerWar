using UnityEngine;
using System.Collections;

public class uGUI_InGameSetting : MonoBehaviour {
	public static GameObject UIPanel;
	public static void OpenSetting(){
		if (UIPanel == null) {
			UIPanel = (GameObject)GameObject.Instantiate (Resources.Load ("InGameSetting"));
		}
		UIPanel.SetActive (true);
		GameEvent.InEditor = true;
		UnityEngine.Cursor.visible = true;
		UnityEngine.Cursor.lockState = CursorLockMode.None;
		Time.timeScale = 0.0001f;

	}
	public  void CloseSetting(){
		UIPanel.SetActive (false);
		GameEvent.InEditor = false;
		Time.timeScale = 1;
	}
		
}
