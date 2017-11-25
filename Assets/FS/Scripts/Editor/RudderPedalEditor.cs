//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(RudderPedal))]
public class RudderPedalEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool ParametersFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private RudderPedal targetRudderPedal = null;
	
	
	private void OnEnable()
	{
		targetRudderPedal = target as RudderPedal;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetRudderPedal, "Rudder Pedal" );
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
			ShowInputAxisOptions( "Yaw", targetRudderPedal.Controller );
			
			EditorGUILayout.Space();
		}
		
		ParametersFoldOut = EditorGUILayout.Foldout( ParametersFoldOut, "Parameters" );
		if ( ParametersFoldOut )
		{		
			targetRudderPedal.TranslateAxis = EditorGUILayout.Vector3Field( "Translate Axis", targetRudderPedal.TranslateAxis );
			targetRudderPedal.DeflectionMeters = EditorGUILayout.FloatField( "Deflection Meters", targetRudderPedal.DeflectionMeters );		
		}
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetRudderPedal);
    }
}	
		