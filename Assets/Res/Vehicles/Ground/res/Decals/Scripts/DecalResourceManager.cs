using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DecalResourceManager : MonoBehaviour {

	public Texture[] Decals;

	public static Dictionary<string,Texture> DecalArray = new Dictionary<string,Texture>  ();
	void Start(){
		if(DecalArray.Count ==0)
			foreach (Texture t in Decals) {
				DecalArray.Add (t.name,t);
			}
	}
}
