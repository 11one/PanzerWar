using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInput : MonoBehaviour
{
	public static MobileInput _Instance;

	public static bool isInZoom =false;
	public static Vector2 DragForce = Vector2.zero;
	public static Vector2 MoveDelta;
	private Vector2 ETCTouchMoveDelta,ButtonTouchMoveDelta;


	public bool IgnoreLayers =false;

	void Awake()
	{
		_Instance = this;
	}



	public void Regiseter()
	{
		MobileInputPanel.Instance.GetComponent<ETCTouchPad> ().onMove.AddListener(onMove);
		MobileInputPanel.Instance.GetComponent<ETCTouchPad> ().onMoveStart.AddListener(onStart);
		MobileInputPanel.Instance.GetComponent<ETCTouchPad> ().onMoveEnd.AddListener(onStop);
	}
	void Update(){
		MoveDelta = ETCTouchMoveDelta;
	}

	public void onMove(Vector2 Dir){
		ETCTouchMoveDelta = Dir;
	}
	public void onStart(){
		ETCTouchMoveDelta = Vector2.zero;
	}

	public void onStop(){
		ETCTouchMoveDelta = Vector2.zero;
	}


}

