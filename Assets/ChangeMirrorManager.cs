using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMirrorManager : MonoBehaviour {


	public GameObject jailMirror;
	public GameObject hallMirror;
	public GameObject player;

	private bool isJailMirrorUsed;

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
				player.GetComponent<RayCastingController> ().mirrorManager = hallMirror;
				isJailMirrorUsed = false;
			} else {
				player.GetComponent<RayCastingController> ().mirrorManager = jailMirror;
				isJailMirrorUsed = true;
			}
		}
	}
}
