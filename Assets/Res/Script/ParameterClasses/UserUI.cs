using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

[System.Serializable]
public class UserUI
{
	public Text TeamA,TeamB;
	public Text HealthUI,GameNote,FireState;
	public EventTrigger MG;
	public Text DamageToolBar,DestroyToolBar;
	public Joystick CameraJoyStick;
	public Image ReloadBar,RightBarFill;
	public Transform EventCapture,EventRestore,EventRepair,Message;
	public Text CaptureProgress,RestoreProgress,RepairProgress,MessageContent;
}