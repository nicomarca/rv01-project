using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

	private GameObject fpsCharacter;

	// Use this for initialization
	void Start () {
		fpsCharacter = GameObject.Find ("FirstPersonCharacter");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		/*
		if (collision.gameObject.transform.CompareTag("draggable")) {
			collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
			fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (collision);
		}
		*/
	}

	void OnCollisionStay(Collision collision) {

	}

	void OnCollisionExit(Collision collision) {
		/*
		if (collision.gameObject.transform.CompareTag("draggable")) {
			collision.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (null);
		}
		*/
	}

}
