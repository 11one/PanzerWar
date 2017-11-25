using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIModule : MonoBehaviour {
	
	public InputField AccountInput,PasswordInput; 

	public Button LoginButton;

	public static LoginUIModule Instance;

	public System.Action<string,string> onUserLogin;

	void Awake(){
		Instance = this;
	}

	void Start(){
		AccountInput.text = PlayerPrefs.GetString ("UserAccount");
		PasswordInput.text = PlayerPrefs.GetString ("UserPassword");

		LoginButton.onClick.AddListener (onLoginClicked);

		AccountInput.onValueChanged.AddListener (SaveInputValue);
		PasswordInput.onValueChanged.AddListener (SaveInputValue);
	}

	void onLoginClicked(){
		onUserLogin (AccountInput.text, PasswordInput.text);
	}

	void SaveInputValue(string Value){
		PlayerPrefs.SetString ("UserAccount", AccountInput.text);
		PlayerPrefs.SetString ("UserPassword", PasswordInput.text);
	}

}
