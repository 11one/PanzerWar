//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(WheelBrake))]
public class WheelBrakeEditor : InputEditor 
{
	private bool InputFoldOut = true;

	private HOEditorUndoManager UndoManager = null;
	private WheelBrake targetWheelBrake = null;
	
	private void OnEnable()
	{
		targetWheelBrake = target as WheelBrake;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetWheelBrake, "Wheel Brake" );
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
			ShowInputButtonOptions( "Brake Input", targetWheelBrake.Controller );
						
			EditorGUILayout.Space();
			
		}
		
		base.OnInspectorGUI();
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetWheelBrake);
    }
}	
		