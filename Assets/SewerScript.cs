using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerScript : MonoBehaviour {

	public AudioClip 		caveTheme;
	private AudioSource 	caveThemeSource;

	// Use this for initialization
	void Start () {
		caveThemeSource = CreateSource(caveTheme);
		caveThemeSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
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
