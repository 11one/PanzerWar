using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uGUI_CInput : MonoBehaviour {
	public enum Style{
		Null,
		Main,
		Label,
	}
	public Style style;
	public Transform Content;
	[HideInInspector]
	public string Action,PrimaryKey,SecondKey;
	public static uGUI_CInput ThisScript;
	void Start(){
		Switch ();
	}
	void Switch(){
		switch (style) {
			case Style.Main:
				MainHandle();
				ThisScript = this;
			break;
			case Style.Label:
				Label();
				break;
		}
	}
	public static bool NeedDelayTime =false;

	public  void Refresh(){
		if (cInput.scanning) {
			if(!IsInvoking("Refresh"))
				InvokeRepeating ("Refresh", 0, 0.1f);
			return;
		} 
		else {
			uGUI_CInput[] DestroyOJ = ThisScript.Content.GetComponentsInChildren<uGUI_CInput> ();
			for (int i =0; i<DestroyOJ.Length; i++) {
				Destroy (DestroyOJ [i].gameObject);
			}
			ThisScript.MainHandle ();
		}
	}

	void MainHandle(){
		Debug.Log(cInput.length);

		for (int n = 0; n < cInput.length; n++) { 
			GameObject KeySettingTemplet = (GameObject)Instantiate (Resources.Load ("UI/KeySettingTemplet"));
			KeySettingTemplet.GetComponent<uGUI_CInput>().style = Style.Label;
			KeySettingTemplet.transform.SetParent (Content);
			KeySettingTemplet.transform.localScale = new Vector3 (1, 1, 1);
			KeySettingTemplet.GetComponent<uGUI_CInput>().Action =  cInput.GetText(n);
			KeySettingTemplet.GetComponent<uGUI_CInput>().PrimaryKey = cInput.GetText(n,1);
			KeySettingTemplet.GetComponent<uGUI_CInput>().SecondKey =  cInput.GetText(n, 2);

		}

	}
	void Label(){
		transform.Find ("Action").GetComponent<Text> ().text = uGUI_Localsize.GetContent(Action);
		transform.Find ("PrimaryKey").GetComponent<Text> ().text = PrimaryKey;
		transform.Find ("SecondKey").GetComponent<Text> ().text = SecondKey;
	}
	public void OnClickKey(Button Click){
		if (Click.gameObject.name == "PrimaryKey") {
			cInput.ChangeKey (Action, 1);
			Refresh ();
			transform.Find ("PrimaryKey").GetComponent<Text> ().text = uGUI_Localsize.GetContent("KeySettingInput");
		} else if (Click.gameObject.name == "SecondKey") {
			cInput.ChangeKey (Action, 2);
			Refresh ();
			transform.Find ("SecondKey").GetComponent<Text> ().text = uGUI_Localsize.GetContent("KeySettingInput");

		}
	}
	public void SetDefault(){
		cInput.ResetInputs ();
		Refresh ();
	}
}
