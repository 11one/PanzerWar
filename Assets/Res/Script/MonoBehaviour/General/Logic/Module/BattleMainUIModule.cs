using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMainUIModule : MonoBehaviour {
    public System.Action<List<string>> onUpdateVehicleList;

    public System.Action<string, int[]> onVehicleSelected;

    public System.Action<bool> onToggleSelectVehicleUIObject;

    public static BattleMainUIModule _Instance;

    private BattleMainUIReferenceModule uiReference;

    private GameObject mainUIObject;

    private string currentSelectedVehicle;

    public static void Init() {
        if (_Instance == null) {
            _Instance = new GameObject("BattleMainUIModule", typeof(BattleMainUIModule)).GetComponent<BattleMainUIModule>();
            _Instance.InstanceInit();
        }
    }

    private void InstanceInit() {
        mainUIObject = Instantiate<GameObject>(Resources.Load<GameObject>("UI/BattleUIModule/Main"));

        uiReference = mainUIObject.GetComponent<BattleMainUIReferenceModule>();

        onToggleSelectVehicleUIObject = (isActive) => {
            uiReference.SelectVehiclePanel.SetActive(isActive);
        };

        //炮弹增加逻辑
        for (int i = 0; i < uiReference.AmmunitionAdders.Length; i++) {
            int current = i;

            uiReference.AmmunitionChangeFielders[current].text = PlayerPrefs.GetInt(string.Format("{0}{1}", currentSelectedVehicle, current)).ToString();

            uiReference.AmmunitionAdders[current].onClick.AddListener(() => {
                int ammoCount = int.Parse(uiReference.AmmunitionChangeFielders[current].text);
                ammoCount += 1;
                uiReference.AmmunitionChangeFielders[current].text = ammoCount.ToString();
            });

            uiReference.AmmunitionMinusers[current].onClick.AddListener(() => {
                int ammoCount = int.Parse(uiReference.AmmunitionChangeFielders[current].text);
                ammoCount -= 1;
                uiReference.AmmunitionChangeFielders[current].text = ammoCount.ToString();
            });

            uiReference.AmmunitionChangeFielders[current].onValueChanged.AddListener(
                (changedValue) => {
                    PlayerPrefs.SetInt(string.Format("{0}{1}", currentSelectedVehicle, current), int.Parse(changedValue));
                }
            );
        }
        //列表更新逻辑
        onUpdateVehicleList = (vehicleList) => {
            for (int i = 0; i < vehicleList.Count; i++) {
                int current = i;

                //选择默认车辆
                if (current == 0) {
                    OnSelectNewVehicle(vehicleList[0]);
                }

                uiReference.vehicleSelectDropDown.AddData(
                    new MaterialUI.OptionData() {
                        text = uGUI_Localsize.GetContent(vehicleList[i]),
                        onOptionSelected = () => {
                            OnSelectNewVehicle(vehicleList[current]);
                        }
                    }
                );
            }

        };

        uiReference.JoinBattleButton.onClick.AddListener(() => {

            onVehicleSelected(
                currentSelectedVehicle,
                new int[]{
                    int.Parse(uiReference.AmmunitionChangeFielders[0].text),
                    int.Parse(uiReference.AmmunitionChangeFielders[1].text),
                    int.Parse(uiReference.AmmunitionChangeFielders[2].text)
                }
            );

        });
    }

    private void OnSelectNewVehicle(string _vehicle) {
        currentSelectedVehicle = _vehicle;

        StartCoroutine(LoadImage(_vehicle));

        UpdateAmmunitionList(_vehicle);

        AssetRequestTask vehicleDataRequestTask = new AssetRequestTask() {
            onAssetLoaded = (Object data) => {
                VehicleTextData vehicleTextData = (VehicleTextData)data;
                BulletScript bulletData = vehicleTextData.TFParameter.bulletType.GetComponent<BulletScript>();

                string formater = "{0}:{1} {2}:{3}(m/s) {4}:{5}(hp) {6}:{7}(mm)";


                uiReference.APText.text = string.Format(
                    formater,
                    uGUI_Localsize.GetContent("AmmoType"), "AP",
                    uGUI_Localsize.GetContent("AmmoSpeed"), bulletData.Speed,
                    uGUI_Localsize.GetContent("AmmoDamage"), bulletData.APDamage,
                    uGUI_Localsize.GetContent("AmmoPenerate"), bulletData.APPenetration
                );

                uiReference.HEText.text = string.Format(
                    formater,
                    uGUI_Localsize.GetContent("AmmoType"), "HE",
                    uGUI_Localsize.GetContent("AmmoSpeed"), bulletData.Speed * 0.75f,
                    uGUI_Localsize.GetContent("AmmoDamage"), bulletData.HeDamage,
                    uGUI_Localsize.GetContent("AmmoPenerate"), bulletData.HePenetration
                );

                uiReference.APCRText.text = string.Format(
                    formater,
                    uGUI_Localsize.GetContent("AmmoType"), "APCR",
                    uGUI_Localsize.GetContent("AmmoSpeed"), bulletData.Speed * 1.25f,
                    uGUI_Localsize.GetContent("AmmoDamage"), bulletData.APDamage * 0.75f,
                   uGUI_Localsize.GetContent("AmmoPenerate"), bulletData.ApcrPenration
                );

            }
        };

        vehicleDataRequestTask.SetAssetBundleName(_vehicle, "data");

        AssetBundleManager.LoadAssetFromAssetBundle(vehicleDataRequestTask);

    }

    private void UpdateAmmunitionList(string _vehicle) {
        for (int i = 0; i < uiReference.AmmunitionAdders.Length; i++) {
            int current = i;

            uiReference.AmmunitionChangeFielders[current].enabled = false;
            uiReference.AmmunitionChangeFielders[current].text = PlayerPrefs.GetInt(string.Format("{0}{1}", _vehicle, current)).ToString();
            uiReference.AmmunitionChangeFielders[current].enabled = true;
        }
    }

    public IEnumerator ShowDeadCountDown(float time,System.Action onFinish){
        uiReference.RespanPanel.SetActive(true);

        while(time>0){
            time -= 1;
            uiReference.RespawnText.text = string.Format(uGUI_Localsize.GetContent("RespawnCountDown"), time);
            yield return new WaitForSeconds(1);
        }

        uiReference.RespanPanel.SetActive(false);

        onFinish();

        yield break;
    }
    //异步加载载具描述图片
    private IEnumerator LoadImage(string _vehicle) {
        ResourceRequest resourceRequest = Resources.LoadAsync<Sprite>("UI/VehicleImage/" + _vehicle);
        yield return resourceRequest;
        uiReference.VehicleThumbnail.sprite = (Sprite)resourceRequest.asset;
    }

}
