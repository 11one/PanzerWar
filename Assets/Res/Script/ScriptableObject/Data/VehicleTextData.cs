using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultVehicleTextData", menuName = "ShanghaiWindy/Data/VehicleText", order = 1)]
public class VehicleTextData : ScriptableObject {
	public string AssetName = "VehicleNameTextData";

	[Header ("坦克控制器属性")]
	public PlayerTankControllerParameter PTCParameter;
	[Header ("坦克发射器属性")]
	public TankFireParameter TFParameter;
	[Header ("多炮塔")]
	public bool ExtraTF = false;
	public MultiTurrets[] multiTurrets;
	[Header ("坦克炮塔属性")]
	public MouseTurretParameter MTParameter;
	[Header ("玩家坦克属性")]
	public PlayerStateParameter PSParameter;
}
