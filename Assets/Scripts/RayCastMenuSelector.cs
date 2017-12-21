using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using Valve.VR;


public class RayCastMenuSelector : MonoBehaviour {
	
	private float RAYCASTLENGTH = 1000f;
	private RaycastHit menuSelector;

	public bool VR;
	private Vector3 old_position;
	private Quaternion old_rotation;

	public GameObject wand;
	public GameObject lazer;

	public Material lazerOff, lazerOK, lazerOn;	

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray (wand.transform.position, wand.transform.up);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);

		if (Physics.Raycast (ray.origin, ray.direction, out menuSelector, RAYCASTLENGTH)){

			if (menuSelector.transform.name == "Exit" || menuSelector.transform.name == "Skip Tutorial" || menuSelector.transform.name == "Start") {
				lazer.GetComponent<Renderer> ().material = lazerOn;

				if (Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")) {
					lazer.GetComponent<Renderer> ().material = lazerOK;
					menuSelector.transform.GetComponent<Button> ().onClick.Invoke ();
				}
			} else {
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}

		}
			
	}
}
