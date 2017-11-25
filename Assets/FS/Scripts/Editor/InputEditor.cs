//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//
using UnityEngine;
using UnityEditor;
using System.Reflection; 

//Base class for any editor that needs to edit input - provides
//functionality for selecting input source.
public class InputEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
	
	protected void ShowInputAxisOptions( string name, InputController target )
	{
		if ( null != target )
		{
			//Always display the enum option for the input source.
			target.Source = (InputController.InputSource)EditorGUILayout.EnumPopup( name + " Input Source", target.Source );
			
			//EditorGUILayout.Space();
			
			//Show source specific options.
			switch ( target.Source )
			{
				case InputController.InputSource.InputManager:
				{
					ShowAxisInputManagerSettings( name, target );
				}
				break;
				
				case InputController.InputSource.Reflected:
				{
					ShowAxisReflectedSettings( name, target );
				}
				break;
				
				case InputController.InputSource.Manual:
				{
					ShowAxisManualSettings( name, target );
				}
				break;
			}
			
			//EditorGUILayout.Space();
			
			//Invert option.
			target.Invert = EditorGUILayout.Toggle( "Invert " + name, target.Invert );
			
		}
	}
		
	private void ShowAxisInputManagerSettings( string name, InputController target )
	{
		target.AxisName = EditorGUILayout.TextField( name + " Axis Name", target.AxisName );
	}
	
	private void ShowAxisReflectedSettings( string name, InputController target )
	{
		//Show game object picker
		GameObject lastGameObject = target.ReflectedGameObject;
		target.ReflectedGameObject = EditorGUILayout.ObjectField( name + " GameObject", target.ReflectedGameObject, typeof(GameObject), true) as GameObject;
		
		if ( lastGameObject != target.ReflectedGameObject )
		{
			//Game object has changed so null component.
			target.ReflectedComponent = null;
		}
		 
		//Show component picker..
		if ( null != target.ReflectedGameObject )
		{
			Component[] attachedComponents = target.ReflectedGameObject.transform.GetComponents<Component>();
			string[] attachedComponentNames = new string[attachedComponents.Length];
			for( int i=0; i<attachedComponents.Length; i++ )
			{
				attachedComponentNames[i] = attachedComponents[i].GetType().ToString();
			}
			
			int lastComponentIndex = target.ReflectedComponentIndex;
			
			target.ReflectedComponentIndex = Mathf.Clamp( target.ReflectedComponentIndex, 0, attachedComponents.Length-1 );
			target.ReflectedComponentIndex = EditorGUILayout.Popup( name + " Component", target.ReflectedComponentIndex, attachedComponentNames);
			target.ReflectedComponent = attachedComponents[target.ReflectedComponentIndex];
			
			if ( lastComponentIndex != target.ReflectedComponentIndex )
			{
				//Component has changed reset field index.
				target.ReflectedFieldIndex = 0;
				target.ReflectedFieldVector2Axis = 0;
				target.ReflectedFieldVector3Axis = 0;
			}
			
			if ( null != target.ReflectedComponent )
			{
				// get all public static fields
				FieldInfo[] fieldInfos;
				fieldInfos = target.ReflectedComponent.GetType().GetFields(BindingFlags.Instance | 
                       BindingFlags.Static |
                       BindingFlags.Public);

				
				//Loop through types and see how many are valid.
				int validTypesFound = 0;
				for( int i=0; i<fieldInfos.Length; i++ )
				{
					if ( fieldInfos[i].FieldType == typeof(float) ||
						  fieldInfos[i].FieldType == typeof(Vector2) ||
						  fieldInfos[i].FieldType == typeof(Vector3) )
					{
						validTypesFound++;
					}
				}
				
				//Loop back through and store all valid types in array.
				string[] validTypeNames = new string[validTypesFound];
				int validIterator = 0;
				for( int i=0; i<fieldInfos.Length; i++ )
				{
					if ( fieldInfos[i].FieldType == typeof(float) ||
						  fieldInfos[i].FieldType == typeof(Vector2) ||
						  fieldInfos[i].FieldType == typeof(Vector3) )
					{
						validTypeNames[validIterator] = fieldInfos[i].Name;
						validIterator++;
					}
				}
				
				target.ReflectedFieldIndex = Mathf.Clamp( target.ReflectedFieldIndex, 0, validTypesFound-1 );
				target.ReflectedFieldIndex = EditorGUILayout.Popup( name + " Field", target.ReflectedFieldIndex, validTypeNames);
				
				if ( (fieldInfos.Length > 0) && (validTypeNames.Length > 0) )
				{
					target.ReflectedFieldType = target.ReflectedComponent.GetType().GetField(validTypeNames[target.ReflectedFieldIndex]).FieldType;
					target.ReflectedFieldName = validTypeNames[target.ReflectedFieldIndex];
					
					if ( target.ReflectedFieldType == typeof(Vector2) )
					{
						string[] axis = new string[]{ "X", "Y" };
						target.ReflectedFieldVector2Axis = EditorGUILayout.Popup( name + " Vector2 Axis", target.ReflectedFieldVector2Axis, axis);
					}	
					else if ( target.ReflectedFieldType == typeof(Vector3) )
					{
						string[] axis = new string[]{ "X", "Y", "Z" };
						target.ReflectedFieldVector3Axis = EditorGUILayout.Popup( name + " Vector3 Axis", target.ReflectedFieldVector3Axis, axis);
					}	
				}
				else
				{
					target.ReflectedFieldName = "";
				}
			}
		}
		else
		{
			//Make sure everything else is null.
			target.ReflectedComponent = null;
			target.ReflectedComponentIndex = 0;
			target.ReflectedFieldIndex = 0;
			target.ReflectedFieldVector2Axis = 0;
			target.ReflectedFieldVector3Axis = 0;
		}
	}
	
	private void ShowAxisManualSettings( string name, InputController target  )
	{
		//Do nothing.
		EditorGUILayout.HelpBox("Use SetManualInputMinusOneToOne() to set", MessageType.Info);
	}
	
	
	protected void ShowInputButtonOptions( string name, InputController target )
	{
		if ( null != target )
		{
			//Always display the enum option for the input source.
			target.Source = (InputController.InputSource)EditorGUILayout.EnumPopup( name + " Input Source", target.Source );
			
			//EditorGUILayout.Space();
			
			//Show source specific options.
			switch ( target.Source )
			{
				case InputController.InputSource.InputManager:
				{
					ShowButtonInputManagerSettings( name, target );
				}
				break;
				
				case InputController.InputSource.Reflected:
				{
					ShowButtonReflectedSettings( name, target );
				}
				break;
				
				case InputController.InputSource.Manual:
				{
					ShowButtonManualSettings( name, target );
				}
				break;
			}
			
			//EditorGUILayout.Space();
			
			//Invert option.
			target.Invert = EditorGUILayout.Toggle( "Invert " + name, target.Invert );
		}
			
	}
	
	private void ShowButtonInputManagerSettings( string name, InputController target )
	{
		target.ButtonName = EditorGUILayout.TextField( name + " Button Name", target.ButtonName );
	}
		
	private void ShowButtonReflectedSettings( string name, InputController target )
	{
		//Show game object picker
		GameObject lastGameObject = target.ReflectedGameObject;
		target.ReflectedGameObject = EditorGUILayout.ObjectField( name + " GameObject", target.ReflectedGameObject, typeof(GameObject), true) as GameObject;
		
		if ( lastGameObject != target.ReflectedGameObject )
		{
			//Game object has changed so null component.
			target.ReflectedComponent = null;
		}
		 
		//Show component picker..
		if ( null != target.ReflectedGameObject )
		{
			Component[] attachedComponents = target.ReflectedGameObject.transform.GetComponents<Component>();
			string[] attachedComponentNames = new string[attachedComponents.Length];
			for( int i=0; i<attachedComponents.Length; i++ )
			{
				attachedComponentNames[i] = attachedComponents[i].GetType().ToString();
			}
			
			int lastComponentIndex = target.ReflectedComponentIndex;
			
			target.ReflectedComponentIndex = Mathf.Clamp( target.ReflectedComponentIndex, 0, attachedComponents.Length-1 );
			target.ReflectedComponentIndex = EditorGUILayout.Popup( name + " Component", target.ReflectedComponentIndex, attachedComponentNames);
			target.ReflectedComponent = attachedComponents[target.ReflectedComponentIndex];
			
			if ( lastComponentIndex != target.ReflectedComponentIndex )
			{
				//Component has changed reset field index.
				target.ReflectedFieldIndex = 0;
				target.ReflectedFieldVector2Axis = 0;
				target.ReflectedFieldVector3Axis = 0;
			}
			
			if ( null != target.ReflectedComponent )
			{
				// get all public static fields
				FieldInfo[] fieldInfos;
				fieldInfos = target.ReflectedComponent.GetType().GetFields(BindingFlags.Instance | 
                       BindingFlags.Static |
                       BindingFlags.Public);

				
				//Loop through types and see how many are valid.
				int validTypesFound = 0;
				for( int i=0; i<fieldInfos.Length; i++ )
				{
					if ( fieldInfos[i].FieldType == typeof(bool) )
					{
						validTypesFound++;
					}
				}
				
				//Loop back through and store all valid types in array.
				string[] validTypeNames = new string[validTypesFound];
				int validIterator = 0;
				for( int i=0; i<fieldInfos.Length; i++ )
				{
					if ( fieldInfos[i].FieldType == typeof(bool) )
					{
						validTypeNames[validIterator] = fieldInfos[i].Name;
						validIterator++;
					}
				}
				
				target.ReflectedFieldIndex = Mathf.Clamp( target.ReflectedFieldIndex, 0, validTypesFound-1 );
				target.ReflectedFieldIndex = EditorGUILayout.Popup( name + " Field", target.ReflectedFieldIndex, validTypeNames);
				
				if ( (fieldInfos.Length > 0) && (validTypeNames.Length > 0) )
				{
					target.ReflectedFieldType = target.ReflectedComponent.GetType().GetField(validTypeNames[target.ReflectedFieldIndex]).FieldType;
					target.ReflectedFieldName = validTypeNames[target.ReflectedFieldIndex];
				}
				else
				{
					target.ReflectedFieldName = "";
				}
			}
		}
		else
		{
			//Make sure everything else is null.
			target.ReflectedComponent = null;
			target.ReflectedComponentIndex = 0;
			target.ReflectedFieldIndex = 0;
			target.ReflectedFieldVector2Axis = 0;
			target.ReflectedFieldVector3Axis = 0;
		}
	}
		
	private void ShowButtonManualSettings( string name, InputController target )
	{
		//Do nothing.
		EditorGUILayout.HelpBox("Use SetManualInputButtonPressed() to set", MessageType.Info);
	}
		
}	
		