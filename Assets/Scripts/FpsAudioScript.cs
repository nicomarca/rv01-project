using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsAudioScript : MonoBehaviour {

	[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
	public AudioClip m_FootstepSound_1;
	public AudioClip m_FootstepSound_2;
	public AudioClip m_FootstepSound_3;
	public AudioClip m_FootstepSound_4;
	public AudioClip m_JumpSound;           // the sound played when character leaves the ground.
	public AudioClip m_LandSound;           // the sound played when character touches back on ground.

	private AudioSource m_AudioSource;

	float time;

	// Use this for initialization
	void Start () {
		m_FootstepSounds = new AudioClip[4];
		m_FootstepSounds[0] =  m_FootstepSound_1;
		m_FootstepSounds[1] =  m_FootstepSound_2;
		m_FootstepSounds[2] =  m_FootstepSound_3;
		m_FootstepSounds[3] =  m_FootstepSound_4;
		m_AudioSource = GetComponent<AudioSource>();

		time = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayLandingSound()
	{
		m_AudioSource.clip = m_LandSound;
		m_AudioSource.Play();
		//m_NextStep = m_StepCycle + .5f;
	}

	public void PlayJumpSound()
	{
		m_AudioSource.clip = m_JumpSound;
		m_AudioSource.Play();
	}

	public void PlayFootStepAudio(bool isGrounded, bool isMoving)
	{
		//Debug.Log (Time.time);

		if (Time.time - time <= 0.4)
		{
			return;
		}

		if (!isGrounded || !isMoving)
		{
			return;
		}

		//Debug.Log ("OUI : " + (Time.time - time));
		// pick & play a random footstep sound from the array,
		int n = Random.Range(0, 4);
		Debug.Log (n);
		m_AudioSource.clip = m_FootstepSounds[n];
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
		// move picked sound to index 0 so it's not picked next time
		m_FootstepSounds[n] = m_FootstepSounds[0];
		m_FootstepSounds[0] = m_AudioSource.clip;

		time = Time.time;
	}
}
