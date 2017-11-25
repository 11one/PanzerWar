using System.Collections;
using UnityEngine;

public class SettingManager : MonoBehaviour {
    public static GameObject settingGameObject;

    public static SettingData settingData;

    public static IEnumerator Init(){
        if (settingGameObject != null)
            yield break;
        
        settingGameObject = Instantiate(Resources.Load<GameObject>("UI/Setting"));

        DontDestroyOnLoad(settingGameObject);

        yield return new WaitForEndOfFrame();

        settingGameObject.GetComponent<SettingUIModule>().Init();

        settingData = settingGameObject.GetComponent<SettingUIModule>().settingData;

        Close();
    }

    public static void Open(){
        settingGameObject.SetActive(true);
    }
    public static void Close(){
        settingGameObject.SetActive(false);
    }

}
