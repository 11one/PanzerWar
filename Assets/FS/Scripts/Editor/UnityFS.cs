using UnityEditor;
using UnityEngine;

public class UnityFS : EditorWindow
{
	private static WWW www = null;
	private static bool GettingUpdate = false;
	private static bool ShouldImport = false;
	private static bool Done = false;
	private static string PackagePath = null;
	private static float progress = 0.0f;
	
	public static void ShowWindow() 
	{
		//Clear all values...
		www = null;
		GettingUpdate = false;
		ShouldImport = false;
		Done = false;
		PackagePath = null;
		progress = 0.0f;
		
		PackagePath = Application.dataPath + "/UnityFS/Data/" + "update.unitypackage";
		EditorWindow.GetWindow(typeof(UnityFS));
	}

	void OnGUI()
	{ 
		EditorGUILayout.BeginVertical();
		GUILayout.TextArea( "UnityFS\n---------\nA new version of UnityFS is now available. Press download to update and install the latest UnityFS package.\n\n" +
		"For more information on the Update, please visit the UnityFS website - http://unityfs.chris-cheetham.com " );
		
		if ( !GettingUpdate && !Done )
		{
			if (GUILayout.Button("Download"))
	        {
				www = new WWW(AircraftEditor.Contents[3]);
	        	GettingUpdate = true;
				 progress = 0.0f;
	        }
		}
		else if ( Done )
		{
			GUILayout.Button("Done!");
		}
		else
		{
			GUILayout.Button("Downloading Please wait.." + ((int)(progress * 100.0f)).ToString() + "%" );
			Repaint();
		}
		
		EditorGUILayout.EndVertical();
		if ( ShouldImport )
		{
			AssetDatabase.ImportPackage( PackagePath, true );
			ShouldImport = false;
			Done = true;
		}
		if ( GettingUpdate )
		{
			if ( www.isDone )
			{
				if ( null == www.error )
				{
      				System.IO.FileStream fs = new System.IO.FileStream(PackagePath, System.IO.FileMode.Create,
                                  													System.IO.FileAccess.Write);
      				fs.Write(www.bytes, 0, www.bytes.Length);
      				fs.Close();	
					ShouldImport = true;
				}	
				GettingUpdate = false;
			}
			else
			{
				progress = www.progress;
			}
		}
	}
}