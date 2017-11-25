//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
    

[CustomEditor(typeof(Aerofoil))]
public class AerofoilEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		Aerofoil myTarget = (Aerofoil) target;
		
    	myTarget.CL = EditorGUILayout.CurveField( "CL", myTarget.CL );
		myTarget.CD = EditorGUILayout.CurveField( "CD", myTarget.CD );
		myTarget.CM = EditorGUILayout.CurveField( "CM", myTarget.CM );

    }
}
		