using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegiserUIModule : MonoBehaviour {
	public InputField AccountInput,PasswordInput,NickNameInput; 
	public Button RegisterButton;

	public System.Action<string,string,string> onUserRegister;

	public static RegiserUIModule Instance;

	void Awake(){
		Instance = this;
	}
	void Start(){
		RegisterButton.onClick.AddListener (onRegisterClicked);
	}
	void onRegisterClicked(){
		onUserRegister (AccountInput.text, PasswordInput.text,NickNameInput.text);
	}

}
