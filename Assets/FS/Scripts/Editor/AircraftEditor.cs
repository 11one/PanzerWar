//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
using UnityEngine;
using System;
using System.IO;     
using System.Text;

[CustomEditor(typeof(Aircraft))]
public class AircraftEditor : InputEditor 
{
	private bool FetchVersion = false;
	private bool GettingVersion = false;
	private WWW www = null;
	public static string[] Contents = null;
	
	private HOEditorUndoManager UndoManager = null;
	private Aircraft targetAircraft = null;
	private bool InputFoldOut = true;
	
	private void OnEnable()
	{
		targetAircraft = target as Aircraft;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetAircraft, "Aircraft" );
	}
	
	public override void OnInspectorGUI() 
	{
		//Undo start..
		UndoManager.CheckUndo();
		 
		if ( GUILayout.Button( "Get latest version of UnityFS." ) )
		{
			FetchVersion = true;
		}
		
		//Show input dialogs for camera change.
		InputFoldOut = EditorGUILayout.Foldout( InputFoldOut, "Input" );
		if ( InputFoldOut )
		{
			ShowInputButtonOptions( "Change Camera", targetAircraft.ChangeCameraController );
			EditorGUILayout.Space();
		}
		
		base.OnInspectorGUI();
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetAircraft);
	}
	
	public void OnSceneGUI() 
	{
		CopyGizmos();
		CheckForUpdates();
    }
	 
	private void CopyGizmos()
	{
		string source = Application.dataPath + "/UnityFS/Data/";
		string dest = Application.dataPath + "/Gizmos/";
		string[] files = System.IO.Directory.GetFiles(source, "*.png");	
		
		if ( !Directory.Exists(dest) )
		{
			Directory.CreateDirectory(dest);
		}

		foreach ( string sourceFilename in files )
		{
			string destFilename = sourceFilename.Replace( source, dest );
			if ( !File.Exists(destFilename) )
			{
				File.Copy( sourceFilename, destFilename );
			}
		}
	}
	
	
	private void CheckForUpdates()
	{ 
		if ( !FetchVersion )
		{
			string autoupdatePath = Application.dataPath + "/UnityFS/Data/" + "autoupdate.dat";
			if ( File.Exists(autoupdatePath) )
			{
				using (StreamReader sr = new StreamReader(autoupdatePath))
	            {
					Contents = new string[4];
					Contents[0] = sr.ReadLine();
					Contents[1] = sr.ReadLine();
					Contents[2] = sr.ReadLine();
					Contents[3] = sr.ReadLine();
				}	
				string LastCheckString = Contents[1];
				DateTime LastCheck;
				DateTime.TryParse( LastCheckString, out LastCheck );
				TimeSpan difference = DateTime.Now.Subtract( LastCheck );
				if ( difference.TotalDays > 1 )
				{
					Contents[1] = DateTime.Now.ToString();
					System.IO.StreamWriter file = new System.IO.StreamWriter(autoupdatePath);
					file.WriteLine(Contents[0]);
					file.WriteLine(Contents[1]);
					file.WriteLine(Contents[2]);
					file.WriteLine(Contents[3]);
					file.Close();
					FetchVersion = true;
				}
			}
		}
		if ( FetchVersion )
		{
			Debug.Log( "UnityFS - Checking for updates." );
			
			www = new WWW(Contents[2]);
			FetchVersion = false;
			GettingVersion = true;
		}
		if ( GettingVersion )
		{
			if ( www.isDone )
			{
				if ( null == www.error )
				{	
					float onlineVersion;
					float.TryParse( www.text, out onlineVersion );
					float currentVersion;
					float.TryParse( Contents[0], out currentVersion );
					if ( onlineVersion > currentVersion )
					{
						UnityFS.ShowWindow();
					}
					else
					{
						Debug.Log( "UnityFS - Current version is up to date." );
					}
				}
				GettingVersion = false;
			}
		}
	}
}	
		