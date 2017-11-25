using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Linq;

[CustomEditor (typeof(VehicleData))]

public class VehicleDataEditor : EditorWindowBase
{
	VehicleData vehicleData;

	public override void Awake ()
	{
		base.Awake ();
		EditorHeadline = "ShanghaiWindy Ground Vehicle Manager";
	}

	public override void OnSelectionChanged ()
	{
		base.OnSelectionChanged ();
	}

    public override void ShortCut() {
        base.ShortCut();
        GameObject active = Selection.activeGameObject;
        var e = Event.current;
        if (e.keyCode == KeyCode.C) {
            if (active != null) {
                Vector3 P0 = active.transform.localPosition;
                Vector3 E0 = active.transform.localEulerAngles;
                vehicleData = (VehicleData)target;

                foreach (FieldInfo fieldInfo in vehicleData.cacheData.GetType().GetFields()) {
                    if (active.name == fieldInfo.Name) {
                        fieldInfo.SetValue(vehicleData.cacheData, new VehicleObjectTransformData() {
                            localPosition = P0,
                            localEulerAngle = E0
                        });
                        e.Use();
                        EditorHeadline = string.Format("Transform:{0} is updated to asset at{1}", active.name, System.DateTime.Now);
                        Repaint();
                    }
                }
            }

        }
    }

