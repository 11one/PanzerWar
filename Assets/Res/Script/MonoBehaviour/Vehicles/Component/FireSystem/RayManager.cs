using UnityEngine;

public class RayManager : MonoBehaviour
{
	public static RayManager Instance;

	Transform FFPoint;

	public RaycastHit RayHitInfoFFPoint;

	public bool IsHitFFPoint = false;

	bool Inited = false;


	public void Init (Transform _FFPoint)
	{
		Instance = this;

		FFPoint = _FFPoint;

		Inited = true;
	}

	void Update ()
	{
		if (Inited) {
			int layerMask = 1 << LayerMask.NameToLayer ("ExternalHitBox") | 1 << LayerMask.NameToLayer ("Building") | 1 << LayerMask.NameToLayer ("Terrian");
            IsHitFFPoint = Physics.Raycast(FFPoint.position, FFPoint.forward, out RayHitInfoFFPoint, 500, layerMask);

			//IsHit = Physics.Raycast (game.position, FFPoint.forward, out RayHitInfo, 500, layerMask);
		}
	}
}
