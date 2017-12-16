using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class JailScenarioScript : MonoBehaviour {

	public Light light;
	public GameObject displayedWand;
	public GameObject playerWand;
	public GameObject stick;
	public GameObject player;

	private float timer;
	private bool flashDone = false;
	private bool flashStabilized = false;
	private bool wandTaken = false;

	void Start () {
		timer = 0.0f;
	}

	void Update () {
		timer += Time.deltaTime;

		flashLight ();

		if (!wandTaken){
			if (Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")){
				Debug.Log (Vector3.Distance (displayedWand.transform.position, stick.transform.position));
				if (Vector3.Distance (displayedWand.transform.position, stick.transform.position) < 2.5f) {
					displayedWand.SetActive (false);
					stick.SetActive (false);

					player.GetComponent<RayCastingController> ().enabled = true;
					playerWand.SetActive (true);
					wandTaken = true;

					light.intensity = 0;
				}
			}
		}
	}

	void flashLight(){
		if (timer > 2.0) {
			if (!flashDone) {
				light.intensity += 3.0f;
				if (light.intensity > 12) {
					flashDone = true;
					displayedWand.SetActive (true);
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
