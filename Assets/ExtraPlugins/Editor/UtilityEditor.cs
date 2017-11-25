using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Collections.Generic;


[CustomEditor(typeof(UtilityEditor))] 
[ExecuteInEditMode]
public class UtilityEditor :EditorWindow{
	
	public TankFire.NationType nationType;
	public BuildTarget runtimePlatform = BuildTarget.Android;

	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		#if UNITY_ANDROID
		PlayerSettings.applicationIdentifier = "com.shanghaiwindy.TankWar";
		#endif
		#if UNITY_IPHONE
		PlayerSettings.applicationIdentifier = "com.shanghaiwindy.Waroftanks";
		#endif

		if (new DirectoryInfo("Assets/StreamingAssets/TWRPackages").Exists) {
			if (new DirectoryInfo("Assets/StreamingAssets/TWRPackages").GetFiles().Length == 0) {
				EditorUtility.DisplayDialog("Warning", "Forget to show AssetBundle Folder", "OK");
				ShowTWRFolder();
			}
		}
	}
	[MenuItem("Waroftanks/Android/Keystore")]
	static void ChangeKeyStore(){
		PlayerSettings.keyaliasPass = "28858991";
		PlayerSettings.keystorePass = "28858991";

		#if UNITY_ANDROID
		PlayerSettings.applicationIdentifier = "com.shanghaiwindy.TankWar";
		#endif
		#if UNITY_IPHONE
		PlayerSettings.applicationIdentifier = "com.shanghaiwindy.Waroftanks";
		#endif

	}

	[MenuItem("Waroftanks/Show TWRFolder")]
	static void ShowTWRFolder(){
		if (new DirectoryInfo ("Assets/StreamingAssets/TWRPackages").Exists) {
			if (new DirectoryInfo ("Assets/StreamingAssets/TWRPackages").GetFiles ().Length == 0) {
				new DirectoryInfo ("Assets/StreamingAssets/TWRPackages").Delete ();
				System.IO.File.Move ("Assets/StreamingAssets/.TWRPackages", "Assets/StreamingAssets/TWRPackages");
			}
		} else {
			System.IO.File.Move ("Assets/StreamingAssets/.TWRPackages", "Assets/StreamingAssets/TWRPackages");
		}
	}

	[MenuItem("Waroftanks/Hide TWRFolder")]
	static void HideTWRFolder(){
		if (new DirectoryInfo ("Assets/StreamingAssets/.TWRPackages").Exists) {
			if (new DirectoryInfo ("Assets/StreamingAssets/.TWRPackages").GetFiles ().Length == 0) {
				new DirectoryInfo ("Assets/StreamingAssets/.TWRPackages").Delete ();
				System.IO.File.Move ("Assets/StreamingAssets/TWRPackages", "Assets/StreamingAssets/.TWRPackages");
			}
		} else {
			System.IO.File.Move ("Assets/StreamingAssets/TWRPackages", "Assets/StreamingAssets/.TWRPackages");
		}

	}

	[MenuItem("Waroftanks/Open Garage")]
	static void LoadGarage(){
		UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Package/Sence/garage/garage.unity");
	}

	GameObject TestSingleAssetBuild;


	[MenuItem("Waroftanks/Open Login")]
	static void LoadLogin(){
		UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Package/Sence/Login/Login.unity");
	}

	[MenuItem("Waroftanks/Waroftanks Easy Setup")]
	static void Init(){
		Rect wr = new Rect (0,0,500,500);
		UtilityEditor window = (UtilityEditor)EditorWindow.GetWindowWithRect (typeof (UtilityEditor),wr,true,"Easy Tank Easy World -----TankWar");
		window.Show();
	}


	

	public static string GetMD5HashFromFile(string fileName)
	{
		try
		{
			FileStream file = new FileStream(fileName, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (System.Exception ex)
		{
			Debug.Log (ex.Source);
			return null;
		}
	}

	GameObject myTurret;
	void InitTurret(){
		GameObject InstanceTurret = new GameObject ("TurretTransform");
		GameObject InstanceGun = new GameObject ("GunTransform");
		InstanceTurret.tag = "Turret";
		InstanceTurret.transform.SetParent (myTurret.transform.parent);
		InstanceTurret.transform.localPosition = myTurret.transform.localPosition;
		InstanceTurret.transform.eulerAngles = Vector3.zero;
		InstanceGun.transform.SetParent (InstanceTurret.transform);
		InstanceGun.transform.localPosition = myTurret.transform.GetChild(0).localPosition;
		InstanceGun.transform.eulerAngles = Vector3.zero;
		myTurret.transform.GetChild (0).SetParent (InstanceGun.transform);
		myTurret.transform.SetParent (InstanceTurret.transform);
		GameObject FFPoint = new GameObject ("FFPoint");
		FFPoint.transform.SetParent (InstanceGun.transform);
		FFPoint.transform.localPosition = InstanceGun.transform.localPosition;
		FFPoint.transform.eulerAngles = Vector3.zero;
		GameObject FireEffect = new GameObject ("FireEffect");
		FireEffect.transform.SetParent (InstanceGun.transform);
		FireEffect.transform.localPosition = InstanceGun.transform.localPosition;
		FireEffect.transform.eulerAngles = Vector3.zero;
	}
	GameObject TankModel;

	void InitTankPrefabs(){
		#region 初始化 坦克物体变量
		GameObject EngineSmoke,LeftTrackEffect,RightTrackEffect;
		GameObject TankPrefabs = new GameObject();
		#endregion
		TankPrefabs.name = TankModel.name+"_Pre";

		TankModel.transform.parent = TankPrefabs.transform;
	
		GameObject TankTransform = new GameObject ("TankTransform");
		TankTransform.transform.parent = TankModel.transform;

		Transform RightWheel, RightTrack, LeftWheel, LeftTrack,RightUpperWheels,LeftUpperWheels,Turret,Gun;
		#region 在模型上寻找虚拟对象 
		RightWheel = TankModel.transform.Find ("RightWheel");		RightTrack = TankModel.transform.Find ("RightTrack");
		LeftWheel = TankModel.transform.Find ("LeftWheel");		LeftTrack = TankModel.transform.Find ("LeftTrack");
		Turret = TankModel.transform.Find ("Turret");		Gun = Turret.transform.Find ("Gun");	
		RightUpperWheels = TankModel.transform.Find ("RightUpperWheel");	
		LeftUpperWheels = TankModel.transform.Find ("LeftUpperWheel");	

		#endregion
		RightWheel.parent = TankTransform.transform;		RightTrack.parent = TankTransform.transform;
		LeftWheel.parent = TankTransform.transform;			LeftTrack.parent = TankTransform.transform;
		LeftUpperWheels.parent = TankTransform.transform;			RightUpperWheels.parent = TankTransform.transform;	

		#region 实例化坦克的附加物体 坦克控制器 坦克脚本 坦克ui 坦克瞄准镜 主摄像机 坦克引擎声音 坦克引擎烟雾 死亡效果
		EngineSmoke = new GameObject("EngineSmoke");
		LeftTrackEffect = new GameObject("LeftTrackEffect");
		RightTrackEffect=new GameObject("RightTrackEffect");
		#endregion
		GameObject FFPoint = new GameObject("FFPoint");
		GameObject EffectStart = new GameObject("EffectStart");
		GameObject TurretTransform = new GameObject ("TurretTransform"),GunTransform = new GameObject ("GunTransform");
		GameObject MainHitBox = new GameObject ("MainHitBox");
		GameObject TurretHitBox = new GameObject ("TurretHitBox");


		TurretTransform.transform.parent = TankModel.transform;
		Debug.Log (TurretTransform.transform.parent);
		TurretTransform.transform.position = Turret.transform.position;GunTransform.transform.position = Gun.transform.position;
		Turret.parent = TurretTransform.transform;
		Gun.parent = GunTransform.transform;
		GunTransform.transform.parent = TurretTransform.transform;
		FFPoint.transform.parent = Gun;
		FFPoint.transform.localPosition = new Vector3(0,0,0);
		EffectStart.transform.parent = Gun;
		EffectStart.transform.localPosition = new Vector3(0,0,0);
		MainHitBox.transform.parent = TankModel.transform;
		TurretHitBox.transform.parent = TurretTransform.transform;

		EngineSmoke.transform.parent = TankModel.transform;
		EngineSmoke.tag = "EngineSmoke";

		LeftTrackEffect.transform.parent = TankModel.transform;
		RightTrackEffect.transform.parent = TankModel.transform;

		GameObject LODMesh = new GameObject ("LODMesh");
		LODMesh.transform.SetParent (TankModel.transform);
		LODMesh.tag = "LODMesh";
		TankPrefabs.AddComponent<TankInitSystem> ();
		EditorUtility.DisplayDialog("提示","设置坦克数值不要忘记了! ","好的");
	}




	public Transform[] ExcpetSelf(Transform[] t){
		Transform[]  ReturnTransform = new Transform[t.Length-1];
		int i;
		for (i=0;i<t.Length;i++){
			if(i!=0)
				ReturnTransform[i-1] = t[i];
		}
		Transform temp = null;
		for (i = 0; i < ReturnTransform.Length - 1; i++) 
		{ 
			for (int j = i + 1; j < ReturnTransform.Length; j++) 
				
			{ 
				if (ReturnTransform[i].localPosition.y > ReturnTransform[j].localPosition.y )
				{ 
					temp = ReturnTransform[i]; 
					ReturnTransform[i] = ReturnTransform[j]; 
					ReturnTransform[j] = temp; 
				} 
				ReturnTransform[i].name = "Wheel_"+i.ToString();
			} 
			
		}
		ReturnTransform[ReturnTransform.Length-1].name = "Wheel_"+(ReturnTransform.Length-1).ToString();

		return ReturnTransform;
	}
	/// <summary>s
	/// 写入INI文件
	/// </summary>
	/// <param name="section">节点名称[如[TypeName]]</param>
	/// <param name="key">键</param>
	/// <param name="val">值</param>
	/// <param name="filepath">文件路径</param>
	/// <returns></returns>
	[DllImport("kernel32")]
	private static extern long WritePrivateProfileString(string section,string key,string val,string filepath);

}
