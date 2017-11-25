using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class VehicleGUIInfo{
	public int APDamage,HEDamage,APCRDamage,APPenetration,HEPenetration,APCRPenetration,MaxVelocity,MinVelocity,Health;
}

public class TankInfo : MonoBehaviour {
	public static TankInfo ThisScript;

	public Text Command,Driver,Loader,Radioman,Gunner,Exp;
	public Text Damage,Penetration,Velocity,Health;
	public Slider DamageWeight,PenetrationWeight,VelocityWeight,HealthWeight;
	void Start(){
		ThisScript = this;
	}
	//Called By GarageTankManageSelected OnClick();
	public void UpdateCrewLabel(Crew.CrewInfo MyCrew){
		Command.text = "Exp:"+MyCrew.ChiefProficiency.ToString();
		Driver.text = "Exp:"+MyCrew.DriverProficiency.ToString();
		Loader.text = "Exp:"+MyCrew.ReloaderProficiency.ToString();
		Radioman.text = "Exp:"+MyCrew.RedioProficiency.ToString();
		Gunner.text = "Exp:"+MyCrew.GunnerProficiency.ToString();
		//if (MyCrew.Exp < AccountDataManager.GetVehicleExperienced (GarageTankManageSelected.currentSelectedVehicleName))
		//	Exp.text = uGUI_Localsize.GetContent ("Experience") + MyCrew.Exp.ToString () + "/" + AccountDataManager.GetVehicleExperienced (GarageTankManageSelected.currentSelectedVehicleName);
		//else
			//Exp.text = uGUI_Localsize.GetContent ("Experienced");
	}


	public void UpdateVehicleLabel(VehicleGUIInfo vehicleGUIInfo){
		Damage.text = string.Format ("{0}{1}/{2}/{3}", uGUI_Localsize.GetContent ("GUIDamage"), vehicleGUIInfo.APDamage.ToString (), vehicleGUIInfo.HEDamage.ToString (), vehicleGUIInfo.APCRDamage.ToString ());
		Penetration.text = string.Format ("{0}{1}/{2}/{3}", uGUI_Localsize.GetContent ("GUIPenetration"), vehicleGUIInfo.APPenetration.ToString (), vehicleGUIInfo.HEPenetration.ToString (), vehicleGUIInfo.APCRPenetration.ToString ());
		Velocity.text = string.Format ("{0}{1}/{2}", uGUI_Localsize.GetContent ("GUISpeed"), vehicleGUIInfo.MaxVelocity.ToString (), vehicleGUIInfo.MinVelocity.ToString ());
		Health.text = string.Format ("{0}{1}", uGUI_Localsize.GetContent ("GUIHealth"), vehicleGUIInfo.Health.ToString ());

		DamageWeight.value = vehicleGUIInfo.APDamage / 700.0f;
		PenetrationWeight.value = vehicleGUIInfo.APPenetration / 300.0f;
		HealthWeight.value = vehicleGUIInfo.Health / 2000.0f;
		VelocityWeight.value = vehicleGUIInfo.MaxVelocity / 65f;

	}

}

public class Crew {
	public struct CrewInfo {
		public float GunnerProficiency;
		public float ReloaderProficiency;
		public float ChiefProficiency;
		public float DriverProficiency;
		public float RedioProficiency;
		public float RepairMent;
		public string Snow, Grass, Sand;
		public float Exp;
	}
	;
	public static CrewInfo NewCrewInfo() {
		CrewInfo temp = new CrewInfo();
		temp.GunnerProficiency = 0.0f;
		temp.ReloaderProficiency = 0.0f;
		temp.ChiefProficiency = 0.0f;
		temp.DriverProficiency = 0.0f;
		temp.RedioProficiency = 0.0f;
		temp.RepairMent = 0.0f;
		temp.Snow = "null";
		temp.Grass = "null";
		temp.Sand = "null";
		temp.Exp = 0;
		return temp;
	}
	public static Dictionary<string, System.Object> GetCrewInfo(CrewInfo MyCrew) {
		Dictionary<string, System.Object> Temp = new Dictionary<string, System.Object>();
		Temp.Add("GunnerProficiency", MyCrew.GunnerProficiency);
		Temp.Add("ReloaderProficiency", MyCrew.ReloaderProficiency);
		Temp.Add("ChiefProficiency", MyCrew.ChiefProficiency);
		Temp.Add("DriverProficiency", MyCrew.DriverProficiency);
		Temp.Add("RedioProficiency", MyCrew.RedioProficiency);
		Temp.Add("RepairMent", MyCrew.RepairMent);
		Temp.Add("Snow", MyCrew.Snow);
		Temp.Add("Sand", MyCrew.Sand);
		Temp.Add("Grass", MyCrew.Grass);
		Temp.Add("Exp", MyCrew.Exp);
		return Temp;
	}
	public static CrewInfo GetCrewInfo(Dictionary<string, System.Object> CrewDictionary) {
		CrewInfo Temp = NewCrewInfo();
		if (CrewDictionary != null) {
			if (CrewDictionary.ContainsKey("GunnerProficiency"))
				Temp.GunnerProficiency = float.Parse(CrewDictionary["GunnerProficiency"].ToString());
			if (CrewDictionary.ContainsKey("ReloaderProficiency"))
				Temp.ReloaderProficiency = float.Parse(CrewDictionary["ReloaderProficiency"].ToString());
			if (CrewDictionary.ContainsKey("ChiefProficiency"))
				Temp.ChiefProficiency = float.Parse(CrewDictionary["ChiefProficiency"].ToString());
			if (CrewDictionary.ContainsKey("DriverProficiency"))
				Temp.DriverProficiency = float.Parse(CrewDictionary["DriverProficiency"].ToString());
			if (CrewDictionary.ContainsKey("RedioProficiency"))
				Temp.RedioProficiency = float.Parse(CrewDictionary["RedioProficiency"].ToString());
			if (CrewDictionary.ContainsKey("RepairMent"))
				Temp.RepairMent = float.Parse(CrewDictionary["RepairMent"].ToString());
			if (CrewDictionary.ContainsKey("Snow")) {
				Temp.Snow = CrewDictionary["Snow"].ToString();
			}
			if (CrewDictionary.ContainsKey("Grass")) {
				Temp.Grass = CrewDictionary["Grass"].ToString();
			}
			if (CrewDictionary.ContainsKey("Sand")) {
				Temp.Sand = CrewDictionary["Sand"].ToString();
			}
			if (CrewDictionary.ContainsKey("Exp")) {
				Temp.Exp = int.Parse(CrewDictionary["Exp"].ToString());
			}

		}
		return Temp;
	}
}