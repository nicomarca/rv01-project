using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;


public class FPSdeplacement : MonoBehaviour {

	public float tSpeed;	// Translation speed
	public float rSpeed;	// Rotation speed
	public float jumpForce = 2.0f;


	public Vector3 jump;

	public bool isGrounded;


	private float maxSpeed;

	public bool VR;

	private bool isMoving = false;

	private int collisionCount = 0;




	void Start () {
		maxSpeed = tSpeed; // Limit the speed
		jump = new Vector3(0.0f, 3.0f, 0.0f);
		isGrounded = true;

	}
	
	void Update () {
		Vector3 horizontalAngle = new Vector3 (0, transform.rotation.eulerAngles.y, 0);
		Quaternion horizontalQuat = Quaternion.Euler (horizontalAngle);
		isMoving = false;

		if (tSpeed > maxSpeed)
			tSpeed = maxSpeed;

		if (VR) {
			float mouvmentHorizontal = Input.GetAxis ("Horizontal");
			float mouvmentVertical = Input.GetAxis ("Vertical");

			Vector3 mouvment = new Vector3 (mouvmentHorizontal, 0, mouvmentVertical);
			transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*mouvment * tSpeed);

			if (mouvment != Vector3.zero) {
				isMoving = true;
			}

			if(Input.GetButtonDown ("Jump")) {
				transform.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
				isGrounded = false;
			}

		} else {

			if (Input.GetKey ("z")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*Vector3.forward * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("s")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*Vector3.back * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("q")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*Vector3.left * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("d")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*Vector3.right * tSpeed);
				isMoving = true;
			}
			if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
				transform.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
				isGrounded = false;
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

		if (!isGrounded) {
			isMoving = true;
		}

		if (isMoving == false && isGrounded == false) {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
	}


	void OnCollisionEnter () {
		collisionCount++; 
		isGrounded = true;
	}


	void OnCollisionExit () {
		collisionCount--;
		isGrounded = false;

	}
}
