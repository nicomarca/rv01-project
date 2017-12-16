using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;


public class FPSdeplacement : MonoBehaviour {

	public float tSpeed;	// Translation speed
	public float rSpeed;	// Rotation speed

	private float maxSpeed;

	public bool VR;

	void Start () {
		maxSpeed = tSpeed; // Limit the speed
	}
	
	void Update () {
		if (tSpeed > maxSpeed)
			tSpeed = maxSpeed;

		if (VR) {
			float mouvmentHorizontal = Input.GetAxis ("Horizontal");
			float mouvmentVertical = Input.GetAxis ("Vertical");
			Vector3 mouvment = new Vector3 (mouvmentHorizontal, 0, mouvmentVertical);
			//this.transform.Translate (Camera.main.transform.rotation * mouvment * tSpeed);
			transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*mouvment * tSpeed);
		} else {
			if (Input.GetKey ("z")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.forward * tSpeed);
			}
			if (Input.GetKey ("s")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.back * tSpeed);
			}
			if (Input.GetKey ("q")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.left * tSpeed);
			}
			if (Input.GetKey ("d")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.right * tSpeed);
			}

			if (Input.GetKey ("left")) {
				transform.Rotate (Vector3.up, -rSpeed);
			}
			if (Input.GetKey ("right")) {
				transform.Rotate (Vector3.up, rSpeed);
			}
			if (Input.GetKey ("up")) {
				transform.Rotate (Vector3.right, -rSpeed);
			}
			if (Input.GetKey ("down")) {
				transform.Rotate (Vector3.right, rSpeed);
			}
		}
	}
}
