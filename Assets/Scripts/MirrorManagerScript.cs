using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorManagerScript : MonoBehaviour {

	private GameObject instantiateComparatorObject; // Instance of the comparator object
	private Vector3 comparatorObjectSize;			// Size of the comparator object
	private float distanceToObj;					// Distance between mirror and reference object
	private float distanceToPlayer;					// Distance between mirror and player
	private float ratio;							// Ratio between size and comparator size
	private float size;								// Player size
	
	public GameObject comparatorObject; 			// Prefab of the comparator object
	public GameObject player;  						// Player


	void Start () {
		InstantiateComparatorObject ();
	}
	
	void Update () {
		distanceToObj = Vector3.Distance (transform.parent.position - new Vector3 (0, transform.parent.position.y, 0), instantiateComparatorObject.transform.position - new Vector3 (0, instantiateComparatorObject.transform.position.y, 0));
		distanceToPlayer = Vector3.Distance (transform.parent.position - new Vector3 (0, transform.parent.position.y, 0), player.transform.position - new Vector3 (0, player.transform.position.y, 0));
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

	// TODO
	public float newPlayerSize(){
		if (distanceToObj < 25) {
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
