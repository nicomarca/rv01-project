using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunTriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


	void OnTriggerEnter(Collider other) {
		if (other.transform.name == "Sun") {
			other.transform.GetComponent<voiceManagerMainBack> ().PlayEnd ();
			GetComponent <BoxCollider>().enabled = false;
		}
	}
}