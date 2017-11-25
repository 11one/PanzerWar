using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialUI;

public class OfflineGameManagerUIModule : MonoBehaviour {
    public Button ShowGamePropertySetUIPanel_Button;

    public Button CreateGamePropertyPreset_Button;

    public Button CloseGamePropertySetUIPanel_Button;

    public GameObject GamePropertySetUIPanel;

    public InputField GamePropertyPresetName_InputField;

    public MaterialDropdown BattleYear_Dropdown;

    public MaterialDropdown BattleMap_Dropdown;

    public MaterialDropdown BattleTeamANumber_DropDown, BattleTeamBNumber_DropDown;

    public Button OpenGarage_Button;

    public Button OpenRaceMode_Button;

    private GameMapEnum[] availableGameMapList = new GameMapEnum[]{
        GameMapEnum.Desert,
        GameMapEnum.Rock,
        GameMapEnum.Village
    };

    private int currentGameMap = 0;

    private GameYearEnum[] availableGameYearList = new GameYearEnum[]{
        GameYearEnum.WW2Early,
        GameYearEnum.WW2Late
    };

    private int currentGameYear = 0;

    private OfflineGamePropertyAssemble offlineGamePropertyAssemble = new OfflineGamePropertyAssemble();

    public GameObject roomLabelTemplate;

    private List<OfflineGameProperty> createdInstanceProperty = new List<OfflineGameProperty>();

    private void Start() {

        if (PlayerPrefs.HasKey("OfflineGamePropertyAssemble"))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("OfflineGamePropertyAssemble"), offlineGamePropertyAssemble);
        else {
            offlineGamePropertyAssemble.Assemble.Add(
                new OfflineGameProperty() {
                    gameYearEnum = GameYearEnum.WW2Late,
                    presetName = "Default-WW2Late-1V1",
                    TeamANumber = 1,
                    TeamBNumber = 1
                }
            );

            offlineGamePropertyAssemble.Assemble.Add(
                new OfflineGameProperty() {
                    gameYearEnum = GameYearEnum.WW2Late,
                    presetName = "Default-WW2Late-2V2",
                    TeamANumber = 2,
                    TeamBNumber = 2
                }
            );

            offlineGamePropertyAssemble.Assemble.Add(
                new OfflineGameProperty() {
                    gameYearEnum = GameYearEnum.WW2Early,
                    presetName = "Default-WW2Early-1V1",
                    TeamANumber = 1,
                    TeamBNumber = 1
                }
            );
        }


        //设置界面
        ShowGamePropertySetUIPanel_Button.onClick.AddListener(() => {
            GamePropertySetUIPanel.SetActive(true);
        });

        CloseGamePropertySetUIPanel_Button.onClick.AddListener(() => {
            GamePropertySetUIPanel.SetActive(false);
        });

        CreateGamePropertyPreset_Button.onClick.AddListener(() => {
            string presetName = GamePropertyPresetName_InputField.text;

            OfflineGameProperty currentGameProperty = new OfflineGameProperty();

            currentGameProperty.presetName = presetName;

            currentGameProperty.gameMapEnum = availableGameMapList[currentGameMap];

            currentGameProperty.gameYearEnum = availableGameYearList[currentGameYear];

            currentGameProperty.TeamANumber = int.Parse(BattleTeamANumber_DropDown.buttonTextContent.text);

            currentGameProperty.TeamBNumber = int.Parse(BattleTeamBNumber_DropDown.buttonTextContent.text);

            offlineGamePropertyAssemble.Assemble.Add(currentGameProperty);

            PlayerPrefs.SetString("OfflineGamePropertyAssemble", JsonUtility.ToJson(offlineGamePropertyAssemble));

            Debug.Log(JsonUtility.ToJson(offlineGamePropertyAssemble));

            foreach (OfflineGameProperty gameProperty in offlineGamePropertyAssemble.Assemble) {
                CreateTemplateInstance(gameProperty);
            }

            GamePropertySetUIPanel.SetActive(false);
        });

        for (int i = 0; i < availableGameMapList.Length; i++) {
            BattleMap_Dropdown.AddData(new MaterialUI.OptionData() {
                text = uGUI_Localsize.GetContent(availableGameMapList[i].ToString())
            });
        }

        BattleMap_Dropdown.onItemSelected.AddListener((changedValue) => {
            currentGameMap = changedValue;
        });

        for (int i = 0; i < availableGameYearList.Length; i++) {
            BattleYear_Dropdown.AddData(new MaterialUI.OptionData() {
                text = uGUI_Localsize.GetContent(availableGameYearList[i].ToString())
            });
        }

        BattleYear_Dropdown.onItemSelected.AddListener((valueChanged) => {
            currentGameYear = valueChanged;
        });

        //设置预设显示
        roomLabelTemplate.SetActive(false);

        foreach (OfflineGameProperty gameProperty in offlineGamePropertyAssemble.Assemble) {
            CreateTemplateInstance(gameProperty);
        }
        //Garage
        OpenGarage_Button.onClick.AddListener(() => {
            //ClientNetwork.Instance.StartCoroutine(
            //    AssetBundleManager.RequestScene(true, true, "garage", (myReturnValue) => {
            //        Hashtable vehicleList = new Hashtable();

            //        AccountDataManager.InstanceAllHastables();

            //        foreach (string Vehicle in AccountDataManager.ExcelVehicleData.Keys) {
            //            vehicleList.Add(Vehicle, new Crew.CrewInfo());
            //        }

            //        GarageTankManger.Instance.UpdataTankList(vehicleList, true);

            //        AccountManager.Instance.DataUpdata();
            //    }, null)
            //);
        });

        OpenRaceMode_Button.onClick.AddListener(()=>{
            OfflineGameManager._Instance.onRaceGamerequested();
        });
    }

    private void CreateTemplateInstance(OfflineGameProperty currentGameProperty) {
        if (createdInstanceProperty.Contains(currentGameProperty))
            return;

        createdInstanceProperty.Add(currentGameProperty);

        GameObject templateInstance = Instantiate(roomLabelTemplate);

        templateInstance.SetActive(true);
        templateInstance.transform.SetParent(roomLabelTemplate.transform.parent);
        templateInstance.transform.localScale = new Vector3(1, 1, 1);

        templateInstance.transform.Find("PanelLayer/RoomName").GetComponent<Text>().text = currentGameProperty.presetName;
        templateInstance.transform.Find("PanelLayer/Map").GetComponent<Text>().text = currentGameProperty.gameMapEnum.ToString();
        templateInstance.transform.Find("PanelLayer/Year").GetComponent<Text>().text = currentGameProperty.gameYearEnum.ToString();

        templateInstance.transform.Find("PanelLayer/Button").GetComponent<Button>().onClick.AddListener(() => {
            OfflineGameManager._Instance.onOfflineGameRequested(currentGameProperty);
        });

        templateInstance.transform.Find("PanelLayer/Delete").GetComponent<Button>().onClick.AddListener(() => {
            offlineGamePropertyAssemble.Assemble.Remove(currentGameProperty);
            PlayerPrefs.SetString("OfflineGamePropertyAssemble", JsonUtility.ToJson(offlineGamePropertyAssemble.Assemble));

            Destroy(templateInstance);
        });
    }
}
