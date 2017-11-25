using UnityEngine;
using System.Collections;

// Building 与 MasterClient 的中间层 用于监听 Building 倒塌事件 与 告知MasterClient RaiseEvent
public class MasterBuildingManagerModule : MonoBehaviour {
	//public MasterNetwork masterNetwork;

	public Hashtable DestroyedBuildingList  = new Hashtable();

	public MasterBuildingManagerModule () {
		Building[] SceneBuildings = GameObject.FindObjectsOfType<Building> ();
		for (int i = 0; i < SceneBuildings.Length; i++) {
			SceneBuildings [i].onDestroyed = HandleBuildingDestroyed;
			SceneBuildings [i].isMasterControl = true;
			SceneBuildings [i].isMasterServerObject = true;
		}
	}

	void HandleBuildingDestroyed(Building DestroyedBuidling){
		DestroyedBuildingList.Add (DestroyedBuidling.SerializedID, DestroyedBuidling.DestroyedDir);
		//masterNetwork.EventMasterBuildingDestroyed (DestroyedBuidling.SerializedID, DestroyedBuidling.DestroyedDir);
	}

}
