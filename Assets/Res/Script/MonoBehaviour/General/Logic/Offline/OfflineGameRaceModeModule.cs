using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialUI;

public class OfflineGameRaceModeModule : MonoBehaviour {
    public void Init(){
        if (PoolManager._Instance == null) {
            DontDestroyOnLoad(Instantiate(Resources.Load("PoolManager")));
        }
        GameDataManager.OfflineMode = true;

        StartCoroutine(AssetBundleManager.RequestScene(true, true, "Desert", null, (onFinish) => {
            new ClientBuildingManagerModule();


            TankInitSystem vehicle = new GameObject("Vehicle", typeof(TankInitSystem)).GetComponent<TankInitSystem>();

            vehicle.VehicleName = "T-44";

            vehicle._InstanceNetType = InstanceNetType.GameNetWorkOffline;

            vehicle.InitTankInitSystem();

            vehicle.transform.position = new Vector3(40.7f, 0.49f, 106.87f);

            vehicle.onVehicleLoaded = () => {
                RaceLogicModule race = gameObject.AddComponent<RaceLogicModule>();
                race.racer = vehicle.InstanceMesh.transform;
                race.StartRace(() => {
                    StartCoroutine(AssetBundleManager.RequestScene(true, false,"ClientOffline"));
                });
            };
        }));
    }
}
