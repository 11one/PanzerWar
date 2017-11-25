using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour {
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        AssetBundleManager.Init(this);

        StartCoroutine(AssetBundleManager.GetAssetBundleInfos(
            ()=>{
                if(Application.platform == RuntimePlatform.Android){
                    if(PlayerPrefs.GetString("VersionCompressed")!=GameDataManager.Version){
                        StartCoroutine(AssetBundleManager.UncompressAssetBundles((isSucceeded) => {
                            if (isSucceeded) {
                                PlayerPrefs.SetString("VersionCompressed", GameDataManager.Version);
                            }
                        }));
                    }
                }

                StartCoroutine(AssetBundleManager.AssetBundleLoop());
            }
        ));
    }

}
