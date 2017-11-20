﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;


public class FPSdeplacement : MonoBehaviour {

	public float tSpeed = 0.15f;
	public float rSpeed = 8f;

	private bool vr = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (vr) {
			float mouvmentHorizontal = Input.GetAxis ("Horizontal");
			float mouvmentVertical = Input.GetAxis ("Vertical");
			Vector3 mouvment = new Vector3 (mouvmentHorizontal, 0, mouvmentVertical);
			this.transform.Translate(Camera.main.transform.rotation * mouvment * tSpeed);
		} else {
			if(Input.GetKey("z"))
			{
				this.transform.Translate(Vector3.forward * tSpeed);
			}
			if(Input.GetKey("s"))
			{
				this.transform.Translate(Vector3.back * tSpeed);
			}
			if(Input.GetKey("q"))
			{
				this.transform.Translate(Vector3.left * tSpeed);
			}
			if(Input.GetKey("d"))
			{
				this.transform.Translate(Vector3.right * tSpeed);
			}

			if (Input.GetKey("left")) {
				transform.Rotate(Vector3.up, -rSpeed);
			}
			if (Input.GetKey("right")) {
				transform.Rotate(Vector3.up, rSpeed);

			}

		}
		}
		
	}
