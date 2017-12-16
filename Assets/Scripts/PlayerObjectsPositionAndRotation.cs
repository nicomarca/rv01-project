using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

/** This script manages the boots and camera placement. The boots have to stay under the camera (which is free from the empty game object player in VR),
 * same X and Y as the hat but at the same time have to stay on the floor.
 * The position of the camera is handled in case we are not in VR.
 **/

public class PlayerObjectsPositionAndRotation : MonoBehaviour {

	public bool VR;
	public GameObject wand;
	public GameObject boots;
	public GameObject cam;

	private Vector3 old_wand_position;
	private Quaternion old_wand_rotation;

	private Vector3 old_cam_position;

	void Start () {
		old_cam_position = cam.transform.position;

		old_wand_position = wand.transform.localPosition;
		old_wand_rotation = wand.transform.localRotation;
	}
	
	void Update () {
		cameraPlacement ();
		bootsPlacement ();
		wandPlacement ();
	}

	void cameraPlacement(){
		if (!VR) {
			cam.transform.position = new Vector3 (transform.position.x, GetComponent<CapsuleCollider> ().bounds.max.y - 0.5f, transform.position.z);
		}
	}

	void bootsPlacement(){
		boots.transform.position = new Vector3 (cam.transform.position.x, GetComponent<CapsuleCollider>().bounds.min.y, cam.transform.position.z);
		boots.transform.rotation = new Quaternion (0, cam.transform.rotation.y, 0, 0);
	}

	void wandPlacement(){
		if (VR) {
			wand.transform.localPosition = old_wand_position+ InputTracking.GetLocalPosition (VRNode.RightHand);
			wand.transform.localRotation = old_wand_rotation* InputTracking.GetLocalRotation (VRNode.RightHand);
		} else {
			//position
			wand.transform.localPosition = new Vector3 (cam.transform.localPosition.x + 0.7f, cam.transform.localPosition.y - 0.4f, cam.transform.localPosition.z + 1f);

			// rotation
			Vector3 targetDir = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x + 10, Input.mousePosition.y, 10f)) - wand.transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 100, 0.0f);
			wand.transform.rotation = Quaternion.LookRotation (newDir);
		}
	}
}
