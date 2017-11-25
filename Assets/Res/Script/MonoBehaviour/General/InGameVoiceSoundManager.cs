using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameVoiceSoundManager : MonoBehaviour {
	public enum VoiceType{
		Armor_Ricochet,Enemy_Killed,Start_Battle,Vehicle_Destroyed,Armor_Pierced,Armor_Not_Pierced
	}
	public enum Language {
		CN,
		JP,
		GER,
		RU,
		EN
	}
	public Language language;

	[System.Serializable]
	public class InGameVoiceProperty{
		public Language language = Language.JP;
		public string[] Armor_Ricochet,Enemy_Killed,Start_Battle,Vehicle_Destroyed,Armor_Pierced,Armor_Not_Pierced;
	}
	public  InGameVoiceProperty[] InGameVoiceProperties;

	string[] Armor_Ricochet,Enemy_Killed,Start_Battle,Vehicle_Destroyed,Armor_Pierced,Armor_Not_Pierced;

	public static InGameVoiceSoundManager Self;
	List<string> InitedLanguage = new List<string>();
	void Start(){
		Self = this;
	}
	public static Language InGameVoiceLanguage(TankFire.NationType nationType){
		Language mylanguage;
		if (nationType == TankFire.NationType.CN) {
			mylanguage = Language.CN;
		} else if (nationType == TankFire.NationType.JP) {
			mylanguage = Language.JP;
		} else if (nationType == TankFire.NationType.GER) {
			mylanguage = Language.GER;
		} else if (nationType == TankFire.NationType.RU) {
			mylanguage = Language.RU;
		} else if (nationType == TankFire.NationType.UK) {
			mylanguage = Language.EN;
		}
		else {
			mylanguage = Language.EN;
		}
		return mylanguage;
	}
	public IEnumerator Init(string VehicleName,System.Action<bool> onFinish){
        language = Language.EN; //TODO :
		if (InitedLanguage.Contains (language.ToString ())) {
			onFinish (true);
			yield break;
		}
		
		InitedLanguage.Add (language.ToString());
		foreach (InGameVoiceProperty temp in InGameVoiceProperties) {
			if(temp.language == language){
				int Tasks = 6;
				StartCoroutine(InitAudioSource(temp.Armor_Pierced,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));
				StartCoroutine(InitAudioSource(temp.Enemy_Killed,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));
				StartCoroutine(InitAudioSource(temp.Start_Battle,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));
				StartCoroutine(InitAudioSource(temp.Vehicle_Destroyed,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));
				StartCoroutine(InitAudioSource(temp.Armor_Ricochet,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));
				StartCoroutine(InitAudioSource(temp.Armor_Not_Pierced,language.ToString(),(ReturnValue) =>{
					Tasks-=1;
				}));

				while (Tasks != 0) {
					yield return new WaitForEndOfFrame ();
				}
				Armor_Ricochet = temp.Armor_Ricochet;
				Enemy_Killed = temp.Enemy_Killed;
				Start_Battle = temp.Start_Battle;
				Vehicle_Destroyed = temp.Vehicle_Destroyed;
				Armor_Pierced = temp.Armor_Pierced;
				Armor_Not_Pierced = temp.Armor_Not_Pierced;

			}
		}
		yield return new WaitForSeconds (1);
		onFinish (true);
	}
	public static float LastPlayVoiceTime = 0;
	public static void PlayVoice(VoiceType voiceType,Language language){
		if (Time.time - LastPlayVoiceTime < 1) {
			return;
		}
		LastPlayVoiceTime = Time.time;

		switch (voiceType) {
			case VoiceType.Armor_Not_Pierced:
				PoolManager.CreateObject(RandomStringInList(Self.Armor_Not_Pierced)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
			case VoiceType.Armor_Pierced:
				PoolManager.CreateObject(RandomStringInList(Self.Armor_Pierced)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
			case VoiceType.Armor_Ricochet:
				PoolManager.CreateObject(RandomStringInList(Self.Armor_Ricochet)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
			case VoiceType.Enemy_Killed:
				PoolManager.CreateObject(RandomStringInList(Self.Enemy_Killed)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
			case VoiceType.Start_Battle:
				PoolManager.CreateObject(RandomStringInList(Self.Start_Battle)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
			case VoiceType.Vehicle_Destroyed:
				PoolManager.CreateObject(RandomStringInList(Self.Vehicle_Destroyed)+language.ToString(),Vector3.zero,Vector3.zero);
			break;
		}
	}
	public static string RandomStringInList(string[] stringList){
		if (stringList.Length != 0)
			return stringList [Random.Range (0, stringList.Length)];
		else 
			return "";
	}
	IEnumerator InitAudioSource(string[] AudioSorces,string Nation,System.Action<bool> onFinish){
		foreach (string s in AudioSorces) {
			GameObject NewSound = new GameObject(s);
			ResourceRequest resourceRequest = Resources.LoadAsync ("Audio/CrewVoice/" + Nation + "/" + s);
			yield return resourceRequest;
			NewSound.AddComponent<AudioSource>().clip = (AudioClip)resourceRequest.asset;
			PoolManager.UpdateParams(s+Nation,NewSound,2,4,false);
			Destroy (NewSound);
			onFinish (true);
		}
	}
}
