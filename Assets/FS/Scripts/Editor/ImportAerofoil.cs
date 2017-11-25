//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class ImportAerofoil : EditorWindow 
{
 
	private AnimationCurve CL = AnimationCurve.Linear(-180.0f,0.0f,180,0);
	private AnimationCurve CD = AnimationCurve.Linear(-180.0f,0.0f,180,0);
	private AnimationCurve CM = AnimationCurve.Linear(-180.0f,0.0f,180,0);
	private Aerofoil TargetAerofoil = null;
	private string FileName = "";
	private bool Rename = true;
	
    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/UnityFS/Import Aerofoil")]
    static void Init () 
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (ImportAerofoil));
    }
    
    void OnGUI () 
	{	
		//Import button.
		if (GUI.Button(new Rect(10, 10, position.width - 20, 20), "Import .afl"))
		{
			string path = EditorUtility.OpenFilePanel( "Aerofoil", "", "afl");
	        if (path.Length != 0) 
			{
				//Store file name.
				FileName = Path.GetFileNameWithoutExtension(path);
				
				//Reset curves.
				CL = new AnimationCurve();
				CD = new AnimationCurve();
				CM = new AnimationCurve();
				
				//Create a stream reader.
				StreamReader file = new StreamReader(path);
				string text = "";
				
				//Skip past the header.
				while ( !text.Contains("alpha") )
				{
					text = file.ReadLine();
				}
				
				//Read in the data.
				while ( text != null )
				{
					text = file.ReadLine();
					if ( null != text )
					{
						string[] parts = text.Split(null);
						
						string[] reducedParts = new string[4];
						int j=0;
						for ( int i=0; i<parts.Length; i++ )
						{
							if ( parts[i].Length > 0 )
							{
								reducedParts[j] = parts[i];
								j++;
							}
						}
						
						float aoa = 0.0f;
						float.TryParse(reducedParts[0], out aoa );
						
						if ( (int)aoa == 180 )
						{
							return;
						}
						
						float cl= 0.0f;
						float.TryParse(reducedParts[1], out cl );
						
						float cd= 0.0f;
						float.TryParse(reducedParts[2], out cd );
						
						float cm= 0.0f;
						float.TryParse(reducedParts[3], out cm );
						
						//Assign value to curves.
						CL.AddKey( aoa, cl );
						CD.AddKey( aoa, cd );
						CM.AddKey( aoa, cm );
						
					}
				}
			}
		}
		
		//Curves.
		CL = EditorGUI.CurveField( new Rect(10,40,position.width-20,40), "(Cl)Lift", CL );
		CD = EditorGUI.CurveField( new Rect(10,90,position.width-20,40), "(Cd)Drag", CD );
		CM = EditorGUI.CurveField( new Rect(10,140,position.width-20,40), "(Cm)Moment", CM );
		
		//Target aerofoil selector.
		TargetAerofoil = (Aerofoil)EditorGUI.ObjectField( new Rect(10,200,position.width - 20, 20),
                "Target Aerofoil",
                TargetAerofoil,
                typeof(Aerofoil),true);
		
		//Filename preview / editor/
		FileName = EditorGUI.TextField( new Rect(10,230,position.width - 20, 20), "Name:", FileName);
		
		//Rename toggle.
		Rename = EditorGUI.Toggle( new Rect(10, 250,position.width,20), "Rename", Rename);
		
		//Apply
		if (GUI.Button(new Rect(10, 300, position.width - 20, 40), "APPLY"))
		{
			if ( null != TargetAerofoil )
			{
				//Assign curves.
				TargetAerofoil.CL = CL;
				TargetAerofoil.CD = CD;
				TargetAerofoil.CM = CM;
				
				//Rename
				if(Rename)
				{
					TargetAerofoil.name = FileName;
				}
			}
		}
		
    }
}