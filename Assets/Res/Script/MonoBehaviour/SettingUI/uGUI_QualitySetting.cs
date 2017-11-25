using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ScrollbarLevel{
	Lv1,
	Lv2,
	Lv3,
	Lv4
}

public class uGUI_QualitySetting : MonoBehaviour {
	public static GameObject Panel;

	public Scrollbar TextureQuality,ShadowDistance,ShadowQuality;
	ScrollbarLevel TextureQualityLv,ShadowDistanceLv,ShadowQualityLv;
	public Dropdown ScreenSolution;

	public GameObject Settings_Panel;

	public static bool AdvanceTrackSystem = false;
	public static bool B_inBattleMessage = false;
	public static Rect PhysicsScreen = new Rect();
	public static uGUI_QualitySetting _Instance;

	public static void Init (){
		Open();
		_Instance = Panel.GetComponentInChildren<uGUI_QualitySetting>();
		_Instance.ReadSetting ();
		Panel.transform.GetChild(0).GetChild(0).gameObject.SetActive (false);
	}

	public static void Open(){
		if (Panel != null) {
			Panel.transform.GetChild(0).GetChild(0).gameObject.SetActive (true);
		} else {
			Panel = Instantiate(Resources.Load ("UI/Settings")) as GameObject;
			DontDestroyOnLoad (Panel);
			Open ();
		}
	}
	void Start(){
		System.Collections.Generic.List<Dropdown.OptionData> ScreenSolutionDropDownOptions = new System.Collections.Generic.List<Dropdown.OptionData>();
		ScreenSolutionDropDownOptions.Add (new Dropdown.OptionData ("100%", null));
		ScreenSolutionDropDownOptions.Add (new Dropdown.OptionData ("75%", null));
		ScreenSolutionDropDownOptions.Add (new Dropdown.OptionData ("60%", null));
		ScreenSolution.AddOptions (ScreenSolutionDropDownOptions);
		uGUI_QualitySetting._Instance.ScreenSolution.value = PlayerPrefs.GetInt ("CustomScreenSolution",2);
		ScreenSolution.RefreshShownValue ();
		ScreenSolution.onValueChanged.AddListener (OnScreenSolutionValueChanged);
		ReadSetting ();
	}

	public static int CurrentSolutionValue = -1;

	void OnScreenSolutionValueChanged(int value){
		if (ScreenSolution.value != CurrentSolutionValue) {
			ApplySolution (ScreenSolution.value);
			CurrentSolutionValue = ScreenSolution.value;
		}
	}
	public static void ApplySolution(int _ScreenSolutionLv){
		switch (_ScreenSolutionLv) {
			case 0:
				SetScreenSolution (PhysicsScreen.width, PhysicsScreen.height);
				break;
			case 1:
				SetScreenSolution (PhysicsScreen.width*0.85f, PhysicsScreen.height*0.85f);
				break;
			case 2:
				SetScreenSolution (PhysicsScreen.width*0.75f, PhysicsScreen.height*0.75f);
				break;
		}
		PlayerPrefs.SetInt ("CustomScreenSolution", _ScreenSolutionLv);
	}

	public static void SetScreenSolution(float Width,float Height){
		Screen.SetResolution (Mathf.RoundToInt(Width), Mathf.RoundToInt(Height), false);
	}

	public void SaveQuality(){


	}
	public void ReadSetting(){

	}

}
