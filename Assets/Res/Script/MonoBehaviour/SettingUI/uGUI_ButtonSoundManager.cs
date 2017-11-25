using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class uGUI_ButtonSoundManager : MonoBehaviour {
	public static bool HasInstance = false;
	AudioSource audioSource;
	public AudioClip Tap,Swipe,Rumble,Crumple;

	void Start(){
		if (HasInstance)
			return;
		HasInstance = true;
		audioSource = GetComponent<AudioSource> ();
		//UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		DontDestroyOnLoad (this.gameObject);
	}
	Button[] MyButtons,ChildButtons;
	void OnLevelWasLoaded() {
		MyButtons = GameObject.FindObjectsOfType<Button> ();
		for(int i =0;i<MyButtons.Length;i++){
			for (int j = 0; j < MyButtons [i].onClick.GetPersistentEventCount(); j++) {
			 Debug.Log (MyButtons [i].onClick.GetPersistentMethodName (j));
				if (MyButtons [i].onClick.GetPersistentMethodName (j) == "SetActive") {
					GameObject Target = (GameObject)MyButtons [i].onClick.GetPersistentTarget (j);
					ChildButtons = Target.GetComponentsInChildren<Button> ();
					for (i = 0; i < MyButtons.Length; i++) {
						ChildButtons [i].onClick.AddListener (PlayTap);
					}
				}
			}
			MyButtons[i].onClick.AddListener (PlayTap);
		}
	}

	void PlayTap(){
		audioSource.clip = Tap;
		audioSource.Play ();
	}
}
