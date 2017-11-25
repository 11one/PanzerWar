using UnityEngine;
using UnityEngine.UI;
using MaterialUI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
//using WaroftanksClientSDK;
using ShanghaiWindy.Client;


public class AssetBundleManager:MonoBehaviour{
	public static Dictionary<string,AssetBundle> LoadedAssets = new Dictionary<string, AssetBundle>();

    private static AssetBundleManifest assetBundleManifest;

    private static MonoBehaviour MonoActiveObject;

    private static bool hasTaskToFinish = false;

    private static Queue<AssetRequestTask> assetTaskQueue = new Queue<AssetRequestTask>();


    public static void Init(MonoBehaviour _monoActiveObject) {
        MonoActiveObject = _monoActiveObject;
    }

    public static IEnumerator GetAssetBundleInfos(System.Action onFinish) {
        WWW www = null;

        www = new WWW(GetStreamingAssetsPath() + "main.packages");

        yield return www;

        assetBundleManifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AssetBundleManifest");

        if (onFinish != null) {
            onFinish();
        }
    }

    public static IEnumerator UncompressAssetBundles(System.Action<bool> onFinish){
        GameObject LoadingEffect = Instantiate((GameObject)Resources.Load("UI/AssetUncompressLoading"), Vector3.zero, Quaternion.identity);
        LinearProgressIndicator ProgressSlider = LoadingEffect.transform.Find("Progress").GetComponent<LinearProgressIndicator>();
        Text assetText = LoadingEffect.transform.Find("Text").GetComponent<Text>();

        string[] assetBundlesToUncompress = assetBundleManifest.GetAllAssetBundles();

        for (int i = 0; i < assetBundlesToUncompress.Length;i++){
            assetText.text = string.Format("Uncompress game assets. Progress{0}/{1} Current{2}", i, assetBundlesToUncompress.Length, assetBundlesToUncompress[i]);
            if (assetBundlesToUncompress[i].Contains("master"))
                continue;
            
            WWW www = new WWW(GetStreamingAssetsPath() + assetBundlesToUncompress[i]);
            while(!www.isDone){
                ProgressSlider.SetProgress(www.progress,true);
                yield return new WaitForEndOfFrame();
            }

            if(string.IsNullOrEmpty(www.error)){
                System.IO.File.WriteAllBytes(GetUncompressAssetsPath()+assetBundlesToUncompress[i],www.bytes);
            }else{
                DialogManager.ShowAlert(string.Format("Uncompress assets failed Reason{0} Please contact the developer for further detial.",www.error),"Critical Error",MaterialIconHelper.GetIcon(MaterialIconEnum.ERROR));
                onFinish(false);
                yield break;
            }
            www.Dispose();
        }
        Destroy(LoadingEffect);

        onFinish(true);

        yield break;
    }


