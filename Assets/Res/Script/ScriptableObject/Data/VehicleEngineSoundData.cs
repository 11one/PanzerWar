using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultVehicleEngineData", menuName = "ShanghaiWindy/Data/VehicleComponent/EngineSoundData", order = 1)]
public class VehicleEngineSoundData : ScriptableObject{
	public AudioClip EngineStart;
	public AudioClip EngineIdle;
	public AudioClip EngineRunning;
	public AudioClip EngineStop;
}
