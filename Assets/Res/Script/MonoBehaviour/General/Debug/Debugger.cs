using UnityEngine;

public class Debugger : MonoBehaviour {
	public static  GameObject DebugerGUI;
	public static void Init(){
		if (DebugerGUI != null)
			return;
		
		DebugerGUI = Instantiate(Resources.Load ("UI/DebugerGUi")) as GameObject;
		//KGFDebugGUI.GetInstance ().itsOpen = true;
		DontDestroyOnLoad (DebugerGUI);
	}
}


