using System.Collections.Generic;

public enum GameYearEnum {
    WW2Early,
    WW2Late,
}

public class GameYear {
    public static List<string> GetVehicleListFromYear(GameYearEnum currentYearEnum){
        List<string> WW2LateVehicleList = new List<string>() {
            "T-54","T-44","T-34-3","T-62A","Bat_Chatillon155_58","A7V_SturmpanzerWagen"
        };

        List<string> WW2EarlyVehicleList = new List<string>(){
            "T-26"
        };
        switch(currentYearEnum){
            case GameYearEnum.WW2Early:
                return WW2EarlyVehicleList;
                break;
            case GameYearEnum.WW2Late:
                return WW2LateVehicleList;
                break;
            default:
                return null;
                break;
        }
    }
}