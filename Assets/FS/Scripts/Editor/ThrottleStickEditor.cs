//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(ThrottleStick))]
public class ThrottleStickEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool ParametersFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private ThrottleStick targetThrottleStick = null;
	
	
	private void OnEnable()
	{
		targetThrottleStick = target as ThrottleStick;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetThrottleStick, "Throttle Stick" );
	}
	
	public override void OnInspectorGUI() 
	{
		//Undo start..
		UndoManager.CheckUndo();
		
		//Show input dialogs for yoke.
		InputFoldOut = EditorGUILayout.Foldout( InputFoldOut, "Input" );
		if ( InputFoldOut )
		{
			//Call base class to draw some stuff..
			ShowInputAxisOptions( "Throttle", targetThrottleStick.Controller );
			
			EditorGUILayout.Space();
		}
		
		ParametersFoldOut = EditorGUILayout.Foldout( ParametersFoldOut, "Parameters" );
		if ( ParametersFoldOut )
		{	
			targetThrottleStick.ThrottleAxis = EditorGUILayout.Vector3Field( "Throttle Axis", targetThrottleStick.ThrottleAxis );
			targetThrottleStick.MaxDeflectionDegrees = EditorGUILayout.FloatField( "Max Deflection Degrees", targetThrottleStick.MaxDeflectionDegrees );		
		}
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetThrottleStick);
    }
}	
		