	public static IEnumerator RequestScene(bool InBuilt,bool Cooked,string SceneName,System.Action<bool> SceneBaseLoadFinish = null,System.Action<bool> SceneLoadFinish = null,string CustomMapUrl = null){
		#if ClientCode
		GameObject LoadingEffect = (GameObject)Instantiate((GameObject)Resources.Load("UI/LoadingEffect"),Vector3.zero,Quaternion.identity);
		LinearProgressIndicator ProgressSlider = LoadingEffect.transform.Find("Progress").GetComponent<LinearProgressIndicator>();
		ProgressSlider.SetProgress(0);
		DontDestroyOnLoad (LoadingEffect);
		#endif

		AudioListener.volume = 0;

		List<string> ReleaseAssets = new List<string>();

		foreach (string Key in AssetBundleManager.LoadedAssets.Keys) {
			ReleaseAssets.Add (Key);
		}

		foreach (string key in ReleaseAssets) {
			if (AssetBundleManager.LoadedAssets [key] != null) {
				AssetBundleManager.LoadedAssets [key].Unload (true);
			}
			AssetBundleManager.LoadedAssets.Remove (key);
		}

		LoadedAssets = new Dictionary<string, AssetBundle> ();

		Resources.UnloadUnusedAssets ();
		#if ClientCode
		ProgressSlider.SetProgress(0.25f);
		#endif

		AsyncOperation Asyn  = SceneManager.LoadSceneAsync("PoolScene");
		yield return Asyn;

		#if ClientCode
		ProgressSlider.SetProgress(0.1f);
		#endif

		TankInitSystem[] Vehicles = GameObject.FindObjectsOfType<TankInitSystem> ();
		for (int i = 0; i < Vehicles.Length; i++) {
			while (Vehicles [i] != null&&Vehicles[i].isAssetBundleLoaded == false) {
				yield return new WaitForEndOfFrame ();
			}
		}

		#if ClientCode
		ProgressSlider.SetProgress(0.95f);
		#endif


		if (Cooked) {
			AsyncOperation LoadSceneOperation = SceneManager.LoadSceneAsync (SceneName + "_Cooked");
			yield return LoadSceneOperation;

			Asyn = SceneManager.LoadSceneAsync ("LoadScenePreparation", LoadSceneMode.Additive);
			yield return Asyn;

			if(SceneBaseLoadFinish!=null)
				SceneBaseLoadFinish (true);

			#if ClientCode
			if (RenderSettings.skybox != null) {
				RenderSettings.skybox.shader = Shader.Find (RenderSettings.skybox.shader.name);
			}
			#endif
			SceneAssetPrefab[] Sceneassets = GameObject.FindObjectsOfType<SceneAssetPrefab> ();


			#if ClientCode
			ProgressSlider.SetProgress(0,false);
#endif

            int loadedAssetsCount = 0;

			for (int i = 0; i < Sceneassets.Length; i++) {
				#if ClientCode
				ProgressSlider.SetProgress((float)i/(float)Sceneassets.Length,true);
				#endif

				Sceneassets [i].inLoading = true;
				Sceneassets [i].LoadAsset ();

                Sceneassets[i].onFinish = () => {
                    loadedAssetsCount += 1;
                };

			}

            while(loadedAssetsCount !=Sceneassets.Length){
#if ClientCode
                ProgressSlider.SetProgress((float)loadedAssetsCount / (float)Sceneassets.Length, true);
#endif
                yield return new WaitForEndOfFrame();
            }



			#if ServerSide
			if (GameNetwork.ServerDisShowModel) {
				yield return new WaitForSeconds (1);

				Canvas[] uis =GameObject.FindObjectsOfType<Canvas>();
				foreach (Canvas ui in uis) {
					DestroyImmediate (ui.gameObject,true);
				}

				ParticleSystem[] particleSystems =GameObject.FindObjectsOfType<ParticleSystem>();
				foreach (ParticleSystem particleSystem in particleSystems) {
					if(particleSystem!=null)
						DestroyImmediate (particleSystem.gameObject,true);
				}

				SpriteRenderer[] Sprites =GameObject.FindObjectsOfType<SpriteRenderer>();
				foreach (SpriteRenderer sprite in Sprites) {
					DestroyImmediate (sprite.gameObject,true);
				}


				MeshRenderer[] meshRenderers =GameObject.FindObjectsOfType<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in meshRenderers) {
					DestroyImmediate (meshRenderer,true);
				}
				MeshFilter[] meshFilters =GameObject.FindObjectsOfType<MeshFilter>();
				foreach (MeshFilter meshFilter in meshFilters) {
					DestroyImmediate (meshFilter,true);
				}

				Terrain[] terrains = GameObject.FindObjectsOfType<Terrain> ();
				foreach (Terrain terrain in terrains) {
					Destroy (terrain);
				}

			}
			#endif
		} else if (InBuilt) {
			Asyn = SceneManager.LoadSceneAsync (SceneName);
			yield return Asyn;
		} else if (!InBuilt) {
			WWW www = new WWW	 (CustomMapUrl);
			yield return www;

			//www.assetBundle.LoadAllAssets ();
			Asyn = SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(www.assetBundle.GetAllScenePaths()[0]));

			yield return Asyn;

			RenderSettings.skybox.shader = Shader.Find(RenderSettings.skybox.shader.name);



			foreach (MeshRenderer meshRenderer in GameObject.FindObjectsOfType<MeshRenderer>()) {
				foreach (Material material in meshRenderer.sharedMaterials) {
					material.shader = Shader.Find (material.shader.name);
				}
			}



