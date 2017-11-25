//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEngine;
using UnityEditor;
using System.IO;

public class JavaFoil : EditorWindow 
{
	private Vector2 ScrollPos = Vector2.zero;
	
    // Add menu
    [MenuItem ("Window/UnityFS/JavaFoil")]
    static void Init () 
	{
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (JavaFoil));
    }
    
    void OnGUI () 
	{
		EditorGUILayout.BeginHorizontal();
		ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.Width (position.width), GUILayout.Height (100));
		string text = "Click the launch button to run JavaFoil and create your own aerofoils. " +
						"This link is provided meerly as a matter of convenience. " +
						"UnityFS is in no way affiliated with JavaFoil and accepts no liability should you choose to use it. " +
						"For more information about JavaFoil visit http://www.mh-aerotools.de/airfoils/javafoil.htm";
		
		text = GUILayout.TextArea(text, GUILayout.Height(position.height - 30));        
        EditorGUILayout.EndScrollView();
		EditorGUILayout.EndHorizontal();
		
		if(GUILayout.Button("Launch"))
	    {
	    	Application.OpenURL("http://www.mh-aerotools.de/airfoils/jf_applet.htm");
	    }
    }
}