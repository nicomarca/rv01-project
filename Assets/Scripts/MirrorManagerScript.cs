using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorManagerScript : MonoBehaviour {

	public GameObject comparatorObject; // prefab of the comparator Object
	public GameObject playerSkin;  // first person controller 
	// Use this for initialization

	private GameObject instantiateComparatorObject;

	private Vector3 skinSize;
	private Vector3 comparatorObjectSize;
	private float distanceToObj;
	private float distanceToPlayer;
	private float ratio;



	void Start () {
		InstantiateComparatorObject ();
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log (gameObject.transform.parent.name +" position " + gameObject.transform.parent.position);
		distanceToObj = Vector3.Distance (gameObject.transform.parent.position - new Vector3(0, gameObject.transform.parent.position.y, 0)  , instantiateComparatorObject.transform.position - new Vector3(0, instantiateComparatorObject.transform.position.y, 0));
		distanceToPlayer = Vector3.Distance (gameObject.transform.parent.position  - new Vector3(0, gameObject.transform.parent.position.y, 0) , playerSkin.transform.position - new Vector3(0, playerSkin.transform.position.y, 0));

		//Debug.Log (distanceToObj);
		//Debug.Log ("Distance to Player " + distanceToPlayer);


		
	}

	void InstantiateComparatorObject(){
	
		skinSize = playerSkin.GetComponent<Renderer>().bounds.size;
		comparatorObjectSize = comparatorObject.transform.GetComponent<Renderer> ().bounds.size;
		float ratio = skinSize.y / comparatorObjectSize.y;
		Vector3 objectPosition = gameObject.transform.position;
		objectPosition.y = ((comparatorObject.GetComponent<Renderer> ().bounds.size.y)*ratio) / 2;
		instantiateComparatorObject  =  Instantiate (comparatorObject, objectPosition, Quaternion.identity);
		instantiateComparatorObject.transform.Rotate(new Vector3(90, 0, 0));
		instantiateComparatorObject.transform.localScale = new Vector3 (instantiateComparatorObject.transform.localScale.x * ratio, instantiateComparatorObject.transform.localScale.y * ratio, instantiateComparatorObject.transform.localScale.z * ratio);

	}

	//TODO
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
			} else {
				ratio = 0.5f;

				return ratio;
			}
		} else {
			ratio = 1.0f;

			return ratio;

		}
	}
}
