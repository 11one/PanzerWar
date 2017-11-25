//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(ControlSurface))]
public class ControlSurfaceEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool ParametersFoldOut = true;
	private bool VisualsFoldOut = true;
	private bool AffectedSectionsFoldOut = true;
	
	private HOEditorUndoManager UndoManager = null;
	private ControlSurface targetControlSurface = null;
	
	
	private void OnEnable()
	{
		targetControlSurface = target as ControlSurface;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetControlSurface, "ControlSurface" );
	}
	
	public override void OnInspectorGUI() 
	{
		//Undo start..
		UndoManager.CheckUndo();
		
		InputFoldOut = EditorGUILayout.Foldout( InputFoldOut, "Input" );
		if ( InputFoldOut )
		{
			//Call base class to draw some stuff..
			ShowInputAxisOptions( "Surface", targetControlSurface.Controller );
			
			targetControlSurface.InputCurve = EditorGUILayout.CurveField( "Input Curve", targetControlSurface.InputCurve );
			
			//If input curve has no keyframes make some to ease user.
			if ( 0 == targetControlSurface.InputCurve.keys.Length )
			{
				targetControlSurface.InputCurve.AddKey( 0.0f, 0.0f );
				targetControlSurface.InputCurve.AddKey( 1.0f, 1.0f );
			}
		
		}
		
		EditorGUILayout.Space();
		
		ParametersFoldOut = EditorGUILayout.Foldout( ParametersFoldOut, "Parameters" );
		if ( ParametersFoldOut )
		{
			targetControlSurface.MaxDeflectionDegrees = EditorGUILayout.FloatField( "Max Deflection Degrees", targetControlSurface.MaxDeflectionDegrees );
						
			int rootPercent = (int)(targetControlSurface.RootHingeDistanceFromTrailingEdge * 100.0f);
			rootPercent = EditorGUILayout.IntSlider( "Root hinge offset", rootPercent, 0, 100 );
			targetControlSurface.RootHingeDistanceFromTrailingEdge = (float)rootPercent / 100.0f;
			
			int tipPercent = (int)(targetControlSurface.TipHingeDistanceFromTrailingEdge * 100.0f);
			tipPercent = EditorGUILayout.IntSlider( "Tip hinge offset", tipPercent, 0, 100 );
			targetControlSurface.TipHingeDistanceFromTrailingEdge = (float)tipPercent / 100.0f;
			
			AffectedSectionsFoldOut = EditorGUILayout.Foldout( AffectedSectionsFoldOut, "Affected Sections (Root outwards)" );
			if (AffectedSectionsFoldOut)
			{
				if ( null != targetControlSurface.AffectedSections )
				{
					for ( int i=0; i<targetControlSurface.AffectedSections.Length; i ++ )
					{
						targetControlSurface.AffectedSections[i] = EditorGUILayout.Toggle( i.ToString(), targetControlSurface.AffectedSections[i] );
					}
				}
			}
			
		}
		
		EditorGUILayout.Space();
		
		VisualsFoldOut = EditorGUILayout.Foldout( VisualsFoldOut, "Visuals" );
		if ( VisualsFoldOut )
		{
			targetControlSurface.Model = EditorGUILayout.ObjectField("Model", targetControlSurface.Model, typeof(UnityEngine.GameObject), true) as UnityEngine.GameObject;
			
			if ( targetControlSurface.Model )
			{
				targetControlSurface.ModelRotationAxis = EditorGUILayout.Vector3Field( "Model Rotation Axis", targetControlSurface.ModelRotationAxis );		
			}
		}
		
		
		//User help....
		if ( targetControlSurface.Controller.AxisName.Length == 0 )
		{
			EditorGUILayout.HelpBox( "No input axis defined.", MessageType.Error );
		}
		
		if ( !targetControlSurface.Model )
		{
			EditorGUILayout.HelpBox( "No model attached for visual rotation.", MessageType.Warning );
		}
		
    	
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetControlSurface);
    }
}	
		