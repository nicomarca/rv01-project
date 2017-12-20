using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

	private GameObject	fpsCharacter;					// First person character
	private bool 		wasAlreadyFreeze = false;		// Boolean to know if the object was already frozen
	private Collision	coll;							// Collision object

	void Start () {
		fpsCharacter = GameObject.Find ("Player");
	}
	
	void Update () {
		
	}

	/** 
	 * Called when the collision starts
	 **/
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Rigidbody> () != null) {
			coll = collision;
			if (collision.collider.GetComponent<Rigidbody> ().constraints == RigidbodyConstraints.FreezeAll) {
				wasAlreadyFreeze = true;
			} else {
				collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
				fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (collision);
				wasAlreadyFreeze = false;
			}
		}
	}

	void OnCollisionStay(Collision collision) {

	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.GetComponent<Rigidbody> () != null) {
			coll = null;
			if (!wasAlreadyFreeze) {
				collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			}
			fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (null);
		}
	}

	/**
	 * OnDestroy is called when an object is released in collision with another object
	 * (and not OnCollisionExit()). All collided objects must be free
	 **/
	void OnDestroy() {
		if (coll != null) {
			if (coll.gameObject.GetComponent<Rigidbody> () != null) {
				if (!wasAlreadyFreeze) {
					coll.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				}
				fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (null);
				// Cancel all the forces after a short time so that the objects don't move anymore
				fpsCharacter.GetComponent<RayCastingController> ().preventMovingAfter (coll.gameObject.GetComponent<Rigidbody> (), 0.1f);
				fpsCharacter.GetComponent<RayCastingController> ().preventMovingAfter (coll.gameObject.GetComponent<Rigidbody> (), 0.5f);
				fpsCharacter.GetComponent<RayCastingController> ().preventMovingAfter (GetComponent<Rigidbody> (), 0.1f);
			}
		}
	}
}
