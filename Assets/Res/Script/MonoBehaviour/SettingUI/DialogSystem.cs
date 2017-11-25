using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;

public class DialogSystem : MonoBehaviour {
	public static DialogSystem Instance;
	public Text OutPut;
	public GameObject DialogObject;
	public int DialogInQueue=0, DialogFinsih = 0;

	// Use this for initialization
	void Awake () {
		if (Instance != null) {
			Destroy (Instance.DialogObject);
			Destroy (Instance);
		}
		
		Instance = this;
		DontDestroyOnLoad (this.gameObject);
		DontDestroyOnLoad (DialogObject);
	}
	void Update(){
		if (DialogFinsih >= DialogInQueue) {
			DialogObject.SetActive (false);
		} else {
			DialogObject.SetActive (true);
		}

	}

	public  GameObject G_EventManager;
	public  void ReDirectInputModule(){
		if (G_EventManager == null) {
			if (EventSystem.current != null) {
				Instance.G_EventManager = EventSystem.current.gameObject;
			}
		}

		if (G_EventManager == null)
			return;
		
		if (DialogFinsih >= DialogInQueue) {
			Instance.G_EventManager.SetActive (true);
		} else {
			Instance.G_EventManager.SetActive (false);
		}
	}
	public static void UpdateLabel(string Text,System.Action<bool> MyAction = null,float WaitSecond =0){
		Instance.StartCoroutine (Instance.ShowLabel (Text,MyAction,WaitSecond));
	}
	public static void DialogFinish(){
		Instance.DialogFinsih = Instance.DialogInQueue;
		Instance.ReDirectInputModule ();
	}
	public IEnumerator ShowLabel(string Text,System.Action<bool> MyAction = null,float WaitSecond =0){
		DialogObject.SetActive (true);
		DialogInQueue += 1;
		OutPut.text = Text;
		Instance.ReDirectInputModule ();
		if (MyAction != null) {
			yield return new WaitForSeconds (WaitSecond);
			MyAction (true);
		} 

		if (WaitSecond != 0) {
			yield return new WaitForSeconds (WaitSecond);
			DialogFinish ();
		}

		if (EventSystem.current!=null) {
			EventSystem.current.enabled = true;
		}	

		yield break;
	}
}
