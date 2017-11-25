using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultVehicleHitBox", menuName = "ShanghaiWindy/Data/VehicleHitBox", order = 1)]
public class VehicleHitBox : ScriptableObject {
    [Header("参考模型")]
    public GameObject ReferModel;
    [Header("外部装甲模型")]
    public GameObject ExternalArmorModel;
    [Header("伤害模型预制体（自动生成）")]
    public GameObject HitBoxPrefab;
}
