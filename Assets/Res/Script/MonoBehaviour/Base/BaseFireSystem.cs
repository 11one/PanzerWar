using UnityEngine;
using System.Collections;

public class BaseFireSystem : MonoBehaviour {
	public virtual void AddDamage(int Damage,string HitVehicle){
		Debug.Log ("CoreCalled");
	}
	public virtual void KillFriend(string Vehicle){
		Debug.Log ("CoreCalled");
	}
	public virtual void KillTank(string Vehicle){
		Debug.Log ("CoreCalled");
	}
}
