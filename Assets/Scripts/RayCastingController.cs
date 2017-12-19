using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using System.Collections.Generic;


public class RayCastingController : MonoBehaviour {
	
	private const int 		RAYCASTLENGTH 				= 1000;								// Length of the ray
	private const string 	SHADER_OUTLINED_DIFFUSE 	= "Self-Illumin/Outlined Diffuse";	// Shader Outlined diffuse
	private const string 	SHADER_STANDARD 			= "Standard";						// Standard Shader

	private float 			distanceToObj;				// Distance between player and seased object
	private float 			ratio;						// Ratio between the distances
	private float 			newSizeY;					// New vertical size of the moved object
	private bool 			rotationIsFinished;			// Is the first rotation finished
	private bool 			firstRotation;				// Is the first rotation (SLerp until done)
	private string 			previousShader;				// Previous used shader
	private Rigidbody 		attachedObject;				// Seased object, null if none
	private Collision		attachedObjectCollision;	// Collision of the attachedObject
	private Vector3 		objectSizeInitial;  		// Initial size of the object
	private Vector3			oldPlayerPos;				// Player position before update
	private Vector3			desiredRotationVector;		// Rotation of the attached object so it faces the camera
	private List<Transform> listOfObjectToShader;		// List of sub-objects to shader for a given object
	private List<Transform> listOfObjectToShadow;		// List of sub-objects to shadow for a given object

	public Material 	lazerOff, lazerOK;				// Lazer colors
	public Material		lazerOn,  lazerMirror;	 		// Lazer colors
	public GameObject 	lazer;							// Lazer of the wand 
	public GameObject	wand;							// wand in the right hand of the user
	public GameObject	mirrorManager;					// mirror manager to change player size
	public GameObject 	finalChest;

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


	void Start () {
		distanceToObj = -1;
		attachedObjectCollision = null;
		lazer.GetComponent<Renderer> ().material = lazerOff;
		lazer.GetComponent<AudioSource> ().enabled = false;
		oldPlayerPos = transform.position;
		previousShader = null;
	}

	void Update () {
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		RaycastHit objectFirstPlane;

		Ray ray = new Ray (wand.transform.position, wand.transform.up);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);
		bool mirrorCasted = false;
		bool chestCasted = false;

		if (rayCasted) {
			rayCasted = firstHit.transform.CompareTag ("draggable");
			mirrorCasted = firstHit.transform.CompareTag ("mirror");
			chestCasted = firstHit.transform.CompareTag ("chest");


		}

		// If the user moves, attached object moves too if it exists ; relative size doesn't change
		if (playerMoving ()) {
			return;
		}

