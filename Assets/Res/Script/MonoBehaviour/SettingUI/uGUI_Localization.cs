using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class TextHelper
{
	public static int NationLevel (int Exp)
	{
		int Temp;
		return NationLevel (Exp, out Temp);
	}

	public static int NationLevel (int Exp, out int RequireExp)
	{
		int Level = 1;
		RequireExp = 1200;
		if (Exp >= 1200) {
			Level++;//2
			RequireExp = 3500;
		}
		if (Exp >= 3500) {
			Level++;//3
			RequireExp = 7200;
		}
		if (Exp >= 7200) {
			Level++;//4
			RequireExp = 15000;
		}
		if (Exp >= 15000) {
			RequireExp = 35000;
			Level++;//5
		}
		if (Exp >= 35000) {
			RequireExp = 85000;
			Level++;//6
		}
		if (Exp >= 85000) {
			RequireExp = 125000;
			Level++;//7
		}
		if (Exp >= 125000) {
			RequireExp = 245000;
			Level++;//8
		}
		if (Exp >= 245000) {
			RequireExp = 285000;
			Level++;//9
		}
		if (Exp >= 285000) {
			RequireExp = 305000;
			Level++;//10
		}
		if (Exp >= 305000) {
			RequireExp = 500000;
			Level++;//11
		}
		if (Exp >= 500000) {
			RequireExp = 600000;
			Level++;//12
		}
		if (Exp >= 600000) {
			RequireExp = 0;
			Level++;//13
		}
		return Level;
	}


	public static Dictionary<string,string> TextAssetToDictionary (TextAsset Text)
	{
		Dictionary<string,string> temp = new Dictionary<string, string> ();
		string[] Lines = Text.text.Split ('\n');
		for (int j = 0; j < Lines.Length; j++) {
			if (Lines [j].Split ('=').Length != 1) {
				string[] TextContent = Lines [j].Split ('=') [0].Split (' ');
				string TextContentResult = null;

				for (int i = 0; i < TextContent.Length; i++) {
					TextContentResult += TextContent [i];
				}

				if (!temp.ContainsKey (TextContentResult))
					temp.Add (TextContentResult, Lines [j].Split ('=') [1]);
			}
		}
		return temp;
	}
}

public class uGUI_Localization : MonoBehaviour
{
	public TextAsset[] Languages;

	public static string CurretLanguage = "EN";

	public static Dictionary<string,string> LanguageKey = new Dictionary<string, string> ();

	public static bool Loaded = false;

	public  System.Action onLangLoaded;

	public static uGUI_Localization Instance;
	public void onLanaugeChanged(int ID){
		if (ID == -1)
			return;
		
		switch (ID) {
		case 0:
			PlayerPrefs.SetString("Language", "CN");
			break;
		case 1:
			PlayerPrefs.SetString("Language", "EN");
			break;
		case 2:
			PlayerPrefs.SetString("Language", "RU");
			break;
		case 3:
			PlayerPrefs.SetString("Language", "UA");
			break;
		} 

		Start ();
        MaterialUI.DialogManager.ShowAlert("You are going to change the game language,it requires restart the game.",() => {
            Application.Quit();
        },"OK","Change Language",null);

	}

	void Awake(){
		Instance = this;
        DontDestroyOnLoad(this.gameObject);
	}
	public void Start ()
	{
		LanguageKey = new Dictionary<string, string> ();

		if (PlayerPrefs.HasKey ("Language"))
			CurretLanguage = PlayerPrefs.GetString ("Language");
		else {
			switch (Application.systemLanguage) {
			case SystemLanguage.English:
				CurretLanguage = "EN";
				break;
			case SystemLanguage.Chinese:
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
				CurretLanguage = "CN";
				break;
			case SystemLanguage.Russian:
				CurretLanguage = "RU";
				break;
			case SystemLanguage.Ukrainian:
				CurretLanguage = "UA";
					break;
			default:
				CurretLanguage = "EN";
				break;
			}
		}

		for (int i = 0; i < Languages.Length; i++) {

			if (Languages [i].name == string.Format("Lang_{0}",CurretLanguage)) {
				LanguageKey = TextHelper.TextAssetToDictionary (Languages [i]);

				Loaded = true;

				if (onLangLoaded != null) {
					onLangLoaded ();
				}
			}
		}

        //UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (scene, toScene) => {
        //    uGUI_Localsize[] localsize = FindObjectsOfType<uGUI_Localsize>();
        //    for (int i = 0; i < localsize.Length;i++){
        //        localsize[i].Init();
        //    }
        //    Canvas.ForceUpdateCanvases();
        //};
	}

}
