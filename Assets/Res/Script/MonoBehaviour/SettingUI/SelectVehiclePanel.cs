using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectVehiclePanel : MonoBehaviour {
	public Button joinGame;

	public  string CurrentVehicle;

	private GameObject UnActiveLayer;

	public void Init(string _vehicle){
		CurrentVehicle = _vehicle;
		transform.Find ("TankName").GetComponent<Text> ().text =uGUI_Localsize.GetContent(_vehicle);
		//transform.Find ("PR").GetComponent<Text> ().text =AccountDataManager.GetVehiclePR(_vehicle).ToString();

		GetComponent<Button> ().onClick.AddListener (OnClickSelect);
		joinGame.onClick.AddListener (onClickJoin);

		UnActiveLayer = transform.Find ("UnActive").gameObject;
	
		StartCoroutine (LoadImage (_vehicle));

		DeHighlight ();
	}
	public void Highlight(){
		UnActiveLayer.SetActive (false);
		joinGame.gameObject.SetActive (true);
	}
	public void DeHighlight(){
		UnActiveLayer.SetActive (true);
		joinGame.gameObject.SetActive (false);
	}
	//异步加载载具描述图片
	private IEnumerator LoadImage(string _vehicle){
		ResourceRequest resourceRequest = Resources.LoadAsync<Sprite>("UI/VehicleImage/" + _vehicle);
		yield return resourceRequest;
		transform.Find("Thumbnail").GetComponent<Image>().sprite = (Sprite)resourceRequest.asset;
	}

	void onClickJoin(){
		//BattleUIModule.Instance.onVehicleJoinClicked (CurrentVehicle);
	}

	void OnClickSelect(){
		//BattleUIModule.Instance.onVehicleSelectChanged (CurrentVehicle);
	}
}
