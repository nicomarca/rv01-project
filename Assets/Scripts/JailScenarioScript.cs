using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;


/** This class handles the jail script :
 * apparition of the wand
 * flash of light
 * ...
 * */

public class JailScenarioScript : MonoBehaviour {

	public Light light;
	public GameObject displayedWand;
	public GameObject playerWand;
	public GameObject stick;
	public GameObject player;

	public AudioClip introVoice;
	private AudioSource introVoiceSource;

	public AudioClip instructionWand;
	private AudioSource instructionWandSource;

	private float timer;
	private bool flashDone = false;
	private bool flashStabilized = false;
	private bool wandTaken = false;

	void Start () {
		timer = 0.0f;
		introVoiceSource = CreateSource(introVoice);
		instructionWandSource = CreateSource (instructionWand);
		introVoiceSource.Play ();

	}

	void Update () {
		timer += Time.deltaTime;

		flashLight ();

		// Checks if the user is trying to grab the wand
		if (!wandTaken){
			if (Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")){

				// If the player can grab it, activate the real wand, the raycasting script, and hide the other objects
				if (Vector3.Distance (displayedWand.transform.position, stick.transform.position) < 2.5f) {
					displayedWand.SetActive (false);
					stick.SetActive (false);
					player.GetComponent<RayCastingController> ().enabled = true;
					playerWand.SetActive (true);
					wandTaken = true;

					light.intensity = 0;
					instructionWandSource.Play ();
				}
			}
		}
	}

	void flashLight(){
		// Waits for the introduction to finish 
		if (timer > 4.0) {
			// Increase the light intensity to 12
			if (!flashDone) {
				light.intensity += 3.0f;
				if (light.intensity > 12) {
					flashDone = true;

					// Makes the wand appear
					displayedWand.SetActive (true);

					// Unfreeze the player
					player.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;				}
			} 

			// Decrease the light intensity to 3
			else {
				if (!flashStabilized) {
					light.intensity -= 0.1f;
					if (light.intensity < 3) {
						flashStabilized = true;
					}
				}
			}

		}
	}

	private AudioSource CreateSource(AudioClip clip)
	{

		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.playOnAwake = false;
		source.clip = clip;
		source.volume = 1;
		source.loop = false;
		return source;
	}
}
