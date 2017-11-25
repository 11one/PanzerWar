using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Difficulty{
    Easy,
    Normal
}
public class PunTeams{
    public enum Team {
        red,
        blue,
        none
    }
}

public class GameDataManager: MonoBehaviour {
    public const string Version = "2017.1(Open Source)";
    public static bool OfflineMode = true;
    public static PunTeams.Team OfflinePlayerTeam = PunTeams.Team.red;

    public static Difficulty GetCurrentDifficulty(){
        return Difficulty.Normal;
    }
}
