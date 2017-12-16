using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailScenarioScript : MonoBehaviour {

	public Light light;
	public GameObject wand;
	public GameObject player;

	private float timer;
	private bool flashDone = false;
	private bool flashStabilized = false;

	void Start () {
		timer = 0.0f;
	}

	void Update () {
		timer += Time.deltaTime;

		if (timer > 2.0) {
			if (!flashDone) {
				light.intensity += 3.0f;
				if (light.intensity > 12) {
					flashDone = true;
					wand.SetActive (true);
					player.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
					player.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezePositionY;
				}
			} else {
				if (!flashStabilized) {
					light.intensity -= 0.1f;
					if (light.intensity < 3) {
						flashStabilized = true;
					}
				}
			}

		}
	}
}
