using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputPanel : MonoBehaviour {
	public static MobileInputPanel Instance;

	void Start () {
		Instance = this;
		MobileInput._Instance.Regiseter ();
	}

}
