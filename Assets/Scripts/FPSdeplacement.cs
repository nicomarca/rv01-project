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

	private List<Rigidbody> collisions;

	private IEnumerator coroutine;

	void Start () {
		maxSpeed = tSpeed; // Limit the speed
		jump = new Vector3(0.0f, 3.0f, 0.0f);
		isGrounded = true;

		collisions = new List<Rigidbody> ();

		if (VR) {
			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX;
			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationZ;
		}

	}
	
	void Update () {
		Vector3 horizontalAngle = new Vector3 (0, transform.rotation.eulerAngles.y, 0);
		Quaternion horizontalQuat = Quaternion.Euler (horizontalAngle);
		isMoving = false;

		//if (VR){
			Vector3 rot = new Vector3(0, transform.rotation.eulerAngles.y, 0);
			transform.rotation = Quaternion.Euler (rot);
		//}

		if (tSpeed > maxSpeed)
			tSpeed = maxSpeed;

		if (VR) {
			horizontalAngle = new Vector3 (0, Camera.main.transform.rotation.eulerAngles.y, 0);
			horizontalQuat = Quaternion.Euler (horizontalAngle);

			float mouvmentHorizontal = Input.GetAxis ("Horizontal");
			float mouvmentVertical = Input.GetAxis ("Vertical");

			Vector3 mouvment = new Vector3 (mouvmentHorizontal, 0, mouvmentVertical);
			transform.GetComponent<Rigidbody>().MovePosition(transform.position + horizontalQuat*mouvment * tSpeed);

			if (mouvment != Vector3.zero) {
				isMoving = true;
			}

		} else {
			horizontalAngle = new Vector3 (0, transform.rotation.eulerAngles.y, 0);
			horizontalQuat = Quaternion.Euler (horizontalAngle);
		
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

		if(Input.GetButtonDown ("Jump") && isGrounded) {
			if (coroutine != null) {
				StopCoroutine (coroutine);
			}
			transform.GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
			isGrounded = false;
			coroutine = waitForEndOfJumps (3);
			StartCoroutine (coroutine);
		}

		if (isGrounded == false) {
			isMoving = true;
		} 


		else if (isMoving == false) {
			//GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
	}


	private IEnumerator waitForEndOfJumps(float x){
		yield return new WaitForSeconds (x);
		Vector3 rot = new Vector3(0, transform.rotation.eulerAngles.y, 0);
		transform.rotation = Quaternion.Euler (rot);
		isGrounded = true;
	}


	void OnTriggerEnter() {
		isGrounded = true;
		if (coroutine != null) {
			StopCoroutine (coroutine);
		}
	}

	void OnTriggerExit() {
		isGrounded = false;
	}


	void OnCollisionEnter(Collision collision){
		/* prevent objects from moving when collision with player */
		if (collision.gameObject.GetComponent<Rigidbody> () != null) {
			if (!collisions.Contains (collision.gameObject.GetComponent<Rigidbody> ())) {
				collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
				collisions.Add (collision.gameObject.GetComponent<Rigidbody> ());
			}
		}
	}

	void OnCollisionExit(Collision collision){
		/* freeze the player so he does not bump into objects */
		if (isGrounded) {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}

		/* unfreeze the frozen objetcs colliding with player */
		if (collision.transform.GetComponent<Rigidbody> () != null) {
			collisions.Remove (collision.gameObject.GetComponent<Rigidbody> ());
			collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		}
	}
}
