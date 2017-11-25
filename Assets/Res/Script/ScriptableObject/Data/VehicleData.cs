using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultVehicleData", menuName = "ShanghaiWindy/Data/Vehicle", order = 1)]
public class VehicleData : ScriptableObject {
    [Header("运行数据")]
    public VehicleTextData vehicleTextData;

    [System.Serializable]
    public class ModelData {
        public GameObject MainModel;
        public GameObject LOD;
        public VehicleHitBox HitBox;
    }
    [Header("模型数据")]
    public ModelData modelData;

    [System.Serializable]
    public class CacheData {
        public VehicleObjectTransformData FFPoint, EffectStart, EngineSmoke, EngineSound, MainCameraFollowTarget, MainCameraGunner, FireForceFeedbackPoint, CenterOfGravity,MachineGunFFPoint;
        public Vector3 StartPoint = new Vector3();
    }
    [Header("Cache（自动生成）")]
    public CacheData cacheData;
}

[System.Serializable]
public class VehicleObjectTransformData {
    public Vector3 localPosition;
    public Vector3 localEulerAngle;
}