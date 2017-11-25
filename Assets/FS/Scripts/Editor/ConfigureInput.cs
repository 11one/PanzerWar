//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class ConfigureInput : EditorWindow 
{
	private Vector2 ScrollPos = Vector2.zero;
	private bool InputOverwritten = false;
	private bool InputOverwriteFailed = false;
	
    // Add menu
    [MenuItem ("Window/UnityFS/** Configure Input **")]
    static void Init () 
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (ConfigureInput));
    }
    
    void OnGUI () 
	{
		EditorGUILayout.BeginHorizontal();
		ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.Width (position.width), GUILayout.Height (200));
		string text = "Warning: The following tool will overwrite your default input project settings with UnityFS axis. " +
						"If you wish to keep the default settings you can add the UnityFS axis manually or " +
						"alternatively configure aircraft to use your own custom defined Input." +
						"\n \nThe following axis will be created: \n\nPitch, Roll, Yaw, Throttle, Landing Gear, Change View, Camera Zoom, Brake, Ignition"; 
		
		text = GUILayout.TextArea(text, GUILayout.Height(position.height - 30));        
        EditorGUILayout.EndScrollView();
		EditorGUILayout.EndHorizontal();
		
		if ( !InputOverwritten )
		{
			if(GUILayout.Button("OVERWRITE"))
		    {
				if ( OverwriteInput() ) 
				{
		    		InputOverwritten = true;
				}
				else
				{
					InputOverwriteFailed = true;
				}
		    }
		}
		else if ( InputOverwriteFailed )
		{
			if(GUILayout.Button("Overwrite failed."))
		    {
				//Do nothing.
		    }
		}
		else
		{
			if(GUILayout.Button("Done."))
		    {
				//Do nothing.
		    }
		}
    }
	
	bool OverwriteInput()
	{
		string sourcePath = Application.dataPath + "/UnityFS/Data/input.dat";
		string destPath = Application.dataPath + "/../ProjectSettings/InputManager.asset";
		
		if ( !File.Exists( sourcePath ) )
		{
			Debug.LogError( "Configure Input - Overwrite error. Could not find source file." );
			return false;
		}
		
		if ( !File.Exists( destPath ) )
		{
			Debug.LogError( "Configure Input - Overwrite error. Could not find dest file." );
			return false;
		}
		
		File.Copy( sourcePath, destPath, true );
		AssetDatabase.Refresh();
		
		return true;
	}
}