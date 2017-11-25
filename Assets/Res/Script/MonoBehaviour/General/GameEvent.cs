using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VehicleType{
	LT,
	MT,
	HT,
	TD,
	SPG,
	Fighter,
	TestVehicle
}

public class GameEvent : MonoBehaviour {
	public static Identity Player;
	public static PlayerState InstantPlayerState;

	public static Camera PlayerCamera;
	//public static string PlayerVehicleName;
	//public static string LastInstatnceVehicleName;

	//public static Canvas RegistedPlayerUI;



	public static bool InEditor = false;
	//public static GameObject Target;
	public static  Vector3 PlayerAmmoFinalPostion = Vector3.zero;
	public static bool HasFinalPostion = false;
	public static bool PlayerInCrossFire = false;

	//public static int LocalCauseHit, LocalBeHit;
	//public  delegate void HandlePlayerEnter(); 
	//public static event HandlePlayerEnter onPlayerEnter;

	//public delegate void HandlePlayerModifyControlPostion ();
	//public static event HandlePlayerModifyControlPostion onPlayerModifyControlPostion;





	public static void InitPlayer(Identity player){
		Player = player;
		PlayerCamera = Player.GetComponentInChildren<Camera> ();
		InstantPlayerState = player.GetComponentInChildren<PlayerState> ();
	}

	//public static void RegistPlayerHit(string User,string Vehicle,PhotonPlayer NetPlayer){
	//	if (PlayerHitList.ContainsKey (User)) {
	//		#if !UNITY_EDITOR
	//		//PhotonNetwork.CloseConnection (PlayerHitList [User].NetPlayer);
	//		#endif
	//		PlayerHitList.Remove (User);
	//	}
	//	Hit UserHit = new Hit (User,Vehicle,NetPlayer);
	//	PlayerHitList.Add (User,UserHit);
	//}
//	public static void ReleasePlayerHit(PhotonPlayer User){
//		if (User != null) {
//			if (User.customProperties.ContainsKey ("Account")) {
//				string UserAccount = User.customProperties ["Account"].ToString ();
//				if (PlayerHitList.ContainsKey (UserAccount)) {
////					BattleClient.Instance.StartCoroutine (BattleClient.Upload (PlayerHitList [UserAccount].Vehicle, PlayerHitList [UserAccount].DamageList, PlayerHitList [UserAccount].DestroyVehicleList, User));
	//			}
	//			PlayerHitList.Remove (UserAccount);
	//		}
	//	}
	//}
	//public static bool HasPlayerHit(string User){
	//	if (PlayerHitList.ContainsKey (User)) {
	//		return true;
	//	} else {
	//		return false;
	//	}
	//}
	//public static void Init(){
	// 	PlayerHitList = new Dictionary<string,Hit>();
	//	Coin = new Dictionary<string, int> ();
	//	Exp = new Dictionary<string, int> ();
	//	Player =null;
	//	PlayerCamera =null;
	//	RegistedPlayerUI =null;
	//	LocalCauseHit = 0;
	//	LocalBeHit = 0;
	//}
	//public class Hit{
	//	string User = "";
	//	public string Vehicle = "";
	//	public PhotonPlayer NetPlayer;

	//	public List<string> DestroyVehicleList = new List<string>();
	//	public Dictionary<string,int> DamageList = new Dictionary<string, int>();

	//	public string GetUser(){
	//		return User;
	//	}
	//	public Hit(string user,string vehicle,PhotonPlayer netPlayer){
	//		User = user;
	//		Vehicle = vehicle;
	//		NetPlayer = netPlayer;
	//	}
	//	public void AddDamage(string Vehicle,int Damage){
	//		if (DamageList.ContainsKey (Vehicle)) {
	//			DamageList [Vehicle] += Damage;
	//		} else {
	//			DamageList.Add (Vehicle, Damage);
	//		}
	//	}
	//	public void AddDestroyVehicle(string Vehicle){
	//		DestroyVehicleList.Add (Vehicle);
	//	}
	//}
}
