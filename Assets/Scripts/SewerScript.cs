using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerScript : MonoBehaviour {

	public AudioClip 		caveTheme;
	private AudioSource 	caveThemeSource;

	public GameObject player;
	public GameObject normalWand;
	public GameObject masterWandPlayer;
	public GameObject masterWandScene;
	private bool masterWandTaken;

	// Use this for initialization
	void Start () {
		caveThemeSource = CreateSource(caveTheme);
		caveThemeSource.Play ();

		masterWandTaken = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!masterWandTaken) {
			if (Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")) {
				if (player.GetComponent<RayCastingController> ().IsWandCasted ()) {
					normalWand.SetActive (false);
					masterWandPlayer.SetActive (true);
					masterWandScene.SetActive (false);
					masterWandTaken = true;
				}
			}
		}
	}

	private AudioSource CreateSource(AudioClip clip) {
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.playOnAwake = false;
		source.clip = clip;
		source.volume = 0.7f;
		source.loop = true;
		return source;
	}
}
