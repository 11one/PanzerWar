using UnityEngine;
using System.Collections;

public class BasePlayerState : MonoBehaviour {
	[HideInInspector]
	public string TankName;
	[HideInInspector]
	public int Health =1000;

	public virtual void ApplyHitBoxDamage(int Damage,float Degree,BaseFireSystem Owner,HitBox _hitBox){
		Debug.Log("Base.ApplyHitBoxDamage");
	}
	public virtual void OnRicochet(){
		Debug.Log("Base.OnRicochet");
	}
	public virtual void OnNotBreakDown(){
		Debug.Log("Base.OnNotBreakDown");
	}
}
