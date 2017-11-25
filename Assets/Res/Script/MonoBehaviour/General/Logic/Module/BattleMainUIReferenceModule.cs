using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialUI;

public class BattleMainUIReferenceModule : MonoBehaviour {
    public MaterialDropdown vehicleSelectDropDown;

    public Button[] AmmunitionAdders, AmmunitionMinusers;

    public InputField[] AmmunitionChangeFielders;

    public Image VehicleThumbnail;

    public Text VehicleNameText;

    public Text VehiclePRText;

    public Text APText, HEText, APCRText;

    public Button JoinBattleButton;

    public GameObject RespanPanel;

    public Text RespawnText;

    public GameObject SelectVehiclePanel;
}
