using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Identity : MonoBehaviour
{
	public PunTeams.Team identity = PunTeams.Team.none;
	public static List<Identity> GlobalTeamList = new List<Identity> ();
	public static List<Identity> GlobalTeamAList = new List<Identity> ();
	public static List<Identity> GlobalTeamBList = new List<Identity> ();

	public void Init (PunTeams.Team NewIdentity, InstanceNetType netType)
	{
		identity = NewIdentity;
		GlobalTeamList.Add (this);
		switch (identity) {
		case PunTeams.Team.red:
			GlobalTeamAList.Add (this);
			break;
		case PunTeams.Team.blue:
			GlobalTeamBList.Add (this);
			break;
		}

		if (netType != InstanceNetType.GameNetworkMaster && netType != InstanceNetType.GameNetWorkBotMaster)
			StartCoroutine (InitTeamUI ());
	}

	void OnDestroy ()
	{
		Debug.Log ("OnDestroy");

		GlobalTeamList.Remove (this);
		switch (identity) {
		case PunTeams.Team.red:
			GlobalTeamAList.Remove (this);
			Debug.Log ("GlobalTeamAList");
			break;
		case PunTeams.Team.blue:
			GlobalTeamBList.Remove (this);
			Debug.Log ("GlobalTeamBList");

			break;
		}
		Destroy (TankInfoUI);
	}

	void OnGUI ()
	{
		if (StartUpdateUI && !InHide)
			UpdateTankInfoUI ();
	}

	#region 敌我显示

	public GameObject TankInfoUI;
	public Camera MainCamera;
	public bool StartUpdateUI = false;
	public bool InReDirectCamera = false;
	public bool Enemy = false;
	public bool InHide = false;
	public string OwnerName, OwnerVehicle;

	void Awake ()
	{
		TankInfoUI = (GameObject)Instantiate ((GameObject)Resources.Load ("TankInfo"), Vector3.zero, Quaternion.identity);
	}

	IEnumerator InitTeamUI ()
	{
        if (GameDataManager.OfflineMode) {
            if (identity == GameDataManager.OfflinePlayerTeam) {
				Enemy = false;
			} else {
				Enemy = true;
			}
		}

		MainCamera = Camera.main;


		#region 防止空对象
		while (GameObject.FindGameObjectWithTag ("GameStartGUI") == null) {
			yield return new WaitForSeconds (1);
			Debug.LogError ("Waiting");

		}
		#endregion
		Canvas canvas = GameObject.FindGameObjectWithTag ("GameStartGUI").GetComponent<Canvas> ();
		TankInfoUI.transform.SetParent (canvas.transform);

		TankInfoUI.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		TankInfoUI.GetComponentInChildren<Text> ().color = Color.green;
		TankInfoUI.transform.SetAsFirstSibling ();
		StartUpdateUI = true;
	}

	IEnumerator ReDirectCamera ()
	{
		InReDirectCamera = true;
		while (GameEvent.Player == null)
			yield return new WaitForSeconds (1);
		MainCamera = Camera.main;
		InReDirectCamera = false;
	}

	public void UpdateTankInfoUI ()
	{
		if (MainCamera != null && TankInfoUI != null) {
			TankInfoUI.transform.position = MainCamera.WorldToScreenPoint (transform.position + new Vector3 (0, 5, 0));
			if (TankInfoUI.transform.position.z > 0) {
				TankInfoUI.SetActive (true);
			} else {
				TankInfoUI.SetActive (false);
			}
		}
		if (MainCamera == null) {
			if (!InReDirectCamera) {
				StartCoroutine (ReDirectCamera ());
			}
		}


	}

	public float InitHealth, CurrentHealth;

	public void UpdateHealth (float currentHealth)
	{
		CurrentHealth = currentHealth;
	}

	public void SetOwnerInfo (string ownerName, string vehicleName, float initHealth)
	{
		#if ClientCode
		OwnerName = ownerName;
		OwnerVehicle = vehicleName;
		InitHealth = initHealth;
		CurrentHealth = initHealth;
		if (Enemy) {
			TankInfoUI.GetComponentInChildren<Text> ().text = ownerName.ToString () + uGUI_Localsize.GetContent (vehicleName).ToString ();
			TankInfoUI.GetComponentInChildren<Text> ().color = Color.red;
			Hide ();
		} else {
			TankInfoUI.GetComponentInChildren<Text> ().text = ownerName.ToString () + uGUI_Localsize.GetContent (vehicleName).ToString ();
		}
//		GameEvent.TriggerPlayerEnterHandler ();
		#endif
	}

	public void Show ()
	{
		TankInfoUI.SetActive (true);
		InHide = false;
	}

	public void Hide ()
	{
		TankInfoUI.SetActive (false);
		InHide = true;

	}

	#endregion

}
