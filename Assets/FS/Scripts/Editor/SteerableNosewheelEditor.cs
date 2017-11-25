//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(SteerableNosewheel))]
public class SteerableNosewheelEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool ParametersFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private SteerableNosewheel targetSteerableNosewheel = null;
	
	
	private void OnEnable()
	{
		targetSteerableNosewheel = target as SteerableNosewheel;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetSteerableNosewheel, "Steerable Nosewheel" );
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
			ShowInputAxisOptions( "Steer", targetSteerableNosewheel.Controller );
			
			EditorGUILayout.Space();
		}
		
		ParametersFoldOut = EditorGUILayout.Foldout( ParametersFoldOut, "Parameters" );
		if ( ParametersFoldOut )
		{	
			targetSteerableNosewheel.MaxDeflectionDegrees = EditorGUILayout.FloatField( "Max Deflection Degrees", targetSteerableNosewheel.MaxDeflectionDegrees );
			
			targetSteerableNosewheel.SteerAxis = EditorGUILayout.Vector3Field( "Steer Axis", targetSteerableNosewheel.SteerAxis );
			
			targetSteerableNosewheel.Model = EditorGUILayout.ObjectField("Model", targetSteerableNosewheel.Model, typeof(UnityEngine.GameObject), true) as UnityEngine.GameObject;
			
			targetSteerableNosewheel.ModelRotationAxis = EditorGUILayout.Vector3Field( "Model Rotation Axis", targetSteerableNosewheel.ModelRotationAxis );
		}
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetSteerableNosewheel);
    }
}	
		