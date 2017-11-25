using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(GUITexture))]
public class CrossHair : MonoBehaviour
{
	public Transform FFPoint;
	public VehicleCamera tankCamera;
	public Camera MainCamera;

	public GUITexture FireCrossGUI;
	public Vector3 Target;

	void Start ()
	{
		FireCrossGUI = this.GetComponent<GUITexture> ();
		CaculateGuiTextureScale (FireCrossGUI, (Screen.height / 768f) * 50f);
	}
	
	void Update ()
	{
		Target = HitArea ();
		Vector3 ViewportPoint = MainCamera.WorldToViewportPoint (Target);
		if (ViewportPoint.z > 0) {
			FireCrossGUI.enabled = true;
			FireCrossGUI.transform.position = ViewportPoint;
		} else {
			FireCrossGUI.enabled = false;
		}
	}

	public Vector3 HitArea ()
	{
        return FFPoint.forward * 1000 + FFPoint.position;

        if (RayManager.Instance.IsHitFFPoint) {
            if (tankCamera.IsOpenFireCross)
				return FFPoint.forward * 1000 + FFPoint.position;
			else
				return RayManager.Instance.RayHitInfoFFPoint.point + FFPoint.forward;
		} else {
			return FFPoint.forward * 1000 + FFPoint.position;
		}
	}

	public void CaculateGuiTextureScale (GUITexture guiTexture, float Size)
	{
		guiTexture.pixelInset = new Rect (-Size / 2, -Size / 2, Size, Size);

	}
}
