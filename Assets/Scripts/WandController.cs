using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class WandController : MonoBehaviour
{
	public bool VR;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (InputTracking.GetLocalPosition (VRNode.RightHand));
		//Debug.Log (InputTracking.GetLocalPosition (VRNode.LeftHand));
		if (VR) {
			//transform.localPosition = InputTracking.GetLocalPosition (VRNode.RightHand);
			transform.rotation = InputTracking.GetLocalRotation (VRNode.RightHand);

			transform.Rotate(new Vector3(90, 0, 0));
		} else {
			Vector3 targetDir = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x + 10, Input.mousePosition.y, 10f)) - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 100, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
			transform.Rotate(new Vector3(90, 0, 0));
		}
	}
}
