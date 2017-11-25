using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour {
    public static System.Action<BaseInitSystem> onNewVehicleSpawned;
    public static System.Action<BaseInitSystem> onNewVehicleDestroyed;
    public static System.Action onPlayerVehicleDestroyed;

    public static void ResetActions(){
        onNewVehicleSpawned = null;
        onNewVehicleDestroyed = null;
        onPlayerVehicleDestroyed = null;
    }
}
