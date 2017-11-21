﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class WandController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (InputTracking.GetLocalPosition (VRNode.RightHand) != Vector3.zero) {
			transform.position = InputTracking.GetLocalPosition (VRNode.RightHand);
			transform.rotation = InputTracking.GetLocalRotation (VRNode.RightHand);
		}
		else
		{
			Vector3 targetDir = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x + 10, Input.mousePosition.y, 10f)) - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 100, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
			transform.Rotate(new Vector3(90, 0, 0));
		}
	}
}
