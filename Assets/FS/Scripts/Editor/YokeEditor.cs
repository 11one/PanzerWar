//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(Yoke))]
public class YokeEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool ParametersFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private Yoke targetYoke = null;
	
	
	private void OnEnable()
	{
		targetYoke = target as Yoke;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetYoke, "Yoke" );
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
			ShowInputAxisOptions( "Pitch", targetYoke.PitchController );
			
			EditorGUILayout.Space();
			
			ShowInputAxisOptions( "Roll", targetYoke.RollController );
			
			EditorGUILayout.Space();
			
		}
		
		
		ParametersFoldOut = EditorGUILayout.Foldout( ParametersFoldOut, "Parameters" );
		if ( ParametersFoldOut )
		{
			targetYoke.PitchAxis = EditorGUILayout.Vector3Field( "Pitch Axis", targetYoke.PitchAxis );
			targetYoke.MaxPitchTranslationMeters = EditorGUILayout.FloatField( "Max Pitch Translation Meters", targetYoke.MaxPitchTranslationMeters );
			
			EditorGUILayout.Space();
			
			targetYoke.RollAxis = EditorGUILayout.Vector3Field( "Roll Axis", targetYoke.RollAxis );
			targetYoke.MaxRollDeflectionDegrees = EditorGUILayout.FloatField( "Max Roll Deflection Degrees", targetYoke.MaxRollDeflectionDegrees );

		}
		
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetYoke);
    }
}	
		