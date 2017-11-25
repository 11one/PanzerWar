using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class OfflineGamePropertyAssemble{
	public List<OfflineGameProperty> Assemble = new List<OfflineGameProperty> ();
}

[System.Serializable]
public class OfflineGameProperty {
	public GameMapEnum gameMapEnum = GameMapEnum.Desert;
    public GameModeEnum gameModeEnum = GameModeEnum.InfiniteMode;
    public GameYearEnum gameYearEnum = GameYearEnum.WW2Early;

	public string presetName;
	public int TeamANumber,TeamBNumber;
}
