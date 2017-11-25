using UnityEngine;
using System.Collections;

public class AdvanceHitBox : MonoBehaviour {
	public enum ArmorType{
		Normal
	}

	public enum HitBoxType{
		Command,
		Driver,
		Loader,
		Radioman,
		Gunner,
		Engine,
		Shell
	}
	[System.Serializable]
	public class HitBoxProperty{
		public ArmorType armorType;
		public float Armor = 0;
		public HitBoxType hitBoxType;
		public GameObject SpecialMesh;
		public Vector3 LocalPositionV3 = Vector3.zero;
		public Vector3 LocalScaleV3= Vector3.zero;

	}
	public HitBoxProperty[] hitBoxProperty;
	public Transform MainBody;
	#if UNITY_EDITOR
	void OnDrawGizmos(){
		if(this&&this.MainBody)
			foreach (AdvanceHitBox.HitBoxProperty hitBoxProperty  in this.hitBoxProperty) {
				Vector3 WorldPosition =hitBoxProperty.LocalPositionV3+this.MainBody.position;
				Gizmos.DrawCube(WorldPosition,hitBoxProperty.LocalScaleV3);
			}
	}
	#endif
}
