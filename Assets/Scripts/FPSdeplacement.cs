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

	void Start () {
		maxSpeed = tSpeed; // Limit the speed
		jump = new Vector3(0.0f, 3.0f, 0.0f);
		isGrounded = true;

	}
	
	void Update () {
		isMoving = false;

		if (tSpeed > maxSpeed)
			tSpeed = maxSpeed;

		if (VR) {
			float mouvmentHorizontal = Input.GetAxis ("Horizontal");
			float mouvmentVertical = Input.GetAxis ("Vertical");
			Vector3 mouvment = new Vector3 (mouvmentHorizontal, 0, mouvmentVertical);
			//this.transform.Translate (Camera.main.transform.rotation * mouvment * tSpeed);
			transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*mouvment * tSpeed);
			if(Input.GetButtonDown ("Jump"))
			{
				transform.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
				isGrounded = false;
			}

		} else {
			if (Input.GetKey ("z")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.forward * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("s")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.back * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("q")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.left * tSpeed);
				isMoving = true;
			}
			if (Input.GetKey ("d")) {
				transform.GetComponent<Rigidbody>().MovePosition(transform.position + Camera.main.transform.rotation*Vector3.right * tSpeed);
				isMoving = true;
			}
			if(Input.GetKeyDown(KeyCode.Space) && isGrounded){

				transform.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
				isGrounded = false;
				isMoving = true;
			}

			if (!isGrounded) {
				isMoving = true;
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

			if (isMoving == false && isGrounded == false) {

				Debug.Log ("Coucou je suis là");
				GetComponent<Rigidbody> ().velocity = Vector3.zero;
				GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			} else {

				Debug.Log ("Coucou je suis pas là");

			}
		}
	}

	void OnCollisionStay(Collision collision)
	{
		
		isGrounded = true;
		Debug.Log (collision.transform.name);
	}
}
