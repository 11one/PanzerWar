//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class CopyGroundEffectCurves : EditorWindow 
{
 
	private AnimationCurve ClipboardCLCurve = null;
	private AnimationCurve ClipboardCDCurve = null;
	
    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/UnityFS/Copy Ground Effect Curves")]
	
    static void Init () 
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (CopyGroundEffectCurves));
    }
    
    void OnGUI () 
	{	
		//Copy button.
		if (GUI.Button(new Rect(10, 10, position.width - 20, 40), "Copy"))
		{ 
			if( null != Selection.activeGameObject )
			{
				GroundEffect groundEffect = Selection.activeGameObject.GetComponent<GroundEffect>();
				ClipboardCLCurve = groundEffect.CLHeightVsChord;
				ClipboardCDCurve = groundEffect.CDHeightVsSpan;
			}
		}
		
		//Import button.
		if (GUI.Button(new Rect(10, 60, position.width - 20, 40), "Paste"))
		{
			if( null != Selection.activeGameObject )
			{
				GroundEffect groundEffect = Selection.activeGameObject.GetComponent<GroundEffect>();
				groundEffect.CLHeightVsChord = new AnimationCurve( ClipboardCLCurve.keys );
				groundEffect.CDHeightVsSpan = new AnimationCurve( ClipboardCDCurve.keys );
			}
		}
    }
}