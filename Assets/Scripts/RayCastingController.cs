using UnityEngine;
using System.Collections;
using UnityEngine.VR;



public class RayCastingController : MonoBehaviour 
{
	const float BASEDISTANCE = 1;

	public GameObject wand;

	private float distanceToObj;	// Distance entre le personnage et l'objet saisi
	private float oldDistance;
	private Rigidbody attachedObject;	// Objet saisi, null si aucun objet saisi
	private Vector3 objectSizeInitial;
	private Vector3 secondObjectSize;
	private float ratio;

	private float distanceInitial;
	private float newSizeY;

	public const int RAYCASTLENGTH = 100;	// Longueur du rayon issu de la caméra
	public Material lazerOff, lazerOK, lazerOn;
	public GameObject lazer;

	void Start () 
	{
		distanceToObj = -1;
		lazer.GetComponent<Renderer> ().material = lazerOff;
	}


	void Update () 
	{
		// Le raycast attache un objet cliqué
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		Ray ray = new Ray(wand.transform.position, wand.transform.up);
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
				Debug.Log ("Object attached");
			}
		} 

		/*** L'UTILISATEUR RECLIQUE (LACHE L'OBJET) ***/
		else if (Input.GetMouseButtonDown (0) && attachedObject != null)
		{
			Vector3 vect = new Vector3 (0, newSizeY / 4, 0);
			attachedObject.transform.position += vect;
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

				objectFirstPlane = hitInfo [0]; //normalement == attachedObject
				objectSizeInitial = attachedObject.transform.lossyScale;

				if (hitInfo.Length >= 2) {
					RaycastHit objectSecondPlane;
					objectSecondPlane = hitInfo [1];
					if (objectSecondPlane.transform.name == "Terrain") {

						//secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
						float sizeY = objectSizeInitial.y;
						float distanceToSecondPlane = objectSecondPlane.distance;

						//Calculer nouvelle taille
						Vector3 attachedObjectGroundPosition = attachedObject.position;
						attachedObjectGroundPosition.y = 0;
						float GroundDistanceFirstPlane = Vector3.Distance (wand.transform.position - new Vector3(0,wand.transform.position.y,0), attachedObjectGroundPosition);
						float GroundDistanceSecondPlane = Vector3.Distance (wand.transform.position - new Vector3(0,wand.transform.position.y,0), objectSecondPlane.point);

						newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);

						Debug.Log (GroundDistanceSecondPlane + "et" +GroundDistanceFirstPlane);


						ratio = newSizeY / sizeY;
						/*if (ratio > 0.95 && ratio < 1.05) {
							newSizeY = sizeY;
							ratio = 1;
						}*/

						//Translater
						attachedObject.transform.position = objectSecondPlane.point + new Vector3(0, newSizeY/2, 0);;
						attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, objectSizeInitial.y * ratio, objectSizeInitial.z * ratio);
					} else {
					
						//Vector3 vect = new Vector3 (0, attachedObject.GetComponent<Rigidbody>().GetComponent<Renderer> ().bounds.size.y/2, 0);
						Vector3 newPos = objectFirstPlane.point;
						if (objectFirstPlane.transform.name == "Terrain") {
							distanceToObj = objectFirstPlane.distance;
							objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;

							//secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
							float sizeY = objectSizeInitial.y;
							float distanceToFirstPlane = objectFirstPlane.distance;

							//Calculer nouvelle taille
							Vector3 attachedObjectGroundPosition = attachedObject.position;
							attachedObjectGroundPosition.y = 0;
							float GroundDistanceFirstPlane = Vector3.Distance (wand.transform.position - new Vector3(0,wand.transform.position.y,0), attachedObjectGroundPosition);
							float GroundDistanceSecondPlane = Vector3.Distance (wand.transform.position - new Vector3(0,wand.transform.position.y,0), objectFirstPlane.point);

							/*if (GroundDistanceFirstPlane / GroundDistanceSecondPlane > 0.90 && GroundDistanceFirstPlane / GroundDistanceSecondPlane < 1.10) {
								GroundDistanceFirstPlane = GroundDistanceSecondPlane;
							}*/

							//newSizeY = (distanceToSecondPlane - distanceToObj) / distanceToSecondPlane * sizeY;
							newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);

							ratio = newSizeY / sizeY;

							/*if (ratio > 0.95 && ratio < 1.05) {
								newSizeY = sizeY;
								ratio = 1;
							}*/

							//Translater
							Vector3 vect = new Vector3 (0, newSizeY / 2, 0);

							//attachedObject.MovePosition (ray.origin + (ray.direction * distanceToSecondPlane) + vect);
							attachedObject.transform.position = objectFirstPlane.point + vect;
							attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, objectSizeInitial.y * ratio, objectSizeInitial.z * ratio);

							Debug.Log ("ray direction : " + ray.direction);
							Debug.Log ("nouvelle position : " + attachedObject.position);

				
						}
					}
				}
				lazer.GetComponent<Renderer> ().material = lazerOn;
			}
		} 
		else  // L'utilisateur bouge la sourie sans cliquer 
		{
			if (rayCasted) 
			{
				lazer.GetComponent<Renderer> ().material = lazerOK;
			} 
			else 
			{
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}
		}
	}
}


