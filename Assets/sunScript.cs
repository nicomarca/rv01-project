using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunScript : MonoBehaviour {

	public GameObject player; 
	private bool sunSelected;

	// Use this for initialization
	void Start () {

		sunSelected = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.y < 30 && !sunSelected) {
		
			player.GetComponent<voiceManagerMainBack> ().PlayEnd ();
			sunSelected = true;
		}
		
	}
}
