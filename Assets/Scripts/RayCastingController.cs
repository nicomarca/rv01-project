using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class RayCastingController : MonoBehaviour {
	
	private const int 	RAYCASTLENGTH = 1000;		// Length of the ray

	private float 		distanceToObj;				// Distance entre le personnage et l'objet saisi
	private float 		ratio;						// Ratio between the distances
	private float 		newSizeY;					// New vertical size of the moved object
	private Rigidbody 	attachedObject;				// Objet saisi, null si aucun objet saisi
	private Vector3 	objectSizeInitial;  		// Initial size of the object
	private Collision	attachedObjectCollision;	// Collision of the attachedObject
	private Vector3		oldPlayerPos;				// player position before update
	private Vector3		desiredRotationVector;		// rotation of the attached object so it faces the camera
	private bool 		rotationIsFinished;
	private bool 		firstRotation;

	private struct Axis {
		public bool x;
		public bool y;
		public bool z;

		public Axis(bool p1, bool p2, bool p3){
			x = p1;
			y = p2;
			z = p3;
		}
	}

	public Material 	lazerOff, lazerOK;			// Lazer colors
	public Material		lazerOn, lazerMirror; 		// Lazer colors
	public GameObject 	lazer;						// Lazer of the wand 
	public GameObject	wand;						// wand in the right hand of the user
	//public GameObject	skin;						// skin of the user 
	public GameObject	mirrorManager;				// mirror manager to change player size


	void Start () {
		distanceToObj = -1;
		attachedObjectCollision = null;
		lazer.GetComponent<Renderer> ().material = lazerOff;
		lazer.GetComponent<AudioSource> ().enabled = false;
		oldPlayerPos = transform.position;
	}

	void Update () {
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		RaycastHit objectFirstPlane;

		Ray ray = new Ray(wand.transform.position, wand.transform.up);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);
		bool mirrorCasted = false;

		if (rayCasted) {
			rayCasted = firstHit.transform.CompareTag ("draggable");
			mirrorCasted = firstHit.transform.CompareTag ("mirror");
		}

		// Si l'utilisateur bouge, on bouge l'attached object (si il existe) avec lui
		// on actualise pas la taille d'attachedObject car la taille apparente ne change pas
		if (playerMoving ()) {
			return;
		}

		/**** L'UTILISATEUR CLIQUE ***/
		if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown("Grab")) && attachedObject == null) {
			if (rayCasted) {
				objectFirstPlane = firstHit;
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;
				attachedObject.gameObject.AddComponent<CollisionScript>();

				if (attachedObject.GetComponent<MeshRenderer> ()) {
					attachedObject.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				}
				if (attachedObject.GetComponent<Renderer> ()) {
					attachedObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Self-Illumin/Outlined Diffuse");
				}

				rotationIsFinished = true;
				firstRotation = true;
			} 

			else if (mirrorCasted) {
				float ratioNewPlayerSize = mirrorManager.GetComponent<MirrorManagerScript> ().newPlayerSize ();
				transform.position = new Vector3(transform.position.x, transform.position.y * ratioNewPlayerSize, transform.position.z);
				transform.localScale *= ratioNewPlayerSize;
				Vector3 comparatorObjectPosition = mirrorManager.GetComponent<MirrorManagerScript> ().comparatorObject.transform.position;
				GetComponent<FPSdeplacement> ().tSpeed *= ratioNewPlayerSize;
			}
		}
		/*** L'UTILISATEUR RECLIQUE (LACHE L'OBJET) ***/
		else if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown("Grab")) && attachedObject != null) {
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
			attachedObject.isKinematic = false;
			rotationIsFinished = true;
			firstRotation = true;
			GameObject.Destroy(attachedObject.gameObject.GetComponent<CollisionScript>());

			if (attachedObject.GetComponent<MeshRenderer> ()) {
				attachedObject.GetComponent<MeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}

			if (attachedObject.GetComponent<Renderer> ()) {
				attachedObject.GetComponent<Renderer> ().material.shader = Shader.Find ("Standard");
			}


			attachedObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				
			attachedObject = null;

			lazer.transform.GetChild (0).gameObject.SetActive (false);
			lazer.GetComponent<AudioSource> ().enabled = false;
		}

		/*** L'UTILISATEUR A L'OBJET DANS LA MAIN ***/
		if (attachedObject != null) {
			//V1
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);

			if (hitInfo.Length > 0) {
				hitInfo = orderHitInfo (hitInfo); // order hitInfo by distance
				objectSizeInitial = attachedObject.transform.lossyScale;

				if (hitInfo.Length >= 2) {
					
					// 1er cas : le raycast passe par l'objet puis par le terrain
					if (hitInfo[1].transform.tag == "Terrain") {
						Debug.Log ("Dans le 1er cas");
						moveObjectAgainst (ray, hitInfo[1].point, new Axis(false, false, false));
					}
					// 2eme cas : le raycast passe par le terrain (mais pas par l'objet saisi)
					else if (hitInfo[0].transform.tag == "Terrain") {
						Debug.Log ("Dans le 2eme cas");
						moveObjectAgainst (ray, hitInfo[0].point, new Axis(false, false, false));
					} 
					// 3eme cas : le raycast passe par l'objet puis par un autre qui n'est pas le premier terrain
					// typiquement : la tour
					else if (hitInfo [0].transform.gameObject.GetInstanceID () == attachedObject.gameObject.GetInstanceID ()) {
						if (hitInfo [1].transform.tag == "bordure") {
							Debug.Log ("Dans le 3eme cas A");
							moveObjectAgainst (ray, hitInfo [0].transform.gameObject.transform.position, new Axis (false, false, false));
						} 
						else {
							Debug.Log ("Dans le 3eme cas B"); 
							moveObjectAgainst (ray, hitInfo [1].point, new Axis(false, false, true));
						}
					}
					// 4eme cas : un objet est entre nous et attachedObject
					else if (hitInfo [0].transform.gameObject.GetInstanceID () != attachedObject.gameObject.GetInstanceID ()
					         && hitInfo [1].transform.gameObject.GetInstanceID () == attachedObject.gameObject.GetInstanceID ()) {
						//rebaseObjectInFirstPlane ();
						Debug.Log ("Dans le 4eme cas");
						moveObjectAgainst (ray, hitInfo[0].point, new Axis(false, false, false), false);
					}

					// les autres cas non identifiés (dans le doute on offset vers nous)
					else {
						Debug.Log ("Dans le 1er cas mystère");
						moveObjectAgainst (ray, hitInfo [0].point, new Axis(false, false, true));
					}


				}

				// 5eme cas : le raycast passe seulement par l'objet
				// typiquement : on vise le ciel
				else if (hitInfo[0].transform.gameObject.GetInstanceID() == attachedObject.gameObject.GetInstanceID()) {
					Debug.Log("Dans le 5eme cas");
					moveObjectAgainst (ray, attachedObject.transform.position, new Axis(false, false, false));
				}

				// les autres cas non identifiés (dans le doute on offset vers nous)
				else {
  					Debug.Log ("Dans le 2eme cas mystère");
					moveObjectAgainst (ray, attachedObject.transform.position, new Axis(false, false, true));
				}
		

				lazer.GetComponent<Renderer> ().material = lazerOn;
				lazer.transform.GetChild (0).gameObject.SetActive (true);
				lazer.GetComponent<AudioSource> ().enabled = true;
			} 
			// 6eme cas : le raycast ne touche rien
			// typiquement : on vise le ciel mais on perd le raycast sur l'objet
			else {
				Debug.Log("Dans le 6eme cas");
				Vector3 newPos = ray.origin + ray.direction * Vector3.Distance (ray.origin, attachedObject.transform.position);
				moveObjectAgainst (ray, attachedObject.transform.position, new Axis(false, false, false));
			}
		} 

		/*** L'UTILISATEUR BOUGE LA SOURIS SANS CLIQUER ***/
		else {
			if (mirrorCasted) {
				lazer.GetComponent<Renderer> ().material = lazerMirror;
			} else if (rayCasted) {
				lazer.GetComponent<Renderer> ().material = lazerOK;
			} else {
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}
		}
	}


	/** moveObjectAgainst permet de plaquer un objet contre une autre surface tout en
	 * décalant l'objet d'un offset horizontal/vertical/les deux
	 **/
	private void moveObjectAgainst (Ray ray, Vector3 referencePoint, Axis offsetAxis, bool teleport = false) {
		float offsetX = 0;
		float offsetY = 0;
		float offsetZ = 0;

		if (offsetAxis.x) {
			offsetX = attachedObject.transform.lossyScale.x / 2;
		} if (offsetAxis.y) {
			offsetY = attachedObject.transform.lossyScale.y / 2;
		} if (offsetAxis.z) {
			offsetZ = attachedObject.transform.lossyScale.z / 2;
		}
		

		Vector3 offset = new Vector3 (0, offsetY, 0);
		Vector3 newPos = ray.origin + ray.direction * (Vector3.Distance (ray.origin, referencePoint) - offsetZ) + offset;

		// Cas particulier: l'endroit visé est le bas d'un objet
		// Typiquement: le pied de la tour.
		if (newPos.y < attachedObject.transform.lossyScale.y / 2f) {
			newPos.y = attachedObject.transform.lossyScale.y / 2f;
		}

		changePositionAndSize (newPos, teleport);
	}

	private void changePositionAndSize (Vector3 newPosition, bool teleport) {
		Vector3 attachedObjectGroundPosition = attachedObject.position;
		attachedObjectGroundPosition.y = newPosition.y;
		float GroundDistanceFirstPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), attachedObjectGroundPosition);
		float GroundDistanceSecondPlane = Vector3.Distance (Camera.main.transform.position - new Vector3(0, Camera.main.transform.position.y, 0), newPosition);

		float sizeY = attachedObject.transform.lossyScale.y;
		newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);
		ratio = newSizeY / sizeY;

		setAttachedObjectOrientation ();

		if (teleport) {
			attachedObject.transform.position = newPosition;
		} else {
			attachedObject.transform.position = Vector3.MoveTowards (attachedObject.transform.position, newPosition, 1000.0f);
		}

		if (attachedObject.transform.position == newPosition) {
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
		} else {
			Debug.Log ("Position impossible");
		}
	}

	private void setAttachedObjectOrientation() {

		if (firstRotation) {
			Vector3 attachedObjectRotation = attachedObject.transform.rotation.eulerAngles;
			attachedObjectRotation.x = 0;
			attachedObjectRotation.z = 0;

			if (rotationIsFinished) {
				desiredRotationVector = wand.transform.rotation.eulerAngles;
				desiredRotationVector.x = 0;
				desiredRotationVector.z = 0;
				attachedObject.transform.rotation = Quaternion.Lerp (attachedObject.transform.rotation, Quaternion.Euler (desiredRotationVector), 0.1f);
				rotationIsFinished = false;
			} else {
				attachedObject.transform.rotation = Quaternion.Lerp (attachedObject.transform.rotation, Quaternion.Euler (desiredRotationVector), 0.1f);
			}


			if (Mathf.Abs (desiredRotationVector.y - attachedObjectRotation.y) < 1f) {
				rotationIsFinished = true;
				firstRotation = false;
			}
		} else {
			Vector3 newRot = wand.transform.rotation.eulerAngles;
			newRot.x = attachedObject.transform.rotation.eulerAngles.x;
			newRot.z = attachedObject.transform.rotation.eulerAngles.z;
			attachedObject.transform.rotation = Quaternion.Euler(newRot);
		}
	}

	private void setAttachedObjectOrientationOnSky() {
		var rotationVector = wand.transform.rotation.eulerAngles;
		//attachedObject.transform.rotation = Quaternion.Euler(rotationVector);
	}


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

	private bool playerMoving() {
		if (transform.position == oldPlayerPos) {
			return false;
		} else {
			if (attachedObject != null) {
				attachedObject.MovePosition(attachedObject.transform.position + transform.position - oldPlayerPos);
			}
			oldPlayerPos = transform.position;
			return true;
		}
	}

	public void setAttachedObjectCollision(Collision collision) {
		attachedObjectCollision = collision;
	}

	public Collision getAttachedObjectCollision() {
		return attachedObjectCollision;
	}

	public GameObject getAttachedObject() {
		if (attachedObject != null) {
			return attachedObject.gameObject;
		} else {
			return null;
		}
	}

	public void preventMovingAfter(Rigidbody rb, float x){
		StartCoroutine (preventMovingCoroutine(rb, x));
	}

	private IEnumerator preventMovingCoroutine(Rigidbody rb, float x){
		Debug.Log ("J'attends");
		yield return new WaitForSeconds (x);
		Debug.Log ("J'ai attendu");
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}


