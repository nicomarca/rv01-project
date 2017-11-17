using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSdeplacement : MonoBehaviour {

	public float tSpeed = 0.15f;
	public float rSpeed = 8f;

	private bool vr;

	// Use this for initialization
	void Start () {
		vr = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (vr) {
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
