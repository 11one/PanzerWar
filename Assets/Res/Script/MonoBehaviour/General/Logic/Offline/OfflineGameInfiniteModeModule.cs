using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineGameInfiniteModeModule : MonoBehaviour {
    private RespawnPointModule respawnPointModule;

    private List<BaseInitSystem> teamA = new List<BaseInitSystem>();
    private List<BaseInitSystem> teamB = new List<BaseInitSystem>();

    public void Init(OfflineGameProperty gameProperty) {
        GameDataManager.OfflineMode = true;
        GameEventManager.ResetActions();

        if (PoolManager._Instance == null) {
            DontDestroyOnLoad(Instantiate(Resources.Load("PoolManager")));
        }

        respawnPointModule = new RespawnPointModule();
        respawnPointModule.CorountineRunObject = this;

        StartCoroutine(AssetBundleManager.RequestScene(true, true, gameProperty.gameMapEnum.ToString(), null, (onFinish) => {

            new ClientBuildingManagerModule();

            BattleMainUIModule.Init();



            List<string> vehicleList = GameYear.GetVehicleListFromYear(gameProperty.gameYearEnum);

            GameEventManager.onNewVehicleSpawned = (newVehicle) => {
                if (newVehicle.ownerTeam == PunTeams.Team.red) {
                    teamA.Add(newVehicle);
                }
                else{
                    teamB.Add(newVehicle);
                }

                GameLogic(gameProperty, vehicleList);
  
            };

            GameEventManager.onNewVehicleDestroyed = (destroyedVehicle) => {
                if (destroyedVehicle.ownerTeam == PunTeams.Team.red) {
                    teamA.Remove(destroyedVehicle);
                }
                else {
                    teamB.Remove(destroyedVehicle);
                }

                GameLogic(gameProperty, vehicleList);

            };

            GameEventManager.onPlayerVehicleDestroyed = () => {
                StartCoroutine(BattleMainUIModule._Instance.ShowDeadCountDown(5,()=>{
                    BattleMainUIModule._Instance.onToggleSelectVehicleUIObject(true);
                }));
            };

            BattleMainUIModule._Instance.onUpdateVehicleList(
                vehicleList
            );

            BattleMainUIModule._Instance.onVehicleSelected = (selectedPlayerVehicle,bulletList) => {
                CreatePlayer(selectedPlayerVehicle,bulletList);
                BattleMainUIModule._Instance.onToggleSelectVehicleUIObject(false);
            };



            //CreateBot();
        }));
    }

    private void GameLogic(OfflineGameProperty gameProperty,List<string> vehicleList){
        if (teamA.Count < gameProperty.TeamANumber) {
            CreateBot(RandomVehicleFromList(vehicleList), PunTeams.Team.red);
        }

        if (teamB.Count < gameProperty.TeamBNumber) {
            CreateBot(RandomVehicleFromList(vehicleList), PunTeams.Team.blue);
        }
    }

    private void CreatePlayer(string _vehicle,int[] _bulletList){
        GameObject[] TeamAStartPoints = GameObject.FindGameObjectsWithTag("TeamAStartPoint");
        Vector3 airplaneOffsetHeight;
        Transform StartPoint = respawnPointModule.RandomStartPoint(TeamAStartPoints, false, out airplaneOffsetHeight);

        TankInitSystem vehicle = new GameObject("Vehicle", typeof(TankInitSystem)).GetComponent<TankInitSystem>();
        vehicle.VehicleName = _vehicle;
        vehicle._InstanceNetType = InstanceNetType.GameNetWorkOffline;
        vehicle.ownerTeam = PunTeams.Team.red;
        vehicle.BulletCountList = _bulletList;


        vehicle.InitTankInitSystem();

        vehicle.transform.position = StartPoint.position;
        vehicle.transform.eulerAngles = StartPoint.eulerAngles;
    }

    private string RandomVehicleFromList(List<string> _vehicleList){
        return _vehicleList[Random.Range(0, _vehicleList.Count)];
    }

    private void CreateBot(string _vehicle,PunTeams.Team _botTeam) {
        GameObject[] TeamBStartPoints = GameObject.FindGameObjectsWithTag("TeamBStartPoint");
        Vector3 airplaneOffsetHeight;
        Transform StartPoint = respawnPointModule.RandomStartPoint(TeamBStartPoints, false, out airplaneOffsetHeight);

        TankInitSystem vehicle = new GameObject("Vehicle", typeof(TankInitSystem)).GetComponent<TankInitSystem>();
        vehicle.VehicleName = _vehicle;
        vehicle._InstanceNetType = InstanceNetType.GameNetworkBotOffline;
        vehicle.ownerTeam = _botTeam;

        vehicle.BulletCountList = new int[]{1000,1000,1000};

        vehicle.InitTankInitSystem();
        vehicle.transform.position = StartPoint.position;
        vehicle.transform.eulerAngles = StartPoint.eulerAngles;
    }
}
