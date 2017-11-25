using UnityEngine;
using System.Collections;

public class VectorOperator
{
	public static Vector3 localPosition(Transform Parent, Vector3 Child)
	{
		Vector3 localPosition = Child - Parent.position;
		return new Vector3( Vector3.Dot(localPosition, Parent.right),
		                    Vector3.Dot(localPosition, Parent.up),
		                    Vector3.Dot(localPosition, Parent.forward)
		                   );
	}
}
