using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundModule : MonoBehaviour {
	public TankInitSystem tankInitSystem;

	public VehicleEngineSoundData engineData;

	public float AccelG;

	public bool isDebug =false;

	private AudioSource EngineStart;

	private AudioSource EngineIdle;

	private AudioSource EngineRun;

	private AudioSource EngineStop;

	private bool isStartEngineFinished = false;


	void Start(){
		if (isDebug) {
			Init ();
		}
	}
	public void Init(Transform parent =null){
		EngineStart = CreateAudioSource (engineData.EngineStart,parent);
		EngineIdle = CreateAudioSource (engineData.EngineIdle,parent);
		EngineRun = CreateAudioSource (engineData.EngineRunning,parent);
		//EngineStop = CreateAudioSource (engineData.EngineStop,parent);

		StartCoroutine (PlayEngineStart (EngineStart,() => {
			isStartEngineFinished = true;
			EngineIdle.Play();
			EngineIdle.loop =true;
			EngineIdle.volume =0;
			EngineRun.Play();
			EngineRun.loop =true;
			EngineRun.volume =0;
		}));
	}
	private void Update(){
		if (isStartEngineFinished) {
			if (AccelG <= 0.2f) {
				SetEngineSoundToIdle ();
			}
			if (AccelG >0.2f) {
				SetEngineSoundToRun ();
			}
		}
	}
	private void SetEngineSoundToIdle(){
		EngineIdle.volume = Mathf.Lerp(EngineIdle.volume,1,Time.deltaTime);
		EngineRun.volume = Mathf.Lerp(EngineRun.volume,0,Time.deltaTime);
	}
	private void SetEngineSoundToRun(){
		EngineIdle.volume = 0;
		EngineRun.volume =Mathf.Lerp(EngineRun.volume,AccelG,Time.deltaTime) ;
		EngineRun.volume = Mathf.Clamp(EngineRun.volume,0.2f,1);
	}

	private IEnumerator PlayEngineStart(AudioSource engineStartSource,System.Action onFinish){
		yield return new WaitForSeconds (2);
		engineStartSource.Play ();
		yield return new WaitForSeconds (engineStartSource.clip.length);
		onFinish ();
	}

	private AudioSource CreateAudioSource(AudioClip audioClip,Transform parent =null){
		AudioSource engineAudioSource = new GameObject (
			audioClip.name
			,typeof(AudioSource)
		).GetComponent<AudioSource>();

		if (!isDebug) {
			tankInitSystem.dontDestroyManager.Add (engineAudioSource.gameObject);
		}

		engineAudioSource.playOnAwake = false;
		engineAudioSource.clip = audioClip;
		engineAudioSource.spatialBlend = 1;
		//engineAudioSource.outputAudioMixerGroup = StaticResourcesReferences.Instance.GlobalAudioMixer.FindMatchingGroups ("EngineSound") [0];

		if (parent != null) {
			engineAudioSource.transform.SetParent (parent);
			engineAudioSource.transform.localPosition = Vector3.zero;
			engineAudioSource.transform.localEulerAngles = Vector3.zero;
		}
		return engineAudioSource;
	}
}
