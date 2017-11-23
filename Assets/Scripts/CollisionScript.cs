using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour {

	// TODO: faire un tableau de Collision pour gérer les cas où plusieurs collisions en même temps
	// Doit tenir le tableau à jour
	// A la fin dans la fonction OnDestroy, parcourir toutes les collisions

	private GameObject	fpsCharacter;
	private bool 		wasAlreadyFreeze = false;
	private Collision	coll;

	// Use this for initialization
	void Start () {
		fpsCharacter = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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

	/** Quand attachedObject est lâché en collision avec un autre objet, OnDestroy() est appelé
	 * (et non OnCollisionExit). Il faut alors libérer tous les objets en collision
	 **/
	void OnDestroy(){
		if (coll != null) {
			if (coll.gameObject.GetComponent<Rigidbody> () != null) {
				if (!wasAlreadyFreeze) {
					coll.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				}
				fpsCharacter.GetComponent<RayCastingController> ().setAttachedObjectCollision (null);
			}
		}
	}

}
