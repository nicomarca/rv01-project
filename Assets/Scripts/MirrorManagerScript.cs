using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MirrorManagerScript : MonoBehaviour {

	private GameObject instantiateComparatorObject; // Instance of the comparator object
	private Vector3 comparatorObjectSize;			// Size of the comparator object
	private float distanceToObj;					// Distance between mirror and reference object
	private float distanceToPlayer = 10.0f;					// Distance between mirror and player
	private float size;								// Player size
	private float playerRatio;
	
	public GameObject comparatorObject; 			// Prefab of the comparator object
	public GameObject player;  						// Player


	void Start () {
		InstantiateComparatorObject ();
		playerRatio = 1.0f;
	}
	
	void Update () {
		distanceToObj = Vector3.Distance (transform.parent.position - new Vector3 (0, transform.parent.position.y, 0), instantiateComparatorObject.transform.position - new Vector3 (0, instantiateComparatorObject.transform.position.y, 0));
		distanceToPlayer = Vector3.Distance (transform.parent.position - new Vector3 (0, transform.parent.position.y, 0), player.transform.position - new Vector3 (0, player.transform.position.y, 0));
		//Debug.Log ("Distance to Obj :" + distanceToObj);
		//Debug.Log ("distanceToPlayer:" + distanceToPlayer);

	}

	void InstantiateComparatorObject(){
		size = player.GetComponent<CapsuleCollider> ().height;
		comparatorObjectSize = comparatorObject.transform.GetComponent<Renderer> ().bounds.size;
		float ratio = size / comparatorObjectSize.y;
		Vector3 objectPosition = gameObject.transform.position;
		objectPosition.y = ((comparatorObject.GetComponent<Renderer> ().bounds.size.y) * ratio) / 2;
		instantiateComparatorObject = Instantiate (comparatorObject, objectPosition, Quaternion.identity);
		instantiateComparatorObject.transform.Rotate (new Vector3 (90, 0, 0));
		instantiateComparatorObject.transform.localScale = new Vector3 (instantiateComparatorObject.transform.localScale.x * ratio, instantiateComparatorObject.transform.localScale.y * ratio, instantiateComparatorObject.transform.localScale.z * ratio);
	}

	private float newRatio()
	{
		float ratio = 1.0f;							// Ratio between size and comparator size
		if (SceneManager.GetActiveScene ().name == "JailScene") {
			Debug.Log ("ziehfizehfuhzefze");
			if (distanceToPlayer < 23) {

				if (distanceToPlayer < 3.5) {
					ratio = 3.70f;
					return ratio;
				} else if (distanceToPlayer < 6) {
					ratio = 1.7f;
					return ratio;
				} else if (distanceToPlayer < 8.9) {
					ratio = 1.0f;
					return ratio;
				} else if (distanceToPlayer < 14.3) {
					ratio = 0.57f;
					return ratio;
				} else if (distanceToPlayer < 17.9) {
					ratio = 0.42f;
					return ratio;
				} else if (distanceToPlayer < 22 ){
					ratio = 0.28f;
					return ratio;
				} else {
					ratio =  0.28f;
					return ratio;
				}
			} else {
				ratio = 1.0f;
				return ratio;
			}
		} else {
			if (distanceToPlayer < 25) {
				
				if (distanceToPlayer < 2) {
					ratio = 10.0f;
					return ratio;
				} else if (distanceToPlayer < 4) {
					ratio = 5.0f;
					return ratio;
				} else if (distanceToPlayer < 5.5) {
					ratio = 3.2f;
					return ratio;
				} else if (distanceToPlayer < 8) {
					ratio = 2.0f;
					return ratio;
				} else if (distanceToPlayer < 12) {
					ratio = 1.5f;
					return ratio;
				} else if (distanceToPlayer < 16) {
					ratio = 1.0f;
					return ratio;
				} else if (distanceToPlayer < 20) {
					ratio = 4.0f / 5.0f;
					return ratio;
				} else if (distanceToPlayer < 25) {
					ratio = 3.0f / 5.0f;
					return ratio;
				} else {
					ratio = 2.0f / 5.0f;
					return ratio;
				}
			} else {
				ratio = 1.0f;
				return ratio;
			}
		}
	}

	// TODO
	public float newPlayerSize(){

		float newRatioPlayerObject = newRatio ();
		float oldRatioPlayer = playerRatio;
		playerRatio *= newRatioPlayerObject;
		/*
		if (SceneManager.GetActiveScene ().name == "JailScene") {
			if ((playerRatio <= 0.1f && playerRatio < oldRatioPlayer) || (playerRatio > 2.0f && playerRatio > oldRatioPlayer)) {
				return 1.0f;
			} else {
				return newRatioPlayerObject;
			}
		}else {
			if ((playerRatio <= 0.2f && playerRatio < oldRatioPlayer) || (playerRatio > 5.0f && playerRatio > oldRatioPlayer)) {
				return 1.0f;
			} else {
				return newRatioPlayerObject;
			}
		}
		*/
		return newRatioPlayerObject;
	
	}

}
