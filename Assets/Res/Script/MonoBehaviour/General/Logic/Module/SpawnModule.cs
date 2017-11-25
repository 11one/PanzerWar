using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnModule : MonoBehaviour
{
	private RespawnPointModule RespanPointModule;
	private const string VehicleAssetName = "PhotonInstantiate/TankInitSystem";

	//public void Init (RespawnPointModule _respanPointModule)
	//{
	//	RespanPointModule = _respanPointModule;
	//}

	//public TankInitSystem SpawnOnlinePlayer (string Vehicle, PhotonPlayer Player,out bool Spawned)
	//{
	//	if (HasPlayerController(Player.UserId)) {
	//		Spawned = false;
	//		return null;
	//	}

 //       bool AirPlane = false;

	//	object[] data;
	//	data = new object[] {
	//		"GameNetwork", //0
	//		Vehicle,
	//		Player.ID, 
	//		Player.UserId, 
	//		Player.NickName, 
	//		Player.GetTeam (), 
	//		1,
	//		0,
	//		0
	//	};

	//	GameObject[] TeamAStartPoints = GameObject.FindGameObjectsWithTag ("TeamAStartPoint");
	//	GameObject[] TeamBStartPoints = GameObject.FindGameObjectsWithTag ("TeamBStartPoint");
	//	Transform StartPoint = null;
	//	//player.name = "SuperTester:" + player.ID;
	//	Vector3 HeightOffset = Vector3.zero;



	//	if (Player.GetTeam () == PunTeams.Team.red) {
	//		StartPoint = RespanPointModule.RandomStartPoint (TeamAStartPoints, AirPlane, out HeightOffset);
	//	} else if (Player.GetTeam () == PunTeams.Team.blue) {
	//		StartPoint = RespanPointModule.RandomStartPoint (TeamBStartPoints, AirPlane, out HeightOffset);
	//	} else {
	//		Debug.LogError ("No Team!");
	//		Spawned = false;
	//		return null;
	//	}

	//	PhotonView playerPhotonView = PhotonNetwork.Instantiate (VehicleAssetName, StartPoint.transform.position + HeightOffset, StartPoint.transform.rotation, 0, data).GetComponent<PhotonView> ();
	//	//playerPhotonView.TransferOwnership (player.ID);

	//	playerPhotonView.GetComponent<TankInitSystem> ().PlayerID = Player.ID;

	//	Spawned = true;

	//	return playerPhotonView.GetComponent<TankInitSystem> ();
	//}

	////仅用于AB压力测试
	//public void CreateABTestBot(int ID){

	//	object[] data;
	//	data = new object[] {
	//		"GameNetworkBot",
	//		"T-26",
	//		PunTeams.Team.red,
	//		10000 + ID
	//	};
	//	Vector3 HeightOffset = Vector3.zero;

	//	GameObject[] TeamAStartPoints = GameObject.FindGameObjectsWithTag ("TeamAStartPoint");
	//	Transform StartPoint = RespanPointModule.RandomStartPoint (TeamAStartPoints, false, out HeightOffset);

	//	PhotonView playerPhotonView = PhotonNetwork.Instantiate (VehicleAssetName, StartPoint.transform.position + HeightOffset, StartPoint.transform.rotation, 0, data).GetComponent<PhotonView> ();
	//}







	//public void CleanLeftOnlinePlayerAction(PhotonPlayer Player){
	//	foreach (TankInitSystem tankInitSystem in GameObject.FindObjectsOfType<TankInitSystem>()) {
	//		if (tankInitSystem.PlayerID == Player.ID) {
	//			//tankInitSystem.GetComponentInChildren<SyncGroundVehicle> ().Stop ();
	//		}
	//	}
	//}

	//public void RemoveLeftOnlinePlayer(PhotonPlayer Player){
	//	foreach (TankInitSystem tankInitSystem in GameObject.FindObjectsOfType<TankInitSystem>()) {
	//		if (tankInitSystem.PlayerID == Player.ID) {
	//			PhotonNetwork.Destroy (tankInitSystem.gameObject);
	//		}
	//	}
	//}

	//public  bool HasPlayerController (string UserID)
	//{
	//	foreach (BaseInitSystem InitSystem in GameObject.FindObjectsOfType<BaseInitSystem>()) {
	//		if (InitSystem.UserID == UserID) {
	//			return true;
	//		}
	//	}
	//	return false;
	//}

}
