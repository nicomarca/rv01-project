﻿using UnityEngine;
using System.Collections;



public class RayCastingController : MonoBehaviour 
{
	const float BASEDISTANCE = 1;

	private float distanceToObj;	// Distance entre le personnage et l'objet saisi
	private float oldDistance;
	private Rigidbody attachedObject;	// Objet saisi, null si aucun objet saisi
	private Vector3 objectSizeInitial;
	private Vector3 secondObjectSize;
	private float ratio;

	private float distanceInitial;
	private float newSizeY;

	public const int RAYCASTLENGTH = 100;	// Longueur du rayon issu de la caméra
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = new Vector2(16, 16);	// Offset du centre du curseur
	public Texture2D cursorOff, cursorDragged, cursorDraggable;	// Textures à appliquer aux curseurs


	void Start () 
	{
		distanceToObj = -1;
		Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
		Cursor.visible = true;
		Cursor.SetCursor (cursorDragged, hotSpot, cursorMode);

	}


	void Update () 
	{
		// Le raycast attache un objet cliqué
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);
		RaycastHit objectFirstPlane;

		if (rayCasted) 
		{
			rayCasted = firstHit.transform.CompareTag ("draggable");
		}

		/**** L'UTILISATEUR CLIQUE ***/
		if (Input.GetMouseButtonDown (0) && attachedObject == null)
		{

			if (rayCasted) 
			{
				objectFirstPlane = firstHit;
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;
				oldDistance = distanceToObj;
				Debug.Log ("Object attached");
			}
		} 

