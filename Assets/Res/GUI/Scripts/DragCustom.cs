using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragCustom : MonoBehaviour, IPointerDownHandler, IDragHandler {
    public delegate void HanldePlayerModifyControlPostion();
    public event HanldePlayerModifyControlPostion onPlayerModifyControlPostion;

	private RectTransform panelRectTransform;
	bool EnableCustomUI =false;
	public bool InSetting = false;

	void Awake () {
		panelRectTransform = transform as RectTransform;
		Load ();
		if (InSetting) {
			EnableCustomUI =true;
		}
		onPlayerModifyControlPostion += Load;
	}

	public void OnPointerDown (PointerEventData data) {

	}
	public void OnDestroy(){
		onPlayerModifyControlPostion -= Load;
	}
	public void OnDrag (PointerEventData data) {
		if (EnableCustomUI) {
			panelRectTransform.anchoredPosition += data.delta;
			Save ();
		}
	}
	public void Save(){
		PlayerPrefs.SetString (panelRectTransform.name+"UI", "D");
		PlayerPrefs.SetFloat (panelRectTransform.name + "x", panelRectTransform.anchoredPosition.x);
		PlayerPrefs.SetFloat (panelRectTransform.name + "y", panelRectTransform.anchoredPosition.y);
        onPlayerModifyControlPostion();

	}
	public void Load(){
		if (PlayerPrefs.HasKey (panelRectTransform.name + "UI")) {
			Debug.Log ("Loaded");
			panelRectTransform.anchoredPosition = new Vector2 (PlayerPrefs.GetFloat(panelRectTransform.name + "x"),PlayerPrefs.GetFloat(panelRectTransform.name + "y"));
		}
	}
}
