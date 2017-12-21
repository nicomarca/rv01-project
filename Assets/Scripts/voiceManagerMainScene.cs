using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class voiceManagerMainScene : MonoBehaviour {

	public bool mute;

	public AudioClip 		introMainScene;
	private AudioSource 	introMainSceneSource;

	public AudioClip 		chestInstruction;
	private AudioSource 	    chestInstructionSource;

	public AudioClip 		enigma;
	private AudioSource 	enigmaSource;

	public AudioClip 		keyNotEnoughBig;
	private AudioSource 	keyNotEnoughBigSource;

	public AudioClip 		magicTooWeek;
	private AudioSource 	magicTooWeekSource;

	private IEnumerator 	coroutine;					


	// Use this for initialization
	void Start () {

		introMainSceneSource = CreateSource (introMainScene);
		chestInstructionSource = CreateSource (chestInstruction);
		keyNotEnoughBigSource = CreateSource (keyNotEnoughBig);
		magicTooWeekSource = CreateSource (magicTooWeek);
		enigmaSource = CreateSource (enigma);
		PlayIntroMainScene ();


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

	public void PlayIntroMainScene()
	{
		if (!mute) {
			introMainSceneSource.Play ();
		}
	}

	public void PlayChestInstruction()
	{
		if (!mute) {
			chestInstructionSource.Play ();
			coroutine = waitEndAudio (chestInstructionSource.clip.length + 2);
			StartCoroutine (coroutine);

		}
	}

	public void PlayKeyNotEnoughBig()
	{
		if (!mute) {
			keyNotEnoughBigSource.Play ();
		}
	}

	public void PlayMagicTooWeek()
	{
		if (!mute) {
			magicTooWeekSource.Play ();
		}
	}

	private IEnumerator waitEndAudio(float x){
		yield return new WaitForSeconds (x);
		enigmaSource.Play ();

	}
		

}
