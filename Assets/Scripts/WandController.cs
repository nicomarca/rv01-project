using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class WandController : MonoBehaviour
{
	public bool VR;

	void Start () {
		
	}
	
	void Update () {
		if (VR) {
			transform.rotation = InputTracking.GetLocalRotation (VRNode.RightHand);
			transform.Rotate (new Vector3 (90, 0, 0));
		} else {
			Vector3 targetDir = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x + 10, Input.mousePosition.y, 10f)) - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 100, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
			transform.Rotate (new Vector3 (90, 0, 0));
		}
	}
}
