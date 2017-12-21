using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class WandController : MonoBehaviour
{
	public bool VR;
	public Vector3 old_position;
	public Quaternion old_rotation;
	public GameObject cam;

	void Start ()
	{
		old_position = transform.localPosition;
		old_rotation = transform.localRotation;
	}
	
	void Update () {
		if (VR) {
			transform.localPosition = old_position+ InputTracking.GetLocalPosition (VRNode.RightHand);
			transform.localRotation = old_rotation* InputTracking.GetLocalRotation (VRNode.RightHand);
		} else {
			//position
			transform.localPosition = new Vector3 (cam.transform.localPosition.x + 0.7f, cam.transform.localPosition.y - 0.4f, cam.transform.localPosition.z + 1f);

			// rotation
			Vector3 targetDir = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x + 10, Input.mousePosition.y, 10f)) - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 100, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
		}
	}
}
