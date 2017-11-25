//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class CopyEngineCurve : EditorWindow 
{
 
	private AnimationCurve ClipboardCurve = null;
	
    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/UnityFS/Copy Engine Curve")]
	
    static void Init () 
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (CopyEngineCurve));
    }
    
    void OnGUI () 
	{	
		//Copy button.
		if (GUI.Button(new Rect(10, 10, position.width - 20, 40), "Copy"))
		{ 
			if( null != Selection.activeGameObject )
			{
				Engine engine = Selection.activeGameObject.GetComponent<Engine>();
				ClipboardCurve = engine.PercentageForceAppliedVSAirspeedKTS;
			}
		}
		
		//Import button.
		if (GUI.Button(new Rect(10, 60, position.width - 20, 40), "Paste"))
		{
			if( null != Selection.activeGameObject )
			{
				Engine engine = Selection.activeGameObject.GetComponent<Engine>();
				engine.PercentageForceAppliedVSAirspeedKTS = new AnimationCurve( ClipboardCurve.keys );
			}
		}
    }
}