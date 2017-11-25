using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamModule : MonoBehaviour {
	// 使用 UserID （账号）
	public List<string> PlayerTeamA;
	public List<string> PlayerTeamB;
	public List<string> BotTeamA;
	public List<string> BotTeamB;
	public List<string> AllPlayers;
	public TeamModule(){
		PlayerTeamA = new List<string> ();
		PlayerTeamB = new List<string> ();
		BotTeamA = new List<string> ();
		BotTeamB = new List<string> ();
		AllPlayers = new List<string> ();
	}

	#region 遭遇战匹配

	////遭遇战队伍匹配算法
	//public PunTeams.Team GetSkirmishPlayerTeam (PhotonPlayer newPlayer)
	//{
	//	//先使用简单的匹配算法 然后考虑掉线重新连接、 战车总RP 、个人效率
	//	PunTeams.Team newPlayerTeam = PunTeams.Team.none;

	//	if (PlayerTeamA.Count > PlayerTeamB.Count) {
	//		newPlayerTeam = PunTeams.Team.blue;
	//		BotTeamB.Add (newPlayer.UserId);
	//	} else {
	//		newPlayerTeam = PunTeams.Team.red;
	//		BotTeamA.Add (newPlayer.UserId);
	//	}
	//	AllPlayers.Add (newPlayer.UserId);

	//	return newPlayerTeam;
	//}
	////清除遭遇战下的匹配
	//public void RemoveSkirmishPLeftOnlinePlayer (PhotonPlayer otherPlayer)
	//{
	//	if (otherPlayer.GetTeam () == PunTeams.Team.red) {
	//		PlayerTeamA.Remove (otherPlayer.UserId);	
	//	} else if (otherPlayer.GetTeam () == PunTeams.Team.blue) {
	//		PlayerTeamB.Remove (otherPlayer.UserId);	
	//	}
	//	AllPlayers.Remove (otherPlayer.UserId);
	//}

	#endregion
}
