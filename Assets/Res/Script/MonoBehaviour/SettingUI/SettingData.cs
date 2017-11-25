using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultSettingData", menuName = "ShanghaiWindy/Data/Setting", order = 1)]
public class SettingData : ScriptableObject {
    public bool useDynamicShadow = false;

    public float commonCameraSensitive =1,observeCameraSensitve=1;

    public bool isSaveZoom = true;

    public bool useMobileMonitor = false;

    public bool useCameraJoystick = false;

    public bool useAutoDrive = false;

    public bool useAutoDriveGround = false;

    public float MainSound, InterfaceSound, FireSound, EngineSound, BackGroundSound;

	public void Save()
	{
		PlayerPrefs.SetString("SettingData", JsonUtility.ToJson(this));
	}

	public void Load()
	{
        if(PlayerPrefs.HasKey("SettingData"))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("SettingData"),this);
	}

    public void SetSound(){
        StaticResourcesReferences.Instance.GlobalAudioMixer.SetFloat("Main", MainSound);
        StaticResourcesReferences.Instance.GlobalAudioMixer.SetFloat("Interface", InterfaceSound);
        StaticResourcesReferences.Instance.GlobalAudioMixer.SetFloat("FireSound", FireSound);
        StaticResourcesReferences.Instance.GlobalAudioMixer.SetFloat("EngineSound", EngineSound);
        StaticResourcesReferences.Instance.GlobalAudioMixer.SetFloat("BGM", BackGroundSound);
    }
}