			www.Dispose ();

		}
		#if ClientCode
		Destroy (LoadingEffect);
		Instantiate (Resources.Load ("ResourcePrefab/Skidmarks"));
		#endif



		if (SceneLoadFinish != null) {
			SceneLoadFinish (true);
		}

		AudioListener.volume = 1;


		Resources.UnloadUnusedAssets ();


	}

    public static IEnumerator LoadAssetFromResources(System.Action<GameObject> ReturnObject, string AssetName) {
        ResourceRequest resourceRequest = null;
        resourceRequest = Resources.LoadAsync(AssetName);
        yield return resourceRequest;
        ReturnObject(Instantiate<GameObject>((GameObject)resourceRequest.asset));
    }

    public static void LoadAssetFromAssetBundle(AssetRequestTask assetRequestTask){
        assetTaskQueue.Enqueue(assetRequestTask);
        hasTaskToFinish = true;
    }

    public static IEnumerator AssetBundleLoop(){
        while(true){
            while (hasTaskToFinish == false){
                yield return new WaitForEndOfFrame();
            }
            //处理Task
            while(assetTaskQueue.Count>0){
                AssetRequestTask current = assetTaskQueue.Dequeue();
                //AssetBundle已经被读取 
                if(LoadedAssets.ContainsKey(current.GetABName())){
                    string assetPath = LoadedAssets[current.GetABName()].GetAllAssetNames()[0];

                    AssetBundleRequest abRequest = LoadedAssets[current.GetABName()].LoadAssetAsync(assetPath);

                    yield return abRequest;

                    current.onAssetLoaded(abRequest.asset);
                }
                //AssetBundle尚未读取
                else {
                    //依赖包 Loop
                    string[] DependenciesInfo = assetBundleManifest.GetAllDependencies(current.GetABName());
                    for (int i = 0; i < DependenciesInfo.Length;i++){
                        if(LoadedAssets.ContainsKey(DependenciesInfo[i])){
                            continue;
                        }
                        bool isDependenceAssetLoaded = false;

                        MonoActiveObject.StartCoroutine(LoadAsset(DependenciesInfo[i],()=>{
                            isDependenceAssetLoaded = true;
                        }));

                        while(!isDependenceAssetLoaded){
                            yield return new WaitForFixedUpdate();
                        }
                    }
                    //主AB
                    bool isMainLoaded = false;

                    MonoActiveObject.StartCoroutine(LoadAsset(current.GetABName(), () => {
                        isMainLoaded = true;
                    }));

                    while (!isMainLoaded)
                        yield return new WaitForFixedUpdate();

                    //添加回队列 等待队列回调
                    assetTaskQueue.Enqueue(current);
                }
            }
            hasTaskToFinish = false;
        }
    }

    private static IEnumerator LoadAsset(string assetBundleName,System.Action onFinish){
        WWW www = new WWW(GetAssetbundleLoadPath() + assetBundleName);

        yield return www;

        if(string.IsNullOrEmpty(www.error)){
            LoadedAssets.Add(assetBundleName, www.assetBundle);
            onFinish();
        }else{
            Debug.Log(assetBundleName);
            Debug.LogError(www.error);
        }
    }

    private static string GetAssetbundleLoadPath() {
        string path = "";
        switch (Application.platform) {
            case RuntimePlatform.Android:
                path = "file://" + Application.persistentDataPath +"/";
                break;
            case RuntimePlatform.WindowsPlayer:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerARM:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerX86:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerX64:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WindowsEditor:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.OSXPlayer:
                path = Application.dataPath + "/StreamingAssets/TWRPackages/";
                break;
            case RuntimePlatform.OSXEditor:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
        }

        return path;
    }

    private static string GetStreamingAssetsPath() {
        string path = "";
        switch (Application.platform) {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/TWRPackages/";
                break;
            case RuntimePlatform.WindowsPlayer:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerARM:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerX86:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WSAPlayerX64:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.WindowsEditor:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
            case RuntimePlatform.OSXPlayer:
                path = Application.dataPath + "/StreamingAssets/TWRPackages/";
                break;
            case RuntimePlatform.OSXEditor:
                path = "file://" + Application.streamingAssetsPath + "/TWRPackages/";
                break;
        }

        return path;
    }

    private static string GetUncompressAssetsPath() {
        string path = "";
        switch (Application.platform) {
            case RuntimePlatform.Android:
                path = Application.persistentDataPath + "/";
                break;
            case RuntimePlatform.WindowsEditor:
                path = Application.persistentDataPath + "/";
                break;
        }
        return path;
    }
}