using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineGameManager : MonoBehaviour {
    public System.Action<OfflineGameProperty> onOfflineGameRequested;
    public System.Action onRaceGamerequested;

    public static OfflineGameManager _Instance;

    private void Awake()
    {
        if(_Instance == null)
            _Instance = this;
    }

    private void Start()
    {
        onOfflineGameRequested = HandleOfflineGameRequested;
        onRaceGamerequested = HandleRaceGameRequested;
        uGUI_QualitySetting.Init();
    }

    void HandleOfflineGameRequested(OfflineGameProperty gameProperty) {
        Debug.Log(gameProperty);
        if(gameProperty.gameModeEnum == GameModeEnum.InfiniteMode){
            OfflineGameInfiniteModeModule inifiniteModeModule = new GameObject("OfflineGameInfiniteModeModule", typeof(OfflineGameInfiniteModeModule)).GetComponent<OfflineGameInfiniteModeModule>();
            DontDestroyOnLoad(inifiniteModeModule.gameObject);
            inifiniteModeModule.Init(gameProperty);
        }
    }

    void HandleRaceGameRequested() {
        OfflineGameRaceModeModule raceModule = new GameObject("RaceModule", typeof(OfflineGameRaceModeModule)).GetComponent<OfflineGameRaceModeModule>();
        DontDestroyOnLoad(raceModule.gameObject);
        raceModule.Init();
    }
}
