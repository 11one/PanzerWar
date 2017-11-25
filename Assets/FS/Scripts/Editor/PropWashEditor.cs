//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(PropWash))]
public class PropWashEditor : Editor 
{
	private bool AffectedSectionsFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private PropWash targetPropwash = null;
	
	
	private void OnEnable()
	{
		targetPropwash = target as PropWash;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetPropwash, "PropWash" );
	}
	
	public override void OnInspectorGUI() 
	{
		//Undo start..
		UndoManager.CheckUndo();
		
		targetPropwash.PropWashSource = EditorGUILayout.ObjectField("Propwash Source", targetPropwash.PropWashSource, typeof(Engine), true) as Engine;
		targetPropwash.PropWashStrength = EditorGUILayout.FloatField( "Strength Multiplier", targetPropwash.PropWashStrength );
		
		AffectedSectionsFoldOut = EditorGUILayout.Foldout( AffectedSectionsFoldOut, "Affected Sections (Root outwards)" );
		if (AffectedSectionsFoldOut)
		{
			if ( null != targetPropwash.AffectedSections )
			{
				for ( int i=0; i<targetPropwash.AffectedSections.Length; i ++ )
				{
					targetPropwash.AffectedSections[i] = EditorGUILayout.Toggle( i.ToString(), targetPropwash.AffectedSections[i] );
				}
			}
		}
			
		EditorGUILayout.Space();
		
		
		
		//User help....
		if ( null == targetPropwash.PropWashSource )
		{
			EditorGUILayout.HelpBox( "No prop wash source. This will do nothing!", MessageType.Error );
		}    	

       	//Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetPropwash);
    }
}	
		