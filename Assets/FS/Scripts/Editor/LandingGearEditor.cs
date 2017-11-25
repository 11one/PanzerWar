//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(LandingGear))]
public class LandingGearEditor : InputEditor 
{
	private bool InputFoldOut = true;

	private HOEditorUndoManager UndoManager = null;
	private LandingGear targetLandingGear = null;
	
	private void OnEnable()
	{
		targetLandingGear = target as LandingGear;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetLandingGear, "Landing Gear" );
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
			ShowInputButtonOptions( "Toggle Landing Gear", targetLandingGear.LandingGearController );
						
			EditorGUILayout.Space();
			
		}
		
		base.OnInspectorGUI();
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetLandingGear);
    }
}	
		