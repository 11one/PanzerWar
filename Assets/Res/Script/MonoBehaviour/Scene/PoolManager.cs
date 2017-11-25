using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {
	Dictionary<string,int> ParamsList = new Dictionary<string, int>();
	public static List<string> LoadedObjectName = new List<string> ();

	public static PoolManager _Instance;
	[System.Serializable]
	public class Params{
		public string ObjectName;
		public GameObject Object;
		public int CreateCount =1;
		public float LifeTime =5;
		public Dictionary<GameObject,bool> ObjectList = new Dictionary<GameObject, bool>();
		public bool isBuffer =false;
		public void Init(){
			#if ClientCode
			for (int i =0; i<CreateCount; i++) {
				GameObject TempObject = (GameObject)Instantiate(Object);
				ObjectList.Add(TempObject,false);
				TempObject.transform.SetParent (_Instance.transform);
				if(isBuffer){
					TempObject.SetActive(true);
					PoolManager._Instance.StartCoroutine(Sync(TempObject));
				}
				else {
					TempObject.SetActive(false);
				}
			}
			#endif 
		}
		IEnumerator Sync(GameObject poolObject){
			yield return new WaitForSeconds (5);
			poolObject.SetActive (false);
		}
	}
	public Params[] Objects;

	void Awake(){
		InitParams ();
		InitOnHitSound ();
		#if ClientCode

		#else 
		Objects = new Params[0];
		Resources.UnloadUnusedAssets();
		#endif
		//UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
	}
	protected void InitParams(){
		_Instance = this;
		for (int i =0; i<Objects.Length; i++) {
			Objects[i].Init();
			LoadedObjectName.Add(Objects[i].ObjectName);
			ParamsList.Add(Objects[i].ObjectName,i);
		}
	}
	public static GameObject CreateObject(string GET_Name,Vector3 GET_Position,Vector3 GET_Rotation){
		if (_Instance == null)
			return null;
		
		if (_Instance.ParamsList.ContainsKey (GET_Name) == false) {
			Debug.Log ("Missing Resourece");
			return null;
		} 

		int Index = _Instance.ParamsList [GET_Name];
		Params MyParams = _Instance.Objects [Index];
		float LifeTime = MyParams.LifeTime;
		foreach (GameObject Key in MyParams.ObjectList.Keys) {
			if (Key == null)
				continue;
			
			if(MyParams.ObjectList[Key] == false){
				Key.transform.position = GET_Position;
				Key.transform.eulerAngles = GET_Rotation;
				Key.SetActive(true);
				_Instance.StartCoroutine(_Instance.DisActiveObject(MyParams.ObjectList,Key,LifeTime,Key.name));
				return  Key;
			}
		}
		return null;

	}
	protected IEnumerator DisActiveObject(Dictionary<GameObject,bool> TheDictionary,GameObject Key,float WaitTime,string objectName){

		TheDictionary [Key] = true;
		yield return new WaitForSeconds (WaitTime);
		TheDictionary [Key] = false;
		Key.SetActive (false);
	}
	public static void UpdateParams(string ObjectName,GameObject oj,int CreateCount,float LifeTime,bool isBuffer){
		#if ClientCode
		if (LoadedObject (ObjectName) == false) {
			Params newParams = new Params ();
			newParams.ObjectName = ObjectName;
			newParams.Object = oj;
			newParams.LifeTime = LifeTime;
			newParams.CreateCount = CreateCount;
			newParams.isBuffer =isBuffer;
			newParams.Init ();
			_Instance.Objects = ParamsAdd (_Instance.Objects, newParams);
			_Instance.ParamsList.Add (ObjectName, _Instance.Objects.Length - 1);
			LoadedObjectName.Add (ObjectName);
		} else {
			Debug.Log ("Conflict");
		}
		#endif
	}
	public static bool LoadedObject(string ObjectName){
		if (LoadedObjectName.Contains (ObjectName)||_Instance.ParamsList.ContainsKey(ObjectName))
			return true;
		else 
			return false;
	}
	protected static  Params[] ParamsAdd(Params[] Target,Params AddedElement){
		Params[] New = new Params[Target.Length + 1];
		for (int i =0; i<Target.Length; i++) {
			New[i] = Target[i];
		}
		New [New.Length - 1] = AddedElement;
		return New;
	}
	void OnLevelWasLoaded() {
		LoadedObjectName = new List<string> ();
	}
	void InitOnHitSound(){
		PoolManager.UpdateParams ("ap_critical_hit_huge",(GameObject)Resources.Load("Audio/Sounds/res/Hit/ap_critical_hit_huge"),3,5,false);
		PoolManager.UpdateParams ("ap_critical_hit_medium",(GameObject)Resources.Load("Audio/Sounds/res/Hit/ap_critical_hit_medium"),3,5,false);
		PoolManager.UpdateParams ("ap_critical_hit_small",(GameObject)Resources.Load("Audio/Sounds/res/Hit/ap_critical_hit_small"),3,5,false);
		PoolManager.UpdateParams ("apcr_ricochet_main",(GameObject)Resources.Load("Audio/Sounds/res/Hit/apcr_ricochet_main"),3,5,false);
		PoolManager.UpdateParams ("ap_armor_not_pierce_main",(GameObject)Resources.Load("Audio/Sounds/res/Hit/ap_armor_not_pierce_main"),3,5,false);
	}

}