		/**** THE USER CLICKS ***/
		if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")) && attachedObject == null) {
			if (rayCasted) {
				objectFirstPlane = firstHit;
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;
				applyShader (SHADER_OUTLINED_DIFFUSE);
				applyShadow (UnityEngine.Rendering.ShadowCastingMode.Off);
				attachedObject.gameObject.AddComponent<CollisionScript> ();
				rotationIsFinished = true;
				firstRotation = true;
			} else if (mirrorCasted) {
				float ratioNewPlayerSize = mirrorManager.GetComponent<MirrorManagerScript> ().newPlayerSize ();
				transform.position = new Vector3 (transform.position.x, transform.position.y * ratioNewPlayerSize, transform.position.z);
				transform.localScale *= ratioNewPlayerSize;
				Vector3 comparatorObjectPosition = mirrorManager.GetComponent<MirrorManagerScript> ().comparatorObject.transform.position;
				GetComponent<FPSdeplacement> ().tSpeed *= ratioNewPlayerSize;
			} else if (chestCasted) {
				objectFirstPlane = firstHit;
				objectFirstPlane.transform.GetComponent<ActivateChest> ()._open = true;

			}

		}
		/*** THE USER CLICKS AGAIN (RELEASES THE OBJECT) ***/
		else if ((Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Grab")) && attachedObject != null) {
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
			attachedObject.isKinematic = false;
			rotationIsFinished = true;
			firstRotation = true;
			GameObject.Destroy (attachedObject.gameObject.GetComponent<CollisionScript> ());
			applyShader (previousShader);
			applyShadow (UnityEngine.Rendering.ShadowCastingMode.On);
			GameObject.Destroy (attachedObject.gameObject.GetComponent<CollisionScript> ());
			attachedObject = null;
			previousShader = null;
			lazer.transform.GetChild (0).gameObject.SetActive (false);
			lazer.GetComponent<AudioSource> ().enabled = false;
		}

		/*** THE USER HAS THE OBJECT ***/
		if (attachedObject != null) {
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);

			if (hitInfo.Length > 0) {
				hitInfo = orderHitInfo (hitInfo); // order hitInfo by distance
				objectSizeInitial = attachedObject.transform.lossyScale;

				if (hitInfo.Length >= 2) {
					// 1st case : the ray goes through the object and then through the ground
					if (hitInfo [1].transform.tag == "Terrain") {
						moveObjectAgainst (ray, hitInfo [1].point, new Axis (false, false, false));
					}
					// 2nd case : the ray goes only through the ground
					else if (hitInfo [0].transform.tag == "Terrain") {
						moveObjectAgainst (ray, hitInfo [0].point, new Axis (false, false, false));
					}
					// 3rd case : the ray goes through the object and then throw a second object (tower for exemple)
					else if (hitInfo [0].transform.gameObject.GetInstanceID () == attachedObject.gameObject.GetInstanceID ()) {
						if (hitInfo [1].transform.tag == "bordure") {
							moveObjectAgainst (ray, hitInfo [0].transform.gameObject.transform.position, new Axis (false, false, false));
						} else {
							moveObjectAgainst (ray, hitInfo [1].point, new Axis (false, false, true));
						}
					}
					// 4th case : an object is between us and the attachedObject
					else if (hitInfo [0].transform.gameObject.GetInstanceID () != attachedObject.gameObject.GetInstanceID ()
					         && hitInfo [1].transform.gameObject.GetInstanceID () == attachedObject.gameObject.GetInstanceID ()) {
						moveObjectAgainst (ray, hitInfo [0].point, new Axis (false, false, false), false);
					}
					// Other unidentified cases (offset in our direction)
					else {
						moveObjectAgainst (ray, hitInfo [0].point, new Axis (false, false, true));
					}
				}
				// 5th case : the ray goes through only one object (points to the sky)
				else if (hitInfo [0].transform.gameObject.GetInstanceID () == attachedObject.gameObject.GetInstanceID ()) {
					moveObjectAgainst (ray, attachedObject.transform.position, new Axis (false, false, false));
				}
				// Other unidentified cases (offset in our direction)
				else {
					moveObjectAgainst (ray, attachedObject.transform.position, new Axis (false, false, true));
				}
				lazer.GetComponent<Renderer> ().material = lazerOn;
				lazer.transform.GetChild (0).gameObject.SetActive (true);
				lazer.GetComponent<AudioSource> ().enabled = true;
			}
			// 6th case : the ray doesn't go through anything (looses the object)
			else {
				Vector3 newPos = ray.origin + ray.direction * Vector3.Distance (ray.origin, attachedObject.transform.position);
				moveObjectAgainst (ray, attachedObject.transform.position, new Axis (false, false, false));
			}
		} 

		/*** THE USER IS MOVING THE MOUSE WITHOUT CLICKING ***/
		else {
			if (mirrorCasted || chestCasted) {
				lazer.GetComponent<Renderer> ().material = lazerMirror;
			} else if (rayCasted) {
				lazer.GetComponent<Renderer> ().material = lazerOK;
			} else {
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}
		}
	}


	/** 
	 * moveObjectAgainst allows us to move the object against another surface
	 * horizontal and vertical offset might be needed
	**/
	private void moveObjectAgainst (Ray ray, Vector3 referencePoint, Axis offsetAxis, bool teleport = false) {
		float offsetX = 0;
		float offsetY = 0;
		float offsetZ = 0;

		if (offsetAxis.x) {
			offsetX = attachedObject.transform.lossyScale.x / 2;
		}
		if (offsetAxis.y) {
			offsetY = attachedObject.transform.lossyScale.y / 2;
		}
		if (offsetAxis.z) {
			offsetZ = attachedObject.transform.lossyScale.z / 2;
		}

		Vector3 offset = new Vector3 (0, offsetY, 0);
		Vector3 newPos = ray.origin + ray.direction * (Vector3.Distance (ray.origin, referencePoint) - offsetZ) + offset;

		// So that the attached object doesn't go through the ground
		if (newPos.y < attachedObject.transform.lossyScale.y / 2f) {
			newPos.y = attachedObject.transform.lossyScale.y / 2f;
		}
		changePositionAndSize (newPos, teleport);
	}

	/**
	 * changePositionAndSize moves the attached object to its new position and
	 * changes its size according to the new distance between it and the user
	**/
	private void changePositionAndSize (Vector3 newPosition, bool teleport) {
		Vector3 attachedObjectGroundPosition = attachedObject.position;
		attachedObjectGroundPosition.y = newPosition.y;
		float GroundDistanceFirstPlane = Vector3.Distance (Camera.main.transform.position - new Vector3 (0, Camera.main.transform.position.y, 0), attachedObjectGroundPosition);
		float GroundDistanceSecondPlane = Vector3.Distance (Camera.main.transform.position - new Vector3 (0, Camera.main.transform.position.y, 0), newPosition);

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
			Debug.Log ("Impossible position");
		}
	}

	/**
	 * setAttachedObjectOrientation is used to change the attached object orientation
	 * so that it allways faces the user.
	 * Orientation on the X and Z axes are set to 0 to correct any unwanted orientation
	**/
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
			newRot.x = 0;
			newRot.z = 0;
			attachedObject.transform.rotation = Quaternion.Euler (newRot);
		}
	}

	/**
	 * applyShader is used to apply the self illuminated shader to selected object
	 * and then apply their original shader. Applys to all sub-objects of the
	 * attached object
	**/
	private void applyShader (string shaderToApply) {
		listOfObjectToShader = new List<Transform> ();
		if (attachedObject.GetComponent<Renderer> ()) {
			listOfObjectToShader.Add (attachedObject.transform);
		}
		for (int i = 0; i < attachedObject.transform.childCount; i++) {
			if (attachedObject.transform.GetChild (i).GetComponent<Renderer> ()) {
				listOfObjectToShader.Add (attachedObject.transform.GetChild (i));
			}
			getAllObjectsToShader (attachedObject.transform.GetChild (i));
		}
		if (listOfObjectToShader.Count > 0) {
			previousShader = listOfObjectToShader [0].GetComponent<Renderer> ().material.shader.name;
		}
		foreach (Transform transform in listOfObjectToShader) {
			transform.GetComponent<Renderer> ().material.shader = Shader.Find (shaderToApply);
		}
	}

	/**
	 * getAllObjectsToShader is a util function to get all sub-objects containing
	 * a Renderer component (on which you must apply the new shader)
	**/
	private void getAllObjectsToShader(Transform child) {
		for (int i = 0; i < child.childCount; i++) {
			if (child.GetChild (i).GetComponent<Renderer> ()) {
				listOfObjectToShader.Add (child.GetChild (i));
			}
			getAllObjectsToShader (child.GetChild (i));
		}
	}

	/**
	 * applyShadow is used to activate or not the shadows on the attached object.
	 * The shadow is set to off when the object is attached
	**/
	private void applyShadow (UnityEngine.Rendering.ShadowCastingMode shadowCastingMode) {
		listOfObjectToShadow = new List<Transform> ();
		if (attachedObject.GetComponent<MeshRenderer> ()) {
			listOfObjectToShadow.Add (attachedObject.transform);
		}
		for (int i = 0; i < attachedObject.transform.childCount; i++) {
			if (attachedObject.transform.GetChild (i).GetComponent<MeshRenderer> ()) {
				listOfObjectToShadow.Add (attachedObject.transform.GetChild (i));
			}
			getAllObjectsToShadow (attachedObject.transform.GetChild (i));
		}
		foreach (Transform transform in listOfObjectToShadow) {
			transform.GetComponent<MeshRenderer> ().shadowCastingMode = shadowCastingMode;

		}
	}

	/**
	 * getAllObjectsToShadow is a util function to get all sub-objects containing
	 * a MeshRenderer component (on which you must apply or not the new shadow)
	**/
	private void getAllObjectsToShadow(Transform child) {
		for (int i = 0; i < child.childCount; i++) {
			if (child.GetChild (i).GetComponent<MeshRenderer> ()) {
				listOfObjectToShadow.Add (child.GetChild (i));
			}
			getAllObjectsToShadow (child.GetChild (i));
		}
	}

	/**
	 * orderHitInfo orders the hitted objects by increasing distance to the user
	**/
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

	/**
	 * playerMoving is used to move the object when the player is also moving without
	 * changing its size
	**/
	private bool playerMoving() {
		if (transform.position == oldPlayerPos) {
			return false;
		} else {
			if (attachedObject != null) {
				attachedObject.MovePosition (attachedObject.transform.position + transform.position - oldPlayerPos);
			}
			oldPlayerPos = transform.position;
			return true;
		}
	}

	/**
	 * setAttachedObjectCollision is used to set the collision when attached object hits another object
	**/
	public void setAttachedObjectCollision(Collision collision) {
		attachedObjectCollision = collision;
	}

	/**
	 * getAttachedObjectCollision is used to get the collision when attached object hits another object
	**/
	public Collision getAttachedObjectCollision() {
		return attachedObjectCollision;
	}

	/**
	 * getAttachedObject is used to get the attachedObject
	**/
	public GameObject getAttachedObject() {
		if (attachedObject != null) {
			return attachedObject.gameObject;
		} else {
			return null;
		}
	}

	/**
	 * preventMovingAfter is used to prevent the attached object to move after the user releases it.
	 * (for exemple when it slightly enters into another object)
	**/
	public void preventMovingAfter(Rigidbody rb, float x){
		StartCoroutine (preventMovingCoroutine(rb, x));
	}

	/**
	 * preventMovingCoroutine waits and then cancel all the forces applying to the object so that it
	 * stays still
	**/
	private IEnumerator preventMovingCoroutine(Rigidbody rb, float x){
		yield return new WaitForSeconds (x);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}