	public override void OnInspectorGUI ()
	{
		vehicleData = (VehicleData)target;


		BaseGUI ();

		if (GUILayout.Button ("Open Edit Mode")) {
			LockEditor ();
			OpenEditorScene ();

			TankInitSystem VehicleInstance = InitTankPrefabs ();
			//SetLOD (VehicleInstance);
		}
		if (GUILayout.Button ("Pack Asset")) {
			TankInitSystem VehicleInstance = InitTankPrefabs ();
			//SetLOD (VehicleInstance);

			PackAsset (VehicleInstance);
		}

		//EditorGUILayout.TextField ("Layout number", gd.);
		base.OnInspectorGUI ();

		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	void OpenEditorScene ()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
		EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects);
	}

	TankInitSystem InitTankPrefabs ()
	{

		#region 碰撞检测
		if (vehicleData.modelData.MainModel.GetComponentsInChildren<BoxCollider> ().Length == 0) {
			if (EditorUtility.DisplayDialog ("错误", "坦克模型必须设置完毕碰撞数据", "好的,立刻去设置", "重新选择资源")) {
				EditorGUIUtility.PingObject (vehicleData.modelData.MainModel.GetInstanceID ());
				OpenEditorScene ();

				GameObject EditColliderInstance = Instantiate<GameObject> (vehicleData.modelData.MainModel);
				EditColliderInstance.name = vehicleData.modelData.MainModel.name;

				string TempPath = System.IO.Path.GetDirectoryName (AssetDatabase.GetAssetPath (vehicleData.modelData.MainModel.GetInstanceID ()));
				string PrefabStoreDir = System.IO.Directory.CreateDirectory (string.Format (TempPath + "/Collisions_{0}", vehicleData.modelData.MainModel.name)).FullName;
					
				AssetDatabase.Refresh ();
				//PrefabUtility.CreatePrefab (string.Format (PrefabStoreDir + "/{0}.prefab", vehicleData.modelData.MainModel.name),EditColliderInstance);
			} else {
				return null;
			}
		}
		#endregion

		#region 初始化 坦克物体变量
		GameObject TankPrefabs = new GameObject ();
		GameObject InstanceMesh = Instantiate (vehicleData.modelData.MainModel);
		InstanceMesh.name = vehicleData.modelData.MainModel.name;
		#endregion

		TankPrefabs.name = InstanceMesh.name + "_Pre";

		InstanceMesh.transform.parent = TankPrefabs.transform;

		GameObject TankTransform = new GameObject ("TankTransform");
		TankTransform.transform.parent = InstanceMesh.transform;

		Transform RightWheel, LeftWheel, RightUpperWheels, LeftUpperWheels, Turret, Gun, GunDym;
		#region 在模型上寻找虚拟对象 
		RightWheel = InstanceMesh.transform.Find ("RightWheel");		
		LeftWheel = InstanceMesh.transform.Find ("LeftWheel");		
		Turret = InstanceMesh.transform.Find ("Turret");
		Gun = Turret.transform.Find ("Gun");
        GunDym = Gun.GetChild(0);
			
        RightUpperWheels = InstanceMesh.transform.Find ("RightUpperWheel");	
		LeftUpperWheels = InstanceMesh.transform.Find ("LeftUpperWheel");	
		#endregion
		RightWheel.parent = TankTransform.transform;		
		LeftWheel.parent = TankTransform.transform;			
		LeftUpperWheels.parent = TankTransform.transform;
		RightUpperWheels.parent = TankTransform.transform;	

		#region 实例化坦克的附加物体 坦克控制器 坦克脚本 坦克ui 坦克瞄准镜 主摄像机 坦克引擎声音 坦克引擎烟雾 死亡效果
		//EngineSmoke = new GameObject("EngineSmoke");
		//LeftTrackEffect = new GameObject("LeftTrackEffect");
		//RightTrackEffect=new GameObject("RightTrackEffect");
		#endregion
		GameObject TurretTransform = new GameObject ("TurretTransform");
		GameObject GunTransform = new GameObject ("GunTransform");
		GameObject GunDymTransform = new GameObject ("GunDym");

        VehicleComponentsReferenceManager referenceManager = InstanceMesh.AddComponent<VehicleComponentsReferenceManager>();

        //GameObject MainHitBox = new GameObject ("MainHitBox");
        //GameObject TurretHitBox = new GameObject ("TurretHitBox");


        TurretTransform.transform.SetParent (InstanceMesh.transform);
		TurretTransform.transform.position = Turret.transform.position;

		GunTransform.transform.position = Gun.transform.position;
		Turret.parent = TurretTransform.transform;

		Gun.parent = GunTransform.transform;
		GunTransform.transform.SetParent (TurretTransform.transform);

		GunDymTransform.transform.position = GunDym.transform.position;
		GunDymTransform.transform.SetParent (GunTransform.transform);
		GunDym.SetParent (GunDymTransform.transform);

		#region 发射动画
		Animator FireAnimator = GunDymTransform.AddComponent<Animator> ();
        FireAnimator.runtimeAnimatorController = vehicleData.vehicleTextData.TFParameter.GymAnimation;
        #endregion

        AddDumpNode("FFPoint", GunTransform.transform, true, referenceManager);
        AddDumpNode("EffectStart", GunTransform.transform, true, referenceManager);
        AddDumpNode("FireForceFeedbackPoint", InstanceMesh.transform, true, referenceManager);
        AddDumpNode("EngineSmoke", InstanceMesh.transform, true, referenceManager);
        AddDumpNode("EngineSound", InstanceMesh.transform, true, referenceManager);
        AddDumpNode("MainCameraFollowTarget", InstanceMesh.transform, true, referenceManager);
        AddDumpNode("MainCameraGunner", GunTransform.transform, true, referenceManager);
        AddDumpNode("MachineGunFFPoint", TurretTransform.transform, true, referenceManager);
        AddDumpNode("CenterOfGravity", InstanceMesh.transform, true, referenceManager);



        //MainHitBox.transform.parent = TankModel.transform;
        //TurretHitBox.transform.parent = TurretTransform.transform;

        //EngineSmoke.transform.parent = TankModel.transform;
        //EngineSmoke.tag = "EngineSmoke";



        //		GameObject LODMesh = new GameObject ("LODMesh");
        //		LODMesh.transform.SetParent (TankModel.transform);
        //		LODMesh.tag = "LODMesh";

        GameObject HitBoxInstance = Instantiate<GameObject> (vehicleData.modelData.HitBox.HitBoxPrefab);
		HitBoxInstance.transform.Find ("Main").name = "MainHitBox";
		HitBoxInstance.transform.Find ("MainHitBox").SetParent (InstanceMesh.transform);

		HitBoxInstance.transform.Find ("Turret").name = "TurretHitBox";
		HitBoxInstance.transform.Find ("TurretHitBox").SetParent (TurretTransform.transform);

		HitBoxInstance.transform.Find ("Gun").name = "GunHitBox";
		HitBoxInstance.transform.Find ("GunHitBox").SetParent (GunTransform.transform);

		HitBoxInstance.transform.Find ("Dym").name = "DymHitBox";
		HitBoxInstance.transform.Find ("DymHitBox").SetParent (GunDymTransform.transform);

		DestroyImmediate (HitBoxInstance);
		//HitBoxInstance.transform.Find("Dym").SetParent()
		Restore (LeftWheel.GetComponentsInChildren<Transform> ());
		Restore (RightWheel.GetComponentsInChildren<Transform> ());

		Restore (LeftUpperWheels.GetComponentsInChildren<Transform> ());
		Restore (RightUpperWheels.GetComponentsInChildren<Transform> ());

		new GameObject ("MainCameraTarget").transform.SetParent (TurretTransform.transform);

        TankInitSystem initySystem = TankPrefabs.AddComponent<TankInitSystem>();
        initySystem.PSParameter = vehicleData.vehicleTextData.PSParameter;
        initySystem.TFParameter = vehicleData.vehicleTextData.TFParameter;
        initySystem.MTParameter = vehicleData.vehicleTextData.MTParameter;
        initySystem.PTCParameter = vehicleData.vehicleTextData.PTCParameter;

        return initySystem;
	}

	public Transform[] Restore (Transform[] t)
	{
		Transform[] ReturnTransform = new Transform[t.Length - 1];
		int i;
		for (i = 0; i < t.Length; i++) {
			if (i != 0)
				ReturnTransform [i - 1] = t [i];
		}
		Transform temp = null;
		for (i = 0; i < ReturnTransform.Length - 1; i++) { 
			for (int j = i + 1; j < ReturnTransform.Length; j++) { 
				if (ReturnTransform [i].localPosition.y > ReturnTransform [j].localPosition.y) { 
					temp = ReturnTransform [i]; 
					ReturnTransform [i] = ReturnTransform [j]; 
					ReturnTransform [j] = temp; 
				} 
				ReturnTransform [i].name = "w" + i.ToString ();
				ReturnTransform [i].SetAsLastSibling ();
			} 

		}
		ReturnTransform [ReturnTransform.Length - 1].name = "w" + (ReturnTransform.Length - 1).ToString ();
		ReturnTransform [ReturnTransform.Length - 1].SetAsLastSibling ();

		return ReturnTransform;
	}

	void PackAsset (TankInitSystem tankInitSystem)
	{
		string CurrentAssetName = tankInitSystem.transform.GetChild (0).name;

		string Path = "Assets/res/Cooked/" + CurrentAssetName.ToLower ();
//
//		//System.IO.DirectoryInfo folder = new System.IO.DirectoryInfo ("Assets/Resources/ExtraPackage/TankModel/" + CurrentAssetName);
//		if (!folder.Exists)
//			folder.Create ();


		//PrefabUtility.CreatePrefab (Path + "_PreBak.prefab", ((TankInitSystem)target).gameObject);

		GameObject Prefab = tankInitSystem.transform.GetChild (0).gameObject;

		#region 游戏内模型 预制体处理
		GameObject Origin = PrefabUtility.CreatePrefab(Path + "_Pre.prefab", Prefab); // 要打包的物体



		#region 客户端打包文件
		AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Origin));
		assetImporter.assetBundleName = CurrentAssetName + "_Pre";
		assetImporter.assetBundleVariant = "clientextramesh";
		#endregion


		ProjectWindowUtil.ShowCreatedAsset(tankInitSystem.gameObject);
		#endregion

		#region 服务器资源处理

		foreach (MeshRenderer meshRenderer in Prefab.GetComponentsInChildren<MeshRenderer>()) {
			DestroyImmediate(meshRenderer);
		}
		foreach (MeshFilter mesh in Prefab.GetComponentsInChildren<MeshFilter>()) {
			DestroyImmediate(mesh);
		}


		#region 服务器打包文件
		GameObject DelicateAsset = PrefabUtility.CreatePrefab("Assets/res/Cooked/DelicatedServer/" + CurrentAssetName.ToLower() + "_Pre" + ".prefab", Prefab);
		assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(DelicateAsset));
		assetImporter.SetAssetBundleNameAndVariant(CurrentAssetName + "_Pre", "masterextramesh");
		#endregion

		#endregion

		DestroyImmediate(Prefab);
		DestroyImmediate(tankInitSystem.gameObject);

	}
    void SetLOD(TankInitSystem root) {
        Transform vehicleComponentRoot = root.transform.GetChild(0); //主要组件位置

        MeshFilter[] OriginalMeshes = root.GetComponentsInChildren<MeshFilter>();

        if (vehicleData.modelData.LOD == null)
            return;

        GameObject VehicleLOD = Instantiate(vehicleData.modelData.LOD);
        VehicleLOD.name = vehicleData.modelData.LOD.name;
        VehicleLOD.transform.SetParent(vehicleComponentRoot);


        LODGroup MainLODgroup = vehicleComponentRoot.gameObject.AddComponent<LODGroup>();

        List<Renderer> MainMainMeshes = new List<Renderer>();
        List<Renderer> MainLODMeshes = new List<Renderer>();

        GetRenderByName(vehicleComponentRoot.transform, ref MainMainMeshes, "MainBody", "TankTransform", "TurretTransform");
        GetRenderByName(VehicleLOD.transform, ref MainLODMeshes, "LeftUpperWheel", "LeftWheel", "MainBody", "RightUpperWheel", "RightWheel", "Turret");

        MainLODgroup.SetLODs(new LOD[] {
            new LOD (0.25f, MainMainMeshes.ToArray()),
            new LOD (0, MainLODMeshes.ToArray ())
        });



        Transform TurretTransform = vehicleComponentRoot.transform.Find("TurretTransform");

        Transform Turret = TurretTransform.Find("Turret");
        Transform Gun = TurretTransform.Find("GunTransform/Gun");
        Transform Dym = TurretTransform.Find("GunTransform/GunDym/Dym");

        Transform LODTurret = VehicleLOD.transform.Find("Turret_LOD");
        Transform LODGun = LODTurret.GetChild(0);
        Transform LODDym = LODGun.GetChild(0);

        LODTurret.SetParent(Turret.parent);
        LODGun.SetParent(Gun.parent);
        LODDym.SetParent(Dym.parent);




    }


    private void AddDumpNode(string dumpName, Transform parent, bool isAddedToReference = false, VehicleComponentsReferenceManager referenceManager = null) {
        GameObject DumpNode = new GameObject(dumpName);
        DumpNode.transform.SetParent(parent);
        DumpNode.transform.localPosition = ((VehicleObjectTransformData)vehicleData.cacheData.GetType().GetField(dumpName).GetValue(vehicleData.cacheData)).localPosition;
        DumpNode.transform.localEulerAngles = ((VehicleObjectTransformData)vehicleData.cacheData.GetType().GetField(dumpName).GetValue(vehicleData.cacheData)).localEulerAngle;
        IconManager.SetIcon(DumpNode, IconManager.LabelIcon.Orange);
        if (isAddedToReference) {
            referenceManager.GetType().GetField(dumpName).SetValue(referenceManager, DumpNode);
        }
    }
    private Transform[] GetChilds(Transform t) {
        return t.GetComponentsInChildren<Transform>().Where(child => child.parent == t).ToArray();
    }

    private void GetRenderByName(Transform Root, ref List<Renderer> RenderList, params string[] RootChildMatchNames) {
        foreach (Transform ChildT in GetChilds(Root)) {
            bool isNameValid = false;

            foreach (string MatchName in RootChildMatchNames) {
                if (ChildT.name.Contains(MatchName)) {
                    isNameValid = true;
                }
            }

            if (!isNameValid)
                continue;


            foreach (Renderer Child in ChildT.GetComponentsInChildren<Renderer>()) {
                if (Child.GetComponent<HitBox>() != null)
                    continue;
                RenderList.Add(Child);
            }

        }
    }
}
