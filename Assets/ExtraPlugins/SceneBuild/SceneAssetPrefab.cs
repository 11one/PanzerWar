using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


namespace ShanghaiWindy.Client {
    public class SceneAssetPrefab :MonoBehaviour {
		public int SerializedID;

        public string assetBundleName, assetBundleVariant, assetName;
        [System.NonSerialized]
        public bool LoadingDone = false, inLoading = false;

        [System.Serializable]
        public class MeshParameter {
            public Vector4 LightingMapTilingOffset;
            public int LightingMapIndex = -1;
            public bool RendererPathinChild = false;
            public string RendererPath = "";
            public ReflectionProbeUsage reflectionusage;
        }
        public MeshParameter[] meshParameters;


        public bool HasParticleSystem = false;
        public bool UseStandardShader = false;


        public System.Action onFinish;

		private void Start(){
			
			#if ClientCode
			assetBundleVariant = "client"+assetBundleVariant;
			#else
			assetBundleVariant = "master"+assetBundleVariant;
			#endif
		}
        public void LoadAsset() {
            AssetRequestTask assetRequestTask = new AssetRequestTask() {
                onAssetLoaded = (myReturnValue) => {
                    if (myReturnValue == null) {
                        LoadingDone = true;
                        inLoading = false;

                        if (onFinish != null)
                            onFinish();
                        
                        return;
                    }
                    if (onFinish != null)
                        onFinish();
                    
                    GameObject Instance = Instantiate(myReturnValue) as GameObject;

                    Instance.transform.SetParent(transform.parent);

                    Instance.transform.position = transform.position;
                    Instance.transform.rotation = transform.rotation;
                    Instance.transform.localScale = transform.localScale;

                    //Instance.transform.localScale = new Vector3(1, 1, 1);
                    //Instance.transform.localEulerAngles = Vector3.zero;
                    //Instance.transform.localPosition = Vector3.zero;

                    //Instance.isStatic = gameObject.isStatic;
                    //Instance.tag = gameObject.tag;
                    //Instance.layer = gameObject.layer;

                    if (Instance.GetComponent<SceneObjectWithSerializedID>() != null) {
                        Instance.GetComponent<SceneObjectWithSerializedID>().SerializedID = SerializedID;
                    }

                    //if (HasParticleSystem) {
                    //    ParticleSystemRenderer[] particles = Instance.GetComponentsInChildren<ParticleSystemRenderer>();
                    //    foreach (ParticleSystemRenderer particle in particles) {
                    //        for (int i = 0; i < particle.sharedMaterials.Length; i++) {
                    //            particle.sharedMaterials[i].shader = Shader.Find(particle.sharedMaterials[i].shader.name);
                    //        }
                    //    }
                    //}
					#if UNITY_EDITOR && UNITY_ANDROID
					foreach(MeshRenderer meshRenderer in Instance.GetComponentsInChildren<MeshRenderer>()){
						meshRenderer.sharedMaterial.shader = Shader.Find("Standard");
					}
					#endif

                    if (meshParameters.Length > 0) {
                        foreach (MeshParameter meshParameter in meshParameters) {
                            MeshRenderer TargetRenderer = null;
                            if (meshParameter.RendererPathinChild) {
                                if (Instance.transform.Find(meshParameter.RendererPath))
                                    TargetRenderer = Instance.transform.Find(meshParameter.RendererPath).GetComponent<MeshRenderer>();
                            }
                            else {
                                if (Instance.transform)
                                    TargetRenderer = Instance.transform.GetComponent<MeshRenderer>();
                            }


                            if (TargetRenderer) {
                                TargetRenderer.lightmapIndex = meshParameter.LightingMapIndex;
                                TargetRenderer.lightmapScaleOffset = meshParameter.LightingMapTilingOffset;
                                TargetRenderer.reflectionProbeUsage = meshParameter.reflectionusage;
								//TargetRenderer.sharedMaterial.shader = Shader.Find(TargetRenderer.sharedMaterial.)
                            }
                        }
                    }

                    LoadingDone = true;
                    inLoading = false;
                }
            };
            assetRequestTask.SetAssetBundleName(assetBundleName, assetBundleVariant);

            AssetBundleManager.LoadAssetFromAssetBundle(assetRequestTask);
        }
    }
}
