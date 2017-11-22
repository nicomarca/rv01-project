using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class RayCastingController : MonoBehaviour {
	private const int 	RAYCASTLENGTH = 200;		// Length of the ray

	private float 		distanceToObj;				// Distance entre le personnage et l'objet saisi
	private float 		ratio;						// Ratio between the distances
	private float 		newSizeY;					// New vertical size of the moved object
	private Rigidbody 	attachedObject;				// Objet saisi, null si aucun objet saisi
	private Vector3 	objectSizeInitial;  		// Initial size of the object
	private Collision	attachedObjectCollision;	// Collision of the attachedObject

	public Material 	lazerOff, lazerOK, lazerOn; // Lazer colors
	public GameObject 	lazer;						// Lazer of the wand 
	public GameObject	wand;						// wand in the right hand of the user

	void Start () {
		distanceToObj = -1;
		attachedObjectCollision = null;
		lazer.GetComponent<Renderer> ().material = lazerOff;
	}


	void Update () {
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		RaycastHit objectFirstPlane;

		Ray ray = new Ray(wand.transform.position, wand.transform.up);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);

		if (rayCasted) 
		{
			rayCasted = firstHit.transform.CompareTag ("draggable");
		}

		/**** L'UTILISATEUR CLIQUE ***/
		if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown("Grab")) && attachedObject == null)
		{

			if (rayCasted) 
			{
				objectFirstPlane = firstHit;
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.constraints = RigidbodyConstraints.None;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;
				attachedObject.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				// setAttachedObjectOrientation ();
			}
		}
		/*** L'UTILISATEUR RECLIQUE (LACHE L'OBJET) ***/
		else if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown("Grab")) && attachedObject != null)
		{
			Vector3 vect = new Vector3 (0, newSizeY / 4, 0);
			attachedObject.transform.position += vect;
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
			attachedObject.isKinematic = false;
			attachedObject.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			attachedObject = null;
		}

		/*** L'UTILISATEUR A L'OBJET DANS LA MAIN ***/
		if (attachedObject != null) {
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);
			if (hitInfo.Length > 0) {
				hitInfo = orderHitInfo (hitInfo); // order hitInfo by distance

				objectFirstPlane = hitInfo [0]; // normalement == attachedObject
				objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;

				if (hitInfo.Length >= 2) {
					RaycastHit objectSecondPlane;
					objectSecondPlane = hitInfo [1];

					// 1er cas : le raycast passe par l'objet puis par le terrain
					if (objectSecondPlane.transform.tag == "Terrain") {
						changePositionAndSizeOnGround (objectSecondPlane.point, objectSizeInitial.y);
					} 
					// 2eme cas : le raycast passe par le terrain (mais pas par l'objet saisi)
					else if (objectFirstPlane.transform.tag == "Terrain"){
						changePositionAndSizeOnGround (objectFirstPlane.point, objectSizeInitial.y);
					} 
					// 3eme cas : le raycast passe par l'objet puis par un autre qui n'est pas le premier terrain
					// typiquement : les montagnes
					else if (hitInfo [0].transform.name == attachedObject.name) {
						Vector3 newPos = ray.origin + ray.direction * Vector3.Distance (ray.origin, attachedObject.transform.position);
						attachedObject.transform.position = newPos;
					}

					//TODO Le cas ou un objet qui n'est pas le terrain passe entre nous et l'objet saisi
				} 

				// 4eme cas : le raycast passe seulement par l'objet
				// typiquement : on vise le ciel
				else if (hitInfo [0].transform.name == attachedObject.name) {
					Vector3 newPos = ray.origin + ray.direction * Vector3.Distance (ray.origin, attachedObject.transform.position);
					attachedObject.transform.position = newPos;
				}

				lazer.GetComponent<Renderer> ().material = lazerOn;
			} 
			// 5eme cas : le raycast ne touche rien
			// typiquement : on vise le ciel mais on perd le raycast sur l'objet
			else {
				Vector3 newPos = ray.origin + ray.direction * Vector3.Distance (ray.origin, attachedObject.transform.position);
				attachedObject.transform.position = newPos;
			}
		} 
		/*** L'UTILISATEUR BOUGE LA SOURIS SANS CLIQUER ***/
		else {
			if (rayCasted) {
				lazer.GetComponent<Renderer> ().material = lazerOK;
			} 
			else {
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}
		}
	}


	// If referenced object is the ground
	private void changePositionAndSizeOnGround(Vector3 referenceObjectPoint, float sizeY)
	{
		// Calculate new size
		Vector3 attachedObjectGroundPosition = attachedObject.position;
		attachedObjectGroundPosition.y = referenceObjectPoint.y;
		float GroundDistanceFirstPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), attachedObjectGroundPosition);
		float GroundDistanceSecondPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), referenceObjectPoint);

		newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);
		ratio = newSizeY / sizeY;

		//DEBUG (uncomment what you need)
		//Debug.Log("attachedObjectGroundPosition.y : " + attachedObjectGroundPosition.y);
		//Debug.Log("GroundDistanceFirstPlane : " + GroundDistanceFirstPlane);
		//Debug.Log("GroundDistanceSecondPlane : " + GroundDistanceSecondPlane);
		//Debug.Log ("newSizeY : " + newSizeY + " ; sizeY : " + sizeY);
		//Debug.Log ("ratio : " + ratio);


		// Translate
		Vector3 verticalReplacement = new Vector3 (0, newSizeY / 2, 0);
		attachedObject.transform.position = referenceObjectPoint + verticalReplacement;
		attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
		//attachedObject.GetComponent<BoxCollider>().size = attachedObject.GetComponent<BoxCollider>().size * ratio;

		// Rotation
		Vector3 rotationVector = attachedObject.transform.rotation.eulerAngles;
		// rotationVector.x = 0;
		rotationVector.y = wand.transform.rotation.eulerAngles.y;
		// rotationVector.z = 0;
		attachedObject.transform.rotation = Quaternion.Euler (rotationVector);
	}

	// If referenced object is an other object
	private void changePositionAndSizeOnObject(Vector3 referecendPoint) {
		attachedObject.transform.position = Vector3.MoveTowards (attachedObject.transform.position, referecendPoint, 100.0f);
	}

	/*
	private void setAttachedObjectOrientation() {
		Vector3 rotationVector = attachedObject.transform.rotation.eulerAngles;
		rotationVector.y = wand.transform.rotation.eulerAngles.y;
		Quaternion quaternionArrival = Quaternion.Euler (rotationVector);
		attachedObject.transform.rotation = Quaternion.Slerp (attachedObject.transform.rotation, quaternionArrival, 0.1f);
	}
	*/

	// Order hitInfo by distance
	private RaycastHit[] orderHitInfo(RaycastHit[] hitInfo) {
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
		return hitInfo;
	}

	private Vector3 getClosestContactPoint(ContactPoint[] contactPoints) {
		Vector3 closestPoint = new Vector3(100, 100, 100);
		foreach (ContactPoint contact in contactPoints) {
			if (Vector3.Distance (Camera.main.transform.position, closestPoint) < Vector3.Distance (Camera.main.transform.position, contact.point)) {
				closestPoint = contact.point;
			}
		}
		return closestPoint;
	}

	public void setAttachedObjectCollision(Collision collision) {
		attachedObjectCollision = collision;
	}

	public Collision getAttachedObjectCollision() {
		return attachedObjectCollision;
	}

}


