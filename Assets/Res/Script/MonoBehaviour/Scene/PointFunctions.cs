using UnityEngine;
using System.Collections;

public class PointFunctions : MonoBehaviour {

	public enum Function{
		SupplyAmmo,
		RepairTank,
		CaptureArea,
		EventCall
	}
	public Function PointFunction;
	public string EventCallName = "New Event";

	void OnTriggerEnter(Collider cl){
		#if ClientCode
		//if(GameNetwork.OfflineMode){
		//	TankInitSystem tankInitSystem = cl.GetComponentInParent<TankInitSystem> ();
		//	if (tankInitSystem) {
		//		switch (PointFunction) {
		//			case Function.SupplyAmmo:
		//				tankInitSystem.GetComponentInChildren<PlayerState>().StartCoroutine(tankInitSystem.GetComponentInChildren<PlayerState>().UpdateAmmo(this.gameObject));
		//				break;
		//			case Function.CaptureArea:
		//				tankInitSystem.GetComponentInChildren<PlayerState>().StartCoroutine(tankInitSystem.GetComponentInChildren<PlayerState>().Capture(this.gameObject));
		//				break;
		//			case Function.EventCall:
		//				//EventManager.TriggerEvent (EventCallName);
		//				break;
		//			case Function.RepairTank:
		//				tankInitSystem.GetComponentInChildren<PlayerState>().StartCoroutine(tankInitSystem.GetComponentInChildren<PlayerState>().RepairTank(this.gameObject));
		//				break;
		//			}
		//	}
		//}
		#else 
		TankInitSystem tankInitSystem = cl.GetComponentInParent<TankInitSystem> ();
		if (tankInitSystem) {
			switch (PointFunction) {
			case Function.SupplyAmmo:
				GetComponent<PhotonView> ().RPC ("OnSupplyAmmo", PhotonTargets.All, tankInitSystem.PlayerID);
				break;
			case Function.CaptureArea:
				GetComponent<PhotonView> ().RPC ("OnCapturePoint", PhotonTargets.All, tankInitSystem.PlayerID);
				break;
			case Function.EventCall:
				EventManager.TriggerEvent (EventCallName);
				break;
			case Function.RepairTank:
				GetComponent<PhotonView> ().RPC ("OnRepairTank", PhotonTargets.All, tankInitSystem.PlayerID);
				break;
			}
		}
		#endif
	}
	//[PunRPC]
	//void OnRepairTank(int player){
	//	PlayerState playerState = GameNetwork.FindTankPlayer(player).GetComponentInChildren<PlayerState> ();
	//	if (playerState) {
	//		playerState.StartCoroutine(playerState.RepairTank(this.gameObject));
	//	}
	//}
	//[PunRPC]
	//void OnSupplyAmmo(int player){
	//	PlayerState playerState =  GameNetwork.FindTankPlayer(player).GetComponentInChildren<PlayerState> ();
	//	if (playerState) {
	//		playerState.StartCoroutine(playerState.UpdateAmmo(this.gameObject));
	//	}
	//}
	//[PunRPC]
	//void OnCapturePoint(int player){
	//	PlayerState playerState =  GameNetwork.FindTankPlayer(player).GetComponentInChildren<PlayerState> ();
	//	if (playerState) {
	//		playerState.StartCoroutine(playerState.Capture(this.gameObject));
	//	}
	//}

}
