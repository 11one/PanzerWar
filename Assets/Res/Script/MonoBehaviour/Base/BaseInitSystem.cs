using UnityEngine;
using System.Collections;



public class BaseInitSystem : MonoBehaviour
{

    public System.Action onVehicleLoaded;

	[HideInInspector]
	public int PlayerID;
	[HideInInspector]
	public string UserID;
	[HideInInspector]
	public bool OnPhotonInstantiatePropertyInit = false;
	[HideInInspector]
	public string VehicleName = "Null";

	[HideInInspector]
	public PunTeams.Team ownerTeam = PunTeams.Team.none;
	[HideInInspector]
	public string PlayerName = "";
	[HideInInspector]
	public bool isAssetBundleLoaded = false;



    public InstanceNetType _InstanceNetType = InstanceNetType.None;

    [System.NonSerialized]
    public int[] BulletCountList = new int[3];



	public virtual void Awake ()
	{
		//VehicleName = GetVehicleName ();
//		if (InstanceNetType == NetWorkSwtich.GameNetworkClient || InstanceNetType == NetWorkSwtich.GameNetWorkOffline) {
//			GameNetwork.Instance.GameStarterUISystem.transform.Find ("MainPage/Top/Close").GetComponent<UnityEngine.UI.Button> ().onClick.RemoveAllListeners ();
//			GameNetwork.Instance.GameStarterUISystem.transform.Find ("MainPage/Top/Close").GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (
//				delegate {
//					GameNetwork.Instance.MainPanel.SetActive (false);
//				});
//			GameNetwork.Instance.PlayerNeedCursor = false;
//		}
	}

	public static bool isBot (InstanceNetType netType)
	{
		if (netType == InstanceNetType.GameNetWorkBotClient || netType == InstanceNetType.GameNetWorkBotMaster || netType == InstanceNetType.GameNetworkBotOffline)
			return true;
		else
			return false;
	}

	public static bool isLocalPlayer (InstanceNetType netType)
	{
		if (netType == InstanceNetType.GameNetworkClient || netType == InstanceNetType.GameNetWorkOffline)
			return true;
		else
			return false;
	}

	public virtual void OnPhotonInstantiate ()
	{

	}
}
