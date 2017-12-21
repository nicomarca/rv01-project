using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerChestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider other) {
		if (other.transform.name == "Player") {
			other.transform.GetComponent<voiceManagerMainScene> ().PlayChestInstruction ();
			GetComponent<BoxCollider> ().enabled = false;
		}
	}
}
