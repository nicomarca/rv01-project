using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class voiceManagerMainBack : MonoBehaviour {

		public bool mute;

		public AudioClip 		end;
		private AudioSource 	endSource;



		// Use this for initialization
		void Start () {

		endSource = CreateSource (end);

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

		public void PlayEnd()
		{
			if (!mute) {
				endSource.Play ();
			}
		}
	}
