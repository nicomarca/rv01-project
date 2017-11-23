using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

	private GameObject	fpsCharacter;
	private bool 		wasAlreadyFreeze = false;

	// Use this for initialization
	void Start () {
		fpsCharacter = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Rigidbody> () != null) {
			if (collision.gameObject.GetComponent<Rigidbody> ().constraints == RigidbodyConstraints.FreezeAll) {
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
			if (!wasAlreadyFreeze) {
				collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			}
			fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (null);
		}
	}

}
