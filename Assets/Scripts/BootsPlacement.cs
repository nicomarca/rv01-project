using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This script manages the boots placement. The boots have to stay under the camera (which is free from the empty game object player in VR),
 * same X and Y as the hat but at the same time have to stay on the floor.
 **/

public class BootsPlacement : MonoBehaviour {

	public GameObject player;
	private float playerHeight;

	void Start () {
		playerHeight = player.transform.position.y;
	}
	
	void Update () {
		float x = Camera.main.transform.position.x;
		float z = Camera.main.transform.position.z;

		float y = player.transform.position.y - playerHeight;

		transform.position = new Vector3 (x, y, z);

	}
}
