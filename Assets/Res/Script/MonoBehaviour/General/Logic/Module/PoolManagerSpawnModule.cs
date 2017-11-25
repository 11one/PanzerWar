using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManagerSpawnModule : MonoBehaviour {
	public static PoolManagerSpawnModule Instance;

	public System.Action<string,Vector3,Vector3> onServerSpawnedObject; //Client 

	public System.Action<string,Vector3,Vector3> onGameObjectRaisedSpawnEvent; //Master 物体

	public void HandleMasterSpawnedObject(string spawnedObject,Vector3 p,Vector3 e){
		PoolManager.CreateObject (spawnedObject, p, e);
	}
	void Awake(){
		Instance = this;
	}

	public static void CreateObject(string spawnedObject,Vector3 p,Vector3 e){
        if (GameDataManager.OfflineMode)
			PoolManager.CreateObject (spawnedObject, p, e);
		
	}
}
