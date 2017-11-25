using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIModule : MonoBehaviour {
    public SettingData settingData;
    //Graphic
    public Toggle DynamicShadowToggle;

    //Camera
    public Toggle SaveZoomToggle;

    public InputField CommonCameraSensitiveInputField, ObserveCameraSensitveInputField;

    //Controller
    public Toggle MobileMonitorToggle;

    public Toggle CameraJoystickToggle;

    public Toggle AutoDriveToggle;

    public Toggle AutoDriveGroundToggle;

    public Slider MainSoundSlider;

    public Slider InterfaceSoundSlider;

    public Slider FireSoundSlider;

    public Slider EngineSoundSlider;

    public Slider BackGroundSoundSlider;


    public void Init() {
#if UNITY_EDITOR
        settingData = Instantiate(settingData);
#endif
        settingData.Load();

        settingData.SetSound();

        //DynamicShadowToggle.isOn = settingData.useDynamicShadow;

        SaveZoomToggle.isOn = settingData.isSaveZoom;

        CommonCameraSensitiveInputField.text = settingData.commonCameraSensitive.ToString();

        ObserveCameraSensitveInputField.text = settingData.observeCameraSensitve.ToString();

        MobileMonitorToggle.isOn = settingData.useMobileMonitor;

        CameraJoystickToggle.isOn = settingData.useCameraJoystick;

        AutoDriveToggle.isOn = settingData.useAutoDrive;

        AutoDriveGroundToggle.isOn = settingData.useAutoDriveGround;

        cInput.MobileEnableMonitor = settingData.useMobileMonitor;

        MainSoundSlider.value = settingData.MainSound;

        InterfaceSoundSlider.value = settingData.InterfaceSound;

        FireSoundSlider.value = settingData.FireSound;

        EngineSoundSlider.value = settingData.EngineSound;

        BackGroundSoundSlider.value = settingData.BackGroundSound;

        //DynamicShadowToggle.onValueChanged.AddListener((flag) => {
        //    settingData.useDynamicShadow = flag;
        //    settingData.Save();
        //});

        SaveZoomToggle.onValueChanged.AddListener((flag) => {
            settingData.isSaveZoom = flag;
            settingData.Save();
        });

        CommonCameraSensitiveInputField.onValueChanged.AddListener((val) => {
            settingData.commonCameraSensitive = int.Parse(val);
            settingData.Save();
        }
        );

        ObserveCameraSensitveInputField.onValueChanged.AddListener((val) => {
            settingData.observeCameraSensitve = int.Parse(val);
            settingData.Save();
        }
        );
        
        MobileMonitorToggle.onValueChanged.AddListener((flag)=>{
            settingData.useMobileMonitor = flag;
            settingData.Save();
        });

        CameraJoystickToggle.onValueChanged.AddListener((flag) => {
            settingData.useCameraJoystick = flag;
            settingData.Save();
        });

        AutoDriveToggle.onValueChanged.AddListener((flag) => {
            settingData.useAutoDrive = flag;
            settingData.Save();
        });

        AutoDriveGroundToggle.onValueChanged.AddListener((flag) => {
            settingData.useAutoDriveGround = flag;
            settingData.Save();
        });

        MainSoundSlider.onValueChanged.AddListener(val=>{
            settingData.MainSound = val;
            settingData.Save();
            settingData.SetSound();
        });
        InterfaceSoundSlider.onValueChanged.AddListener(val => {
            settingData.InterfaceSound = val;
            settingData.Save();
            settingData.SetSound();
        });
        FireSoundSlider.onValueChanged.AddListener(val => {
            settingData.FireSound = val;
            settingData.Save();
            settingData.SetSound();
        });
        EngineSoundSlider.onValueChanged.AddListener(val => {
            settingData.EngineSound = val;
            settingData.Save();
            settingData.SetSound();
        });
        BackGroundSoundSlider.onValueChanged.AddListener(val => {
            settingData.BackGroundSound = val;
            settingData.Save();
            settingData.SetSound();
        });
    }
   
}
