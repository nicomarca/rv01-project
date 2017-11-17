using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculBase {

	private GameObject hitObject;
	private Vector3 center;

	public CalculBase(GameObject gameObject, Vector3 vector) {
		this.hitObject = gameObject;
		this.center = vector;
	}

	public Vector3[] getBasePoints() {
		Vector3 p1 = new Vector3 ();
		Vector3 p2 = new Vector3 ();
		Vector3 p3 = new Vector3 ();
		Vector3 p4 = new Vector3 ();

		p1.x = center.x - Mathf.Cos (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);
		p1.y = center.y - (hitObject.transform.lossyScale.y)/2;
		p1.z = center.x - Mathf.Sin (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);

		p2.x = center.x + Mathf.Cos (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);
		p2.y = center.y - (hitObject.transform.lossyScale.y)/2;
		p2.z = center.x - Mathf.Sin (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);

		p3.x = center.x + Mathf.Cos (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);
		p3.y = center.y - (hitObject.transform.lossyScale.y)/2;
		p3.z = center.x + Mathf.Sin (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);

		p4.x = center.x - Mathf.Cos (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);
		p4.y = center.y - (hitObject.transform.lossyScale.y)/2;
		p4.z = center.x + Mathf.Sin (hitObject.transform.rotation.y) * ((hitObject.transform.lossyScale.x)/2);

		Vector3[] result = new Vector3[4];
		result [0] = p1;
		result [1] = p2;
		result [2] = p3;
		result [3] = p4;
		return result;
	}
}
