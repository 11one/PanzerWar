using UnityEngine;
using System.Collections;

public class uGUI_Item : MonoBehaviour {

	public GameObject UIObject;
	SpriteRenderer spriteRenderer;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	void Update () {
		if (GameEvent.Player) {
			UIObject.transform.LookAt (GameEvent.PlayerCamera.transform);
		}
	}
}
