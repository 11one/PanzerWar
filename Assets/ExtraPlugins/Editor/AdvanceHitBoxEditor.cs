using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AdvanceHitBox))]
public class AdvanceHitBoxEditor : Editor {

	AdvanceHitBox advcaneHitBox;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button("Add A HitBox")) {
			
		}
	}
	void Awake(){
		advcaneHitBox = (AdvanceHitBox)target;
	}
	void OnSceneGUI() {
		if(advcaneHitBox&&advcaneHitBox.MainBody)
			foreach (AdvanceHitBox.HitBoxProperty hitBoxProperty  in advcaneHitBox.hitBoxProperty) {
				Vector3 WorldPosition =hitBoxProperty.LocalPositionV3+advcaneHitBox.MainBody.position;
				hitBoxProperty.LocalPositionV3 = Handles.PositionHandle (WorldPosition,Quaternion.identity) - advcaneHitBox.MainBody.position;
				Handles.Label(WorldPosition,"Armor:"+hitBoxProperty.Armor.ToString()+"HitBoxType"+hitBoxProperty.hitBoxType.ToString());
			}
	}


}
