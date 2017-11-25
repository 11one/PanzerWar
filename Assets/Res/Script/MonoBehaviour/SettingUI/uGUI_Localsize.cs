using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MaterialUI;

public class uGUI_Localsize : MonoBehaviour {
    public string Key;
    public LocalizeComponentType ComponentType;
    public TabPage tabPage;

    public static string GetContent(string _Key) {
        if (uGUI_Localization.LanguageKey.ContainsKey(_Key)) {
            return uGUI_Localization.LanguageKey[_Key];
        }
        else {
            return _Key;
        }
    }

    public void Awake() {
        if (uGUI_Localization.Loaded) {
            LocalizeComponent();
        }
        else {
            uGUI_Localization.Instance.onLangLoaded += LocalizeComponent;
        }
    }
    public void LocalizeComponent() {
        switch (ComponentType) {
            case LocalizeComponentType.Text:
                GetComponent<Text>().text = GetContent(Key);
                //GetComponent<Text>().SetLayoutDirty();
                break;
            case LocalizeComponentType.MD_Input:
                GetComponent<MaterialInputField>().hintText = GetContent(Key);
                GetComponent<MaterialInputField>().OnDeselect(null);
                //GetComponent<MaterialInputField>().SetLayoutDirty();

                break;

            case LocalizeComponentType.MD_CheckBox:
                GetComponent<MaterialCheckbox>().toggleOnLabel = GetContent(Key);
                GetComponent<MaterialCheckbox>().toggleOffLabel = GetContent(Key);
                break;
            case LocalizeComponentType.MD_Switch:
                GetComponent<MaterialSwitch>().toggleOnLabel = GetContent(Key);
                GetComponent<MaterialSwitch>().toggleOffLabel = GetContent(Key);
                break;
            case LocalizeComponentType.MD_TabPage:
                tabPage.tabName = GetContent(Key);
                break;
            case LocalizeComponentType.MD_DropDown:
                GetComponent<MaterialDropdown>().buttonTextContent.text = GetContent(Key);
                break;
        }


    }
}
public enum LocalizeComponentType {
    Text,
    MD_Input,
    MD_CheckBox,
    MD_TabPage,
    MD_Switch,
    MD_DropDown
}