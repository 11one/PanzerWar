//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
     

[CustomEditor(typeof(Engine))]
public class EngineEditor : InputEditor 
{
	private bool InputFoldOut = true;
	private bool PropFoldOut = true;
	private bool EngineFoldOut = true;
	private bool AudioFoldOut = true;
	
	
	private HOEditorUndoManager UndoManager = null;
	private Engine targetEngine = null;
	
	private void OnEnable()
	{
		targetEngine = target as Engine;
 
		// Instantiate undoManager
		UndoManager = new HOEditorUndoManager( targetEngine, "Engine" );
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
			ShowInputAxisOptions( "Throttle", targetEngine.ThrottleController );
			
			EditorGUILayout.Space();	
	
			ShowInputButtonOptions( "Engine", targetEngine.EngineStartController );
	
			EditorGUILayout.Space();
		}
		
		PropFoldOut = EditorGUILayout.Foldout( PropFoldOut, "Propeller" );
		if ( PropFoldOut )
		{	
			targetEngine.AnimatedPropellerPivot = EditorGUILayout.ObjectField( "Animated Pivot", targetEngine.AnimatedPropellerPivot, typeof(UnityEngine.Transform), true) as UnityEngine.Transform;
			
			targetEngine.AnimatedPropellerPivotRotateAxis = EditorGUILayout.Vector3Field( "Rotate Axis", targetEngine.AnimatedPropellerPivotRotateAxis );
			
			targetEngine.SlowPropeller = EditorGUILayout.ObjectField( "Slow Propeller", targetEngine.SlowPropeller, typeof(UnityEngine.GameObject), true) as UnityEngine.GameObject;
			
			targetEngine.FastPropeller = EditorGUILayout.ObjectField( "Fast Propeller", targetEngine.FastPropeller, typeof(UnityEngine.GameObject), true) as UnityEngine.GameObject;
			
			targetEngine.RPMToUseFastProp = EditorGUILayout.FloatField( "RPM To Switch To Fast Prop", targetEngine.RPMToUseFastProp );
			
			EditorGUILayout.Space();	
		}
		
		EngineFoldOut = EditorGUILayout.Foldout( EngineFoldOut, "Engine" );
		if ( EngineFoldOut )
		{
			targetEngine.CurrentEngineState = (Engine.EngineState)EditorGUILayout.EnumPopup( "Current Engine State", targetEngine.CurrentEngineState );
		
			targetEngine.IdleRPM = EditorGUILayout.FloatField( "Idle RPM", targetEngine.IdleRPM );

			targetEngine.MaxRPM = EditorGUILayout.FloatField( "Max RPM", targetEngine.MaxRPM );
			
			targetEngine.ForceAtMaxRPM = EditorGUILayout.FloatField( "Force At Max RPM", targetEngine.ForceAtMaxRPM );
			
			targetEngine.PercentageForceAppliedVSAirspeedKTS = EditorGUILayout.CurveField( "Percentage Force Applied VS Airspeed KTS", targetEngine.PercentageForceAppliedVSAirspeedKTS );

			targetEngine.RPMToAddPerKTOfSpeed = EditorGUILayout.FloatField( "RPM To Add Per Knot Of Speed", targetEngine.RPMToAddPerKTOfSpeed );
			
			targetEngine.RPMLerpSpeed = EditorGUILayout.FloatField( "RPM Lerp Speed", targetEngine.RPMLerpSpeed );
			
			EditorGUILayout.Space();
		}
			
		AudioFoldOut = EditorGUILayout.Foldout( AudioFoldOut, "Audio" );
		if ( AudioFoldOut )
		{
			targetEngine.EngineStartClip = EditorGUILayout.ObjectField( "Engine Start Clip", targetEngine.EngineStartClip, typeof(UnityEngine.AudioClip), true) as UnityEngine.AudioClip;
			
			targetEngine.EngineRunClip = EditorGUILayout.ObjectField( "Engine Run Clip", targetEngine.EngineRunClip, typeof(UnityEngine.AudioClip), true) as UnityEngine.AudioClip;

			targetEngine.PitchAtIdleRPM = EditorGUILayout.FloatField( "Pitch At Idle RPM", targetEngine.PitchAtIdleRPM );
			
			targetEngine.PitchAtMaxRPM = EditorGUILayout.FloatField( "Pitch At Max RPM", targetEngine.PitchAtMaxRPM );
			
			EditorGUILayout.Space();
		}
		
        //Undo end..
		UndoManager.CheckDirty();
		EditorUtility.SetDirty (targetEngine);
    }
}	
		