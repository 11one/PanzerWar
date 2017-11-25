using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor (typeof(VehicleTextData))]
public class VehicleTextDataEditor : EditorWindowBase {
	VehicleTextData vehicleTextData;
	public override void Awake ()
	{
		base.Awake ();
		EditorHeadline = "ShanghaiWindy Ground Vehicle Text Data Manager";
		vehicleTextData = (VehicleTextData)target;

		UpdateAssetLabel ();
	}


	public override void OnInspectorGUI ()
	{
		vehicleTextData = (VehicleTextData)target;


		BaseGUI ();

        if (GUILayout.Button("Export Data as Json")) {
            string path = EditorUtility.SaveFilePanel("Export As Json", "Others/Data/",vehicleTextData.AssetName,"json");
          
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] data = System.Text.Encoding.Default.GetBytes(JsonUtility.ToJson(target));
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        if (GUILayout.Button("Set Asset Label")) {
            UpdateAssetLabel();
        }

        if (vehicleTextData.TFParameter.bulletType != null) {
            BulletScript bulletData = vehicleTextData.TFParameter.bulletType.GetComponent<BulletScript>();
            EditorGUILayout.HelpBox(string.Format("AP Damage {0} Peneration{1}", bulletData.APDamage, bulletData.APPenetration), MessageType.None);
            EditorGUILayout.HelpBox(string.Format("HE Damage {0} Peneration{1}", bulletData.HeDamage, bulletData.HePenetration), MessageType.None);
            EditorGUILayout.HelpBox(string.Format("APCR Damage {0} Peneration{1}", bulletData.APDamage * 0.75f, bulletData.ApcrPenration), MessageType.None);
        }

		base.OnInspectorGUI ();



  
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	private void UpdateAssetLabel(){
		if (vehicleTextData.AssetName != "VehicleNameTextData") {
			AssetImporter assetImporter = AssetImporter.GetAtPath (AssetDatabase.GetAssetPath (vehicleTextData));
			assetImporter.assetBundleName = vehicleTextData.AssetName;
			assetImporter.assetBundleVariant = "data";
		}
	}
}
