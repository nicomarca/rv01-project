using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMirrorManager : MonoBehaviour {


	public GameObject jailMirror;
	public GameObject hallMirror;
	public GameObject player;

	public bool isJailMirrorUsed;

	// Use this for initialization
	void Start () {
		isJailMirrorUsed = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.name == "Player") {
			if (isJailMirrorUsed) {
				hallMirror.GetComponent<MirrorManagerScript>().playerRatio = jailMirror.GetComponent<MirrorManagerScript>().playerRatio;
				player.GetComponent<RayCastingController> ().mirrorManager = hallMirror;
				isJailMirrorUsed = false;
				gameObject.GetComponent<Collider> ().enabled = false;


			} else {
				player.GetComponent<RayCastingController> ().mirrorManager = jailMirror;
				isJailMirrorUsed = true;
			}
		}
	}
}
