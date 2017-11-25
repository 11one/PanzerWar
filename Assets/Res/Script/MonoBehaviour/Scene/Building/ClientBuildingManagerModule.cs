using UnityEngine;

using Generic = System.Collections.Generic;
//监听服务器 的倒塌事件 实现倒塌效果的展现
public class ClientBuildingManagerModule  {
	public System.Action<int,Vector3> onServerBuildingDestroyed;

	public Generic.Dictionary<int,Building> BuildingList  = new Generic.Dictionary<int, Building>();

	public ClientBuildingManagerModule(){
		Building[] SceneBuildings = GameObject.FindObjectsOfType<Building> ();
		for (int i = 0; i < SceneBuildings.Length; i++) {
			SceneBuildings [i].isMasterControl = true;
			SceneBuildings [i].isMasterServerObject = false;

			BuildingList.Add (SceneBuildings [i].SerializedID,SceneBuildings [i]);
		}
		onServerBuildingDestroyed = HandleServerBuildingDestroyed;

	}

	private void HandleServerBuildingDestroyed(int SerializedID,Vector3 Dir){
		if (BuildingList.ContainsKey (SerializedID)) {
			Building TargetBuilding = BuildingList [SerializedID];
			if (TargetBuilding.DestroyType == BuildingDestroyType.ByCollapseAnimation) {
				TargetBuilding.PlayCollapseAnimation (Dir,10);
			} else if (TargetBuilding.DestroyType == BuildingDestroyType.ByModelReplace) {
				TargetBuilding.ShowCollapseModel ();
			}
		}
	}
}
