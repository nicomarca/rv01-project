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
		if (Input.GetMouseButtonDown (0) && attachedObject == null) {
			if (rayCasted) {
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
		else if (Input.GetMouseButtonDown (0) && attachedObject != null) {
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
				hitInfo = orderHitInfo(hitInfo); // order hitInfo by distance

				objectFirstPlane = hitInfo [0]; // normalement == attachedObject
				objectSizeInitial = attachedObject.transform.lossyScale;

				if (hitInfo.Length >= 2) {
					RaycastHit objectSecondPlane;
					objectSecondPlane = hitInfo [1];

					if (attachedObjectCollision == null) {
						if (objectSecondPlane.transform.CompareTag("terrain")) {
							changePositionAndSizeOnGround (objectSecondPlane.point, objectSizeInitial.y);
						} else {
							if (objectFirstPlane.transform.CompareTag("terrain")) {
								changePositionAndSizeOnGround (objectFirstPlane.point, objectSizeInitial.y);
							} else if (objectFirstPlane.transform.CompareTag("draggable")) {

								// attachedobject est derriere un objet au premier plan
								// il faut le ramener au premier plan

								/*
								objectFirstPlane.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
								// attachedObject.transform.position = objectFirstPlane.point - new Vector3(0, 0, attachedObject.transform.lossyScale.z);
								Vector3 temp = Camera.main.transform.position - objectFirstPlane.point;
								attachedObject.transform.position = temp - temp.normalized * attachedObject.transform.lossyScale.z;
								Debug.LogError ("attachedObject position : " + attachedObject.transform.position);
								Debug.LogError ("attachedObject scale : " + attachedObject.transform.lossyScale);
								//changePositionAndSizeOnGround(objectFirstPlane.point, objectSizeInitial.y);
								*/
							}
						}
					} else {

						// on pousse attachedObject jusqu'a un autre objet

						Vector3 closestContactPoint = getClosestContactPoint (attachedObjectCollision.contacts);
						//changePositionAndSizeOnGround(closestContactPoint, objectSizeInitial.y);
					}
				}

				lazer.GetComponent<Renderer> ().material = lazerOn;
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
	private void changePositionAndSizeOnGround(Vector3 referencedPoint, float sizeY) {
		// Calculate new size
		Vector3 attachedObjectGroundPosition = attachedObject.position;
		attachedObjectGroundPosition.y = referencedPoint.y;
		float GroundDistanceFirstPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), attachedObjectGroundPosition);
		float GroundDistanceSecondPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), referencedPoint);

		newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);
		ratio = newSizeY / sizeY;

		// Translater
		Vector3 verticalReplacement = new Vector3 (0, newSizeY / 2, 0);
		attachedObject.transform.position = referencedPoint + verticalReplacement;
		attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
		//attachedObject.GetComponent<BoxCollider>().size = attachedObject.GetComponent<BoxCollider>().size * ratio;

		// Rotation
		Vector3 rotationVector = attachedObject.transform.rotation.eulerAngles;
		rotationVector.y = 0;
		rotationVector.y = wand.transform.rotation.eulerAngles.y;
		rotationVector.y = 0;
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


