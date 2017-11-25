//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//

using UnityEditor;
using UnityEngine;     

[CustomEditor(typeof(GroundEffect))]
public class GroundEffectEditor : Editor 
{
	private GroundEffect targetGroundEffect = null;
	
	private void OnEnable()
	{
		targetGroundEffect = target as GroundEffect;
	}
	
	public override void OnInspectorGUI() 
	{
		//Add default keyframes.
		if ( 0 == targetGroundEffect.CLHeightVsChord.keys.Length )
		{
			targetGroundEffect.CLHeightVsChord.AddKey( 0.0f, 1.6f );
			targetGroundEffect.CLHeightVsChord.AddKey( 0.4f, 1.2f );
			targetGroundEffect.CLHeightVsChord.AddKey( 1.0f, 1.0f );	
		}
		
		if ( 0 == targetGroundEffect.CDHeightVsSpan.keys.Length )
		{
			targetGroundEffect.CDHeightVsSpan.AddKey( 0.0f, 0.2f );
			targetGroundEffect.CDHeightVsSpan.AddKey( 0.4f, 0.8f );
			targetGroundEffect.CDHeightVsSpan.AddKey( 1.0f, 1.0f );	
		}
		
		base.OnInspectorGUI();
	}
	
}	
		