		/*** L'UTILISATEUR RECLIQUE (LACHE L'OBJET) ***/
		else if (Input.GetMouseButtonDown (0) && attachedObject != null)
		{
			/*hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);
			Debug.Log (hitInfo.Length);
			// trier raycasthit[]
			for (int K = 0; K < hitInfo.Length; K++) {
				for (int I = hitInfo.Length - 2; I >= 0; I--) {
					for (int J = 0; J <= I; J++) {
						if (hitInfo [J + 1].distance < hitInfo [J].distance) {
							RaycastHit t = hitInfo [J + 1];
							hitInfo [J + 1] = hitInfo [J];
							hitInfo [J] = t;
						}
					}
				}
			}

			for (int i = 0; i < hitInfo.Length; i++) {
				Debug.Log ("info objet " + (i+1));
				Debug.Log (hitInfo [i].transform.name);
				Debug.Log (hitInfo [i].transform.position);
			}



			if (hitInfo.Length >= 2) {
				RaycastHit objectSecondPlane;

				objectFirstPlane = hitInfo [0];
				objectSecondPlane = hitInfo [1];
				if (objectFirstPlane.transform.CompareTag ("draggable") && objectSecondPlane.transform.GetComponent<Renderer> ()) {
					Debug.Log ("\n distance actuelle : " + hitInfo [0].distance);
					Debug.Log ("\n nouvelle distance : " + hitInfo [1].distance);
					distanceToObj = objectSecondPlane.distance;
					objectSizeInitial = hitInfo [0].rigidbody.GetComponent<Renderer> ().bounds.size;
					secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
					float sizeY = objectSizeInitial.y;
					distanceInitial = objectFirstPlane.distance;
					//Calculer nouvelle taille
					newSizeY = (distanceToObj - secondObjectSize.x) / BASEDISTANCE * sizeY;
					Debug.Log ("tailleactuelle : " + sizeY);
					Debug.Log ("nouvelle taille : " + newSizeY);
					float ratio = newSizeY / sizeY;
					//Translater
					Debug.Log ("ray direction : " + ray.direction);
					Debug.Log ("nouvelle position : " + ray.origin + (ray.direction * (distanceToObj)));
					hitInfo [0].transform.position = (ray.origin + (ray.direction * (distanceToObj - (objectSizeInitial.z * ratio) / 2)));
					//attachedObject.GetComponent<Renderer>().bounds.size.Set(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
					attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);

			
					attachedObject.isKinematic = false;
					attachedObject = null;
					Debug.Log ("Object detached");
					if (rayCasted) {
						Cursor.SetCursor (cursorDraggable, hotSpot, cursorMode);
					} else {
						Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
					}
			
				}
			}*/
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, objectSizeInitial.y * ratio, objectSizeInitial.z * ratio);
			attachedObject.isKinematic = false;
			attachedObject = null;
			Debug.Log ("Object detached");
		}

		/*** L'UTILISATEUR A L'OBJET DANS LA MAIN ***/
		if ( attachedObject != null) // L'utilisateur continue la saisie d'un objet
		{
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);
			if (hitInfo.Length > 0) {
				//Debug.Log (hitInfo.Length);
				// trier raycasthit[]
				for (int K = 0; K < hitInfo.Length; K++) {
					for (int I = hitInfo.Length - 2; I >= 0; I--) {
						for (int J = 0; J <= I; J++) {
							if (hitInfo [J + 1].distance < hitInfo [J].distance) {
								RaycastHit t = hitInfo [J + 1];
								hitInfo [J + 1] = hitInfo [J];
								hitInfo [J] = t;
							}
						}
					}
				}
				/*
			for (int K = 0; K < hitInfo.Length; K++) {
				if (hitInfo [K].rigidbody == attachedObject) {
					for (int L = 0; L < hitInfo.Length - K - 1; L++) {
						hitInfo [K + L] = hitInfo [K + L + 1];
					}
					//hitInfo [hitInfo.Length - 1] = null;
				}
			}*/

				objectFirstPlane = hitInfo [0]; //normalement == attachedObject
				objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;

				if (hitInfo.Length >= 2) {
					RaycastHit objectSecondPlane;
					objectSecondPlane = hitInfo [1];
					if (objectSecondPlane.transform.name == "Terrain") {
						Debug.Log ("\n distance actuelle : " + objectFirstPlane.distance);
						Debug.Log ("\n nouvelle distance : " + objectSecondPlane.distance);


						//secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
						float sizeY = objectSizeInitial.y;
						float distanceToSecondPlane = objectSecondPlane.distance;

						//Calculer nouvelle taille
						float GroundDistanceFirstPlane = Vector3.Distance (this.gameObject.transform.position, attachedObject.position);
						float GroundDistanceSecondPlane = Vector3.Distance (this.gameObject.transform.position, objectSecondPlane.point);
						if (GroundDistanceFirstPlane / GroundDistanceSecondPlane > 0.90 && GroundDistanceFirstPlane / GroundDistanceSecondPlane < 1.10) {
							GroundDistanceFirstPlane = GroundDistanceSecondPlane;
						}

						//newSizeY = (distanceToSecondPlane - distanceToObj) / distanceToSecondPlane * sizeY;
						newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);

						Debug.Log ("tailleactuelle : " + sizeY);
						Debug.Log ("nouvelle taille : " + newSizeY);
						ratio = newSizeY / sizeY;

						//Translater
						Vector3 vect = new Vector3 (0, newSizeY / 2, 0);

						//attachedObject.MovePosition (ray.origin + (ray.direction * distanceToSecondPlane) + vect);
						attachedObject.transform.position = ray.origin + (ray.direction * distanceToSecondPlane) + vect;
						Debug.Log (newSizeY);
						attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, objectSizeInitial.y * ratio, objectSizeInitial.z * ratio);

						Debug.Log ("ray direction : " + ray.direction);
						Debug.Log ("nouvelle position : " + attachedObject.position);
					} else {
					
						//Vector3 vect = new Vector3 (0, attachedObject.GetComponent<Rigidbody>().GetComponent<Renderer> ().bounds.size.y/2, 0);
						Vector3 newPos = objectFirstPlane.point;
						if (objectFirstPlane.transform.name == "Terrain") {
							distanceToObj = objectFirstPlane.distance;
							objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;
							/*
						attachedObject.MovePosition (ray.origin + (ray.direction * distanceToObj));
						objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;
						attachedObject.transform.position = new Vector3 (newPos.x, newPos.y + objectSizeInitial.y / 2, newPos.z);
						*/
							//secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
							float sizeY = objectSizeInitial.y;
							float distanceToFirstPlane = objectFirstPlane.distance;

							//Calculer nouvelle taille

							float GroundDistanceFirstPlane = Vector3.Distance (this.gameObject.transform.position, attachedObject.position);
							float GroundDistanceSecondPlane = Vector3.Distance (this.gameObject.transform.position, objectFirstPlane.point);

							if (GroundDistanceFirstPlane / GroundDistanceSecondPlane > 0.90 && GroundDistanceFirstPlane / GroundDistanceSecondPlane < 1.10) {
								GroundDistanceFirstPlane = GroundDistanceSecondPlane;
							}

							//newSizeY = (distanceToSecondPlane - distanceToObj) / distanceToSecondPlane * sizeY;
							newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);

							Debug.Log ("tailleactuelle : " + sizeY);
							Debug.Log ("nouvelle taille : " + newSizeY);
							ratio = newSizeY / sizeY;

							//Translater
							Vector3 vect = new Vector3 (0, newSizeY / 2, 0);

							//attachedObject.MovePosition (ray.origin + (ray.direction * distanceToSecondPlane) + vect);
							attachedObject.transform.position = ray.origin + (ray.direction * distanceToFirstPlane) + vect;
							Debug.Log (newSizeY);
							attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, objectSizeInitial.y * ratio, objectSizeInitial.z * ratio);

							Debug.Log ("ray direction : " + ray.direction);
							Debug.Log ("nouvelle position : " + attachedObject.position);

				
						}
					}
				}
				Cursor.SetCursor (cursorDragged, hotSpot, cursorMode);
			}
		} 
		else  // L'utilisateur bouge la sourie sans cliquer 
		{
			if (rayCasted) 
			{
				Cursor.SetCursor (cursorDraggable, hotSpot, cursorMode);
			} 
			else 
			{
				Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
			}
		}
	}
}


