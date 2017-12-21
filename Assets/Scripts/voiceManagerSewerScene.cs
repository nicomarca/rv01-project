using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class voiceManagerSewerScene : MonoBehaviour {

	public bool mute;

	public AudioClip 		wandDiscover;
	private AudioSource 	wandDiscoverSource;



	// Use this for initialization
	void Start () {

		wandDiscoverSource = CreateSource (wandDiscover);

	}

	// Update is called once per frame
	void Update () {

	}


	private AudioSource CreateSource(AudioClip clip) {
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.playOnAwake = false;
		source.clip = clip;
		source.volume = 1;
		source.loop = false;
		return source;
	}

	public void PlayWandDiscover()
	{
		if (!mute) {
			wandDiscoverSource.Play ();
		}
	}
}
