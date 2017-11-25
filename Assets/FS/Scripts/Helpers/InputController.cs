//
// UnityFS - Flight Simulation Toolkit. Copyright 2013 Chris Cheetham.
//
using UnityEngine;


#if !NETFX_CORE
using System.Reflection;
#endif

[System.Serializable]
public class InputController
{
	public enum InputSource 
	{
		InputManager = 0,
		Reflected,
		Manual
	}
	
	[SerializeField]
	public InputSource Source = InputSource.InputManager;
	[SerializeField]
	public bool Invert = false;
	
	//Input manager vars
	[SerializeField]
	public string AxisName = "";
	public string ButtonName = "";
	
	//Reflection vars
	[SerializeField]
	public GameObject ReflectedGameObject = null;
	[SerializeField]
	public Component ReflectedComponent = null;
	[SerializeField]
	public int ReflectedComponentIndex = 0;
	[SerializeField]
	public int ReflectedFieldIndex = 0;
	[SerializeField]
	public int ReflectedFieldVector2Axis = 0;
	[SerializeField]
	public int ReflectedFieldVector3Axis = 0;
	[SerializeField]
	public System.Type ReflectedFieldType;
	[SerializeField]
	public string ReflectedFieldName = "";
	 
	//Manual vars
	private float ManualInput = 0.0f;
	private bool ManualButtonPressed = false;
	private bool ButtonPressedLastFrame = false;
	private bool ButtonWasPressedThisFrame = false;
		
	public void SetInputSource( InputSource newSource )
	{
		Source = newSource;
	}
	 
	public void SetManualInputMinusOneToOne( float input )
	{
		ManualInput = Mathf.Clamp( input, -1.0f, 1.0f );
		
		if (  InputSource.Manual != Source )
		{
			Debug.LogWarning( "InputController - Setting manual input when not in manual input mode." );
		}
	}
	
	public void SetManualInputButtonPressed( bool isPressed )
	{
		ManualButtonPressed = isPressed;
	}
	
	public float GetAxisInput()
	{
		float inputValue = 0.0f;
		
		switch ( Source )
		{
			case InputSource.InputManager:
			{
				if ( "" != AxisName )
				{
					inputValue = Input.GetAxis( AxisName );
				}
				else
				{
					Debug.LogWarning( "InputController - Trying to get input from InputManager with no axis defined." );
				}
			}
			break;
			
			case InputSource.Reflected:
			{
#if !NETFX_CORE
				if (ReflectedComponent )
				{
					if( ReflectedFieldName != "" )
					{
						FieldInfo fieldInfo = ReflectedComponent.GetType().GetField(ReflectedFieldName);
						if ( null != fieldInfo )
						{
							System.Type fieldType = fieldInfo.FieldType;
							
							if ( fieldType == typeof(float) )
							{
								//Float so use as is..
								inputValue = (float)fieldInfo.GetValue(ReflectedComponent);
							}
							else if ( fieldType == typeof(Vector2) )
							{
								//Vector2 so use right axis..
								Vector2 reflectedValue = (Vector2)fieldInfo.GetValue(ReflectedComponent);
								inputValue = reflectedValue[ReflectedFieldVector2Axis];
							}
							else if ( fieldType == typeof(Vector3) )
							{
								Vector3 reflectedValue = (Vector3)fieldInfo.GetValue(ReflectedComponent);
								inputValue = reflectedValue[ReflectedFieldVector3Axis];
							}
						}
						
					}	
				}
#endif
			}
			break;
			
			case InputSource.Manual:
			{
				inputValue = ManualInput;
			}
			break;
		}
		
		inputValue = Mathf.Clamp( inputValue, -1.0f, 1.0f );
		
		if ( Invert )
		{
			inputValue = -inputValue;
		}	
		
		return inputValue;
	}
	
	public bool GetButton()
	{
		bool inputValue = false;
		
		switch ( Source )
		{
			case InputSource.InputManager:
			{
				if ( "" != ButtonName )
				{
					inputValue = Input.GetButton( ButtonName );
				}
				else
				{
					Debug.LogWarning( "InputController - Trying to get input from InputManager with no button defined." );
				}
			}
			break;
			
			case InputSource.Reflected:
			{
#if !NETFX_CORE
				if (ReflectedComponent )
				{
					if( ReflectedFieldName != "" )
					{
						FieldInfo fieldInfo = ReflectedComponent.GetType().GetField(ReflectedFieldName);
						if ( null != fieldInfo )
						{
							System.Type fieldType = fieldInfo.FieldType;
							
							if ( fieldType == typeof(bool) )
							{
								//Float so use as is..
								inputValue = (bool)fieldInfo.GetValue(ReflectedComponent);
							}
						}
						
					}	
				}
#endif
			}
			break;
			
			case InputSource.Manual:
			{
				inputValue = ManualButtonPressed;
			}
			break;
		}
		
		if ( Invert )
		{
			inputValue = !inputValue;
		}
		
		//Also support button pressed events not just holding..
		if ( !ButtonPressedLastFrame && inputValue )
		{
			ButtonWasPressedThisFrame = true;
		}
		else
		{
			ButtonWasPressedThisFrame = false;
		}
		
		ButtonPressedLastFrame = inputValue;
		
		return inputValue;
	}
	
	public bool GetButtonPressed()
	{
		//Needs to poll get button in order to check this is a bit dodgy really..
		//(Because there is no update for input controller)
		GetButton();
		return ButtonWasPressedThisFrame;
	}
	
